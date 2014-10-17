using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Fractal.Common;

namespace Fractal.Domain
{
    public class FractalDb
    {
        public static void InitAudit()
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                if (Audit(a => a.Child == null, fractalDb) == null)
                {
                    Audit initAudit = new Audit();
                    initAudit.Id = Guid.NewGuid().ToString();
                    initAudit.CreatedBy = "Init";
                    initAudit.CreatedDate = Clock.Now;
                    fractalDb.Audits.Add(initAudit);
                    fractalDb.SaveChanges();
                }
            }
        }

        public static void UpdateDc(DomainConcept dc)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                Attach(dc, fractalDb);

                Audit audit = CreateAudit(fractalDb);
                AuditDomainConcept auditDomainConcept = CreateAuditDomainConcept(fractalDb, audit, dc);

                if ( auditDomainConcept.Parent.HasChanged(dc))
                {
                    fractalDb.Entry(dc).State = EntityState.Modified;
                }
                dc.Fields.ForEach(field =>
                {
                    if (auditDomainConcept.AuditDomainConceptFields.Find(adcf => adcf.DcfId == field.Id).HasChanged(field))
                    {
                        fractalDb.Entry(field).State = EntityState.Modified;
                    }
                });

                foreach (ConnectionDescription leftConnectionDescription in dc.LeftConnectionDescriptions)
                {
                    AuditConnectionDescription lastAcdLeft = AuditConnectionDescription(acd => acd.Child == null && acd.CdId == leftConnectionDescription.Id, fractalDb);
                    lastAcdLeft.LeftDomainConcept = auditDomainConcept;
                    fractalDb.Entry(lastAcdLeft).State = EntityState.Modified;
                }
                foreach (ConnectionDescription rightConnectionDescription in dc.RightConnectionDescriptions)
                {
                    AuditConnectionDescription lastAcdRight = AuditConnectionDescription(acd => acd.Child == null && acd.CdId == rightConnectionDescription.Id, fractalDb);
                    lastAcdRight.RightDomainConcept = auditDomainConcept;
                    fractalDb.Entry(lastAcdRight).State = EntityState.Modified;
                }


                fractalDb.Audits.Add(audit);
                fractalDb.ADcs.Add(auditDomainConcept);
                fractalDb.ADcfs.AddRange(auditDomainConcept.AuditDomainConceptFields);
                fractalDb.SaveChanges();
            }
        }

        private static void Attach(DomainConcept dc, FractalContext fractalDb)
        {
            fractalDb.Dcs.Attach(dc);
            if (dc.Fields == null)
            {
                dc.Fields = Fields(dc, fractalDb);
            }
            else
            {
                dc.Fields.ForEach(f => fractalDb.Dcfs.Attach(f));
            }
            if (dc.LeftConnectionDescriptions == null)
            {
                dc.LeftConnectionDescriptions = Cds(cd => cd.RightDcId == dc.Id, fractalDb);
            }
            else
            {
                dc.LeftConnectionDescriptions.ForEach(lcd => fractalDb.Cds.Attach(lcd));
            }

            if (dc.RightConnectionDescriptions == null)
            {
                dc.RightConnectionDescriptions = Cds(cd => cd.LeftDcId == dc.Id, fractalDb);
            }
            else
            {
                dc.RightConnectionDescriptions.ForEach(rcd => fractalDb.Cds.Attach(rcd));
            }
        }

        public static DomainConcept CreateDomainConcept(string name, params string[] fields)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                Audit audit = CreateAudit(fractalDb);

                DomainConcept dc = new DomainConcept();
                dc.Id = NextId();
                dc.Name = name;

                List<DomainConceptField> domainConceptFields = new List<DomainConceptField>();
                foreach (string field in fields)
                {
                    DomainConceptField domainConceptField = new DomainConceptField();
                    domainConceptField.Id = NextId();
                    domainConceptField.FieldName = field;
                    domainConceptField.Forder = domainConceptFields.Count + 1;
                    domainConceptField.DomainConcept = dc;
                    domainConceptFields.Add(domainConceptField);
                }
                dc.Dcfs = domainConceptFields;
                dc.LeftCds = new List<ConnectionDescription>();
                dc.RightCds= new List<ConnectionDescription>();

                AuditDomainConcept auditDomainConcept = CreateAuditDomainConcept(fractalDb, audit, dc);

                // Objects
                fractalDb.Dcs.Add(dc);
                fractalDb.Dcfs.AddRange(dc.Fields);
                //Audit
                fractalDb.Audits.Add(audit);
                fractalDb.ADcs.Add(auditDomainConcept);
                fractalDb.ADcfs.AddRange(auditDomainConcept.AuditDomainConceptFields);
                fractalDb.SaveChanges();
    
                return dc;
            }
        }

        private static Audit CreateAudit(FractalContext fractalDb)
        {
            Audit lastAudit = Audit(a => a.Child == null, fractalDb);
            Audit audit = new Audit();
            audit.Id = NextId();
            audit.CreatedBy = "Init";
            audit.CreatedDate = Clock.Now;
            audit.Parent = lastAudit;
            audit.Aorder = lastAudit.Aorder + 1;
            lastAudit.Child = audit;
            return audit;
        }

        public static void RemoveDomainConcept(string domainConceptName)
        {
            WithAudit((fractalDb, audit) =>
            {
                DomainConcept dc = Dc(tdc => tdc.Name == domainConceptName, fractalDb);

                List<DomainConceptInstance> dcis = Dcis(dci=>dci.DomainConceptId == dc.Id, fractalDb, false);
                foreach (DomainConceptInstance dci in dcis)
                {
                    RemoveDomainConceptInstance(audit, dc, dci, fractalDb);
                }
                if (dc != null)
                {
                    dc.Fields.ForEach(f => fractalDb.Dcfs.Remove(f));
                    dc.Fields.ForEach(f => fractalDb.Entry(f).State = EntityState.Deleted);
                    dc.LeftConnectionDescriptions.ForEach(cd =>
                    {
                        cd.RightDc.RightConnectionDescriptions.Remove(cd);
                        fractalDb.Cds.Remove(cd);
                        fractalDb.Entry(cd).State = EntityState.Deleted;
                    });
                    dc.RightConnectionDescriptions.ForEach(cd =>
                    {
                        cd.LeftDc.RightConnectionDescriptions.Remove(cd);
                        fractalDb.Cds.Remove(cd);
                        fractalDb.Entry(cd).State = EntityState.Deleted;
                    });

                    fractalDb.Dcs.Remove(dc);
                    fractalDb.Entry(dc).State = EntityState.Deleted;
                }
                return dc;
            }, (fractalDb, auditDomainConcept, dc) =>
            {
                Audit audit = auditDomainConcept.Audit;
                foreach (ConnectionDescription leftConnectionDescription in dc.LeftConnectionDescriptions)
                {
                    AuditDomainConcept auditLeftDomainConcept = auditDomainConcept;
                    AuditDomainConcept auditRightDomainConcept = CreateAuditDomainConcept(fractalDb, audit, leftConnectionDescription.RightDc);

                    AuditConnectionDescription auditConnectionDescription = CreateAuditConnectionDescription(fractalDb, audit, leftConnectionDescription, auditLeftDomainConcept, auditRightDomainConcept);
                    auditConnectionDescription.Deleted = true;
                    auditConnectionDescription.ACdps.ForEach(acdp=>acdp.Deleted = true);
                    fractalDb.ACds.Add(auditConnectionDescription);
                    fractalDb.ACdps.AddRange(auditConnectionDescription.ACdps);
                    fractalDb.ADcs.Add(auditRightDomainConcept);
                    fractalDb.ADcfs.AddRange(auditRightDomainConcept.AuditDomainConceptFields);
                    
                }
                foreach (ConnectionDescription rightConnectionDescription in dc.RightConnectionDescriptions)
                {
                    AuditDomainConcept auditADomainConcept = CreateAuditDomainConcept(fractalDb, audit, rightConnectionDescription.LeftDc);
                    AuditDomainConcept auditBDomainConcept = auditDomainConcept;
                    AuditConnectionDescription auditConnectionDescription = CreateAuditConnectionDescription(fractalDb, audit, rightConnectionDescription, auditADomainConcept, auditBDomainConcept);
                    auditConnectionDescription.Deleted = true;
                    auditConnectionDescription.ACdps.ForEach(acdp => acdp.Deleted = true);
                    fractalDb.ACds.Add(auditConnectionDescription);
                    fractalDb.ACdps.AddRange(auditConnectionDescription.ACdps);
                    fractalDb.ADcs.Add(auditADomainConcept);
                    fractalDb.ADcfs.AddRange(auditADomainConcept.AuditDomainConceptFields);
                }

                auditDomainConcept.Deleted = true;
                auditDomainConcept.AuditDomainConceptFields.ForEach(adcf => adcf.Deleted = true);
                return true;
            });
        }

        private static void RemoveDomainConceptInstance(DomainConcept adc, DomainConceptInstance dci)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                RemoveDomainConceptInstance(adc, dci, fractalDb);
            }
        }

        private static void RemoveDomainConceptInstance(DomainConcept adc, DomainConceptInstance dci, FractalContext fractalDb)
        {
            RemoveDomainConceptInstance(null, adc, dci, fractalDb);
        }

        private static void RemoveDomainConceptInstance(Audit audit, DomainConcept adc, DomainConceptInstance dci, FractalContext fractalDb)
        {
            WithAudit(audit, fractalDb, adc.Name, (dc, fractaDb) =>
            {
                PoofChildren(dci, fractalDb);
                dci.Fields.ForEach(fv=> fractaDb.Dcifs.Remove(fv));
                dci.Fields.ForEach(fv=> fractaDb.Entry(fv).State = EntityState.Deleted );

                dci.LeftCons.ForEach(lcon =>
                {
                    DomainConceptInstance leftDci = lcon.LeftDci;
                    leftDci.RightCons = Connections(con => con.LeftDciId == dci.Id, fractalDb);
                    leftDci.RightCons.Remove(lcon);
                    fractalDb.Cons.Remove(lcon);
                    fractalDb.Entry(lcon).State = EntityState.Deleted;
                });
                dci.RightCons.ForEach(rcon =>
                {
                    DomainConceptInstance rightDci = rcon.RightDci;
                    rightDci.LeftCons = Connections(con => con.RightDciId == dci.Id, fractalDb);
                    rightDci.LeftCons.Remove(rcon);
                    fractalDb.Cons.Remove(rcon);
                    fractalDb.Entry(rcon).State = EntityState.Deleted;
                });
                
                fractalDb.Dcis.Remove(dci);
                fractalDb.Entry(dci).State = EntityState.Deleted;
                return dci;
            }, (auditDomainConceptInstance, fractalDbPassed) =>
            {
                auditDomainConceptInstance.AuditDomainConceptInstanceFieldValues.ForEach(adcif => adcif.Deleted = true);
                auditDomainConceptInstance.Deleted = true;
                return true;
            });
        }

        private static DomainConceptInstance PoofChildren(DomainConceptInstance dci, FractalContext fractalDb)
        {
            if (dci.Fields == null)
            {
                dci.Fields = Fields(dci, fractalDb);
            }
            else
            {
                dci.Fields.ForEach(f => fractalDb.Dcifs.Attach(f));
            }
            if (dci.LeftConnections == null)
            {
                dci.LeftConnections = Connections(con => con.RightDciId == dci.Id, fractalDb);
            }
            else
            {
                dci.LeftConnections.ForEach(lc => fractalDb.Cons.Attach(lc));
            }

            if (dci.RightConnections == null)
            {
                dci.RightConnections = Connections(con => con.LeftDciId == dci.Id, fractalDb);
            }
            else
            {
                dci.RightConnections.ForEach(rc => fractalDb.Cons.Attach(rc));
            }
            return dci;
        }


        public static DomainConcept Dc(Func<DomainConcept, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return Dc(func, fractalDb);
            }
        }

        public static List<DomainConceptField> Fields(DomainConcept dc)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return Fields(dc, fractalDb);
            }
        }

        public static List<DomainConceptField> Fields(DomainConcept dc, FractalContext fractalDb)
        {
            List<DomainConceptField> domainConceptFields = fractalDb.Dcfs.Where(f => f.Dc.Id == dc.Id).ToList();
            domainConceptFields.Sort(new ForderSorter());
            return domainConceptFields;
        }


        public static DomainConcept Dc(Func<DomainConcept, bool> func, FractalContext fractalDb)
        {
            List<DomainConcept> domainConcepts = Dcs(func, fractalDb);
            if (domainConcepts.Count > 0)
            {
                DomainConcept dc = domainConcepts.First();
                dc.Fields = Fields(dc, fractalDb);
                dc.LeftConnectionDescriptions = Cds(cd => cd.RightDcId == dc.Id, fractalDb);
                dc.RightConnectionDescriptions = Cds(cd => cd.LeftDcId == dc.Id, fractalDb);
                return dc;
            }
            return null;
        }


        public static AuditDomainConcept AuditDomainConcept(Func<AuditDomainConcept, bool> func)
        {
            List<AuditDomainConcept> adcs = AuditDomainConcepts(func);
            return adcs.Count>0?adcs.First():null;
        }

        public static AuditDomainConcept AuditDomainConcept(Func<AuditDomainConcept, bool> func, FractalContext fractalDb)
        {
            List<AuditDomainConcept> adcs = AuditDomainConcepts(func, fractalDb);
            return adcs.Count > 0 ? adcs.First() : null;
        }


        public static List<AuditDomainConcept> AuditDomainConcepts(Func<AuditDomainConcept, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return AuditDomainConcepts(func, fractalDb);
            }
        }


        public static List<AuditDomainConcept> AuditDomainConcepts(Func<AuditDomainConcept, bool> func, FractalContext fractalDb)
        {
                return fractalDb.ADcs.Where(func).ToList();
        }

        
        public static List<DomainConcept> Dcs()
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return fractalDb.Dcs.ToList();
            }
        }

        public static List<DomainConcept> Dcs(Func<DomainConcept, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return Dcs(func, fractalDb);
            }
        }

        public static List<DomainConcept> Dcs(Func<DomainConcept, bool> func, FractalContext fractalDb)
        {
                return fractalDb.Dcs.Where(func).ToList();
        }

        public static List<AuditDomainConcept> History(DomainConcept dc)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return fractalDb.ADcs.Where(adc => adc.DomainConceptId == dc.Id).OrderBy(adc=>adc.Aorder).ToList();
            }
        }

        public static void AddDomainConceptField(string domainConceptName, string field)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                Audit audit = CreateAudit(fractalDb);

                DomainConcept dc = Dc(adc => adc.Name == domainConceptName, fractalDb);
                DomainConceptField domainConceptField = new DomainConceptField();
                domainConceptField.Id = NextId();
                domainConceptField.FieldName = field;
                domainConceptField.Forder = dc.Fields.Count + 1;
                dc.Fields.Add(domainConceptField);

                AuditDomainConcept auditDomainConcept = CreateAuditDomainConcept(fractalDb, audit, dc);

                fractalDb.Dcfs.Add(domainConceptField);
                fractalDb.Audits.Add(audit);
                fractalDb.ADcs.Add(auditDomainConcept);
                fractalDb.ADcfs.AddRange(auditDomainConcept.AuditDomainConceptFields);
                fractalDb.SaveChanges();
            }
        }

        private static AuditDomainConcept CreateAuditDomainConcept(FractalContext fractalDb, Audit audit, DomainConcept dc)
        {
            AuditDomainConcept lastAdc = AuditDomainConcept(adc => adc.Child == null && adc.DomainConceptId == dc.Id, fractalDb);
            AuditDomainConcept auditDomainConcept = new AuditDomainConcept();
            auditDomainConcept.AuditDomainConceptId = NextId();
            auditDomainConcept.Audit = audit;
            auditDomainConcept.Parent = lastAdc;
            auditDomainConcept.DomainConceptId = dc.Id;
            auditDomainConcept.Name = dc.Name;
            auditDomainConcept.Aorder = lastAdc==null?0:lastAdc.Aorder + 1;

            foreach (DomainConceptField field in dc.Fields)
            {
                AuditDomainConceptField auditDomainConceptField = new AuditDomainConceptField();
                auditDomainConceptField.AuditDomainConceptFieldId = NextId();
                auditDomainConceptField.DcfId = field.Id;
                auditDomainConceptField.FieldName = field.FieldName;
                auditDomainConceptField.Forder = field.Forder;
                auditDomainConceptField.Adc = auditDomainConcept;
                auditDomainConcept.AddAuditDomainConceptField(auditDomainConceptField);
            }
            audit.AddAuditDomainConcept(auditDomainConcept);
            if (lastAdc != null)
            {
                lastAdc.Child = auditDomainConcept;
            }
            return auditDomainConcept;
        }

        private static string NextId()
        {
            return Guid.NewGuid().ToString();
        }

        public static void RemoveDomainConceptField(string domainConceptName, string field)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                Audit audit = CreateAudit(fractalDb);
                DomainConcept dc = Dc(adc => adc.Name == domainConceptName, fractalDb);
                DomainConceptField fieldToRemove = dc.Fields.Find(f=> f.FieldName == field);
                fractalDb.Entry(fieldToRemove).State = EntityState.Deleted;
                dc.Fields.Remove(fieldToRemove);
                AuditDomainConcept auditDomainConcept = CreateAuditDomainConcept(fractalDb, audit, dc);

                fractalDb.Audits.Add(audit);
                fractalDb.ADcs.Add(auditDomainConcept);
                fractalDb.ADcfs.AddRange(auditDomainConcept.AuditDomainConceptFields);
                fractalDb.SaveChanges();
            }
        }

        private static void WithAudit(Func<FractalContext, Audit, DomainConcept> func, Func<FractalContext, AuditDomainConcept,DomainConcept, bool> auditFunc)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                Audit audit = CreateAudit(fractalDb);
                DomainConcept dc = func(fractalDb, audit);
                if (dc!= null)
                {
                    AuditDomainConcept auditDomainConcept = CreateAuditDomainConcept(fractalDb, audit, dc);
                    auditFunc(fractalDb, auditDomainConcept, dc);

                    //Audit
                    fractalDb.Audits.Add(audit);
                    fractalDb.ADcs.Add(auditDomainConcept);
                    fractalDb.ADcfs.AddRange(auditDomainConcept.AuditDomainConceptFields);
                    fractalDb.SaveChanges();
                }
            }
        }

        public static int Dec(int val)
        {
            return --val;
        }

        public static int Inc(int val)
        {
            return ++val;
        }

        public static void SwapFieldRight(string domainConceptName, string field)
        {
            SwapField(domainConceptName, field, Inc);
        }

        public static void SwapFieldLeft(string domainConceptName, string field)
        {
            SwapField(domainConceptName, field, Dec);
        }

        private static void SwapField(string domainConceptName, string field, Func<int, int> move )
        {
            WithAudit((fractalDb, audit) =>
            {
                DomainConcept dc = Dc(adc => adc.Name == domainConceptName, fractalDb);
                DomainConceptField fieldToMove = dc.Fields.Find(f => f.FieldName == field);
                DomainConceptField matchingField = dc.Fields.Find(f => f.Forder == move(fieldToMove.Forder));
                if (matchingField != null)
                {
                    int firstForder = fieldToMove.Forder;
                    int matchingForder = matchingField.Forder;
                    fieldToMove.Forder = matchingForder;
                    matchingField.Forder = firstForder;
                    return dc;
                } 
                return null;
            }, (fractaldb, auditDomainConcept, dc) => true);
        }

        private static DomainConceptInstance WithAudit(Audit audit, FractalContext fractalDb, string domainConceptName, Func<DomainConcept, FractalContext, DomainConceptInstance> func, Func<AuditDomainConceptInstance, FractalContext, bool> auditFunc)
        {
            bool useExistingAudit = audit == null;
            if (useExistingAudit) { audit = CreateAudit(fractalDb); }
            DomainConcept dc = Dc(adc => adc.Name == domainConceptName, fractalDb);
            DomainConceptInstance dci = func(dc, fractalDb);

            if (dci != null)
            {
                AuditDomainConceptInstance auditDomainConceptInstance = CreateAuditDomainConceptInstance(fractalDb, audit, dci);
                auditFunc(auditDomainConceptInstance, fractalDb);
                if (useExistingAudit) { fractalDb.Audits.Add(audit);}
                fractalDb.ADcis.Add(auditDomainConceptInstance);
                fractalDb.ADcifs.AddRange(auditDomainConceptInstance.AuditDomainConceptInstanceFieldValues);
                if (useExistingAudit) { fractalDb.SaveChanges(); };
            }
            return dci;
        }

        private static DomainConceptInstance WithAudit(string domainConceptName, Func<DomainConcept, FractalContext, DomainConceptInstance> func, Func<AuditDomainConceptInstance, FractalContext, bool> auditFunc)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return WithAudit(null, fractalDb, domainConceptName, func, auditFunc);
            }
        }

        public static DomainConceptInstance CreateDomainConceptInstance(string domainConceptName, params string[] fieldValues)
        {
            return WithAudit(domainConceptName, (dc, fractalDb) =>
                {
                    DomainConceptInstance dci = new DomainConceptInstance();
                    dci.Id = NextId();
                    dci.DomainConceptName = dc.Name;
                    dci.DomainConceptId = dc.Id;

                    int i = 0;
                    List<DomainConceptInstanceFieldValue> domainConceptInstanceFieldValues = new List<DomainConceptInstanceFieldValue>();
                    foreach (DomainConceptField domainConceptField in dc.Fields.OrderBy(f=>f.Forder))
                    {
                        DomainConceptInstanceFieldValue dcif = new DomainConceptInstanceFieldValue();
                        dcif.Id = NextId();
                        dcif.DomainConceptInstance = dci;
                        dcif.DomainConceptName = dc.Name;
                        dcif.DomainConceptId = dc.Id;
                        dcif.DomainConceptFieldId = domainConceptField.Id;
                        dcif.DomainConceptFieldName = domainConceptField.FieldName;
                        dcif.Forder = domainConceptField.Forder;
                        dcif.FieldValue = fieldValues[i++];

                        domainConceptInstanceFieldValues.Add(dcif);
                    }
                    domainConceptInstanceFieldValues.Sort(new DciFieldSorter());
                    dci.Fields = domainConceptInstanceFieldValues;

                    // Objects
                    fractalDb.Dcis.Add(dci);
                    fractalDb.Dcifs.AddRange(dci.Fields);

                    return dci;

                }, (auditDomainConceptInstance, fractalDbPassed) => true);
        }

        private static AuditDomainConceptInstance CreateAuditDomainConceptInstance(FractalContext fractalDb, Audit audit, DomainConceptInstance dci)
        {
            AuditDomainConceptInstance lastAdci = Adci(adci => adci.Child == null && adci.DomainConceptInstanceId == dci.Id, fractalDb);
            AuditDomainConceptInstance auditDomainConceptInstance = new AuditDomainConceptInstance();
            auditDomainConceptInstance.AuditDomainConceptInstanceId = NextId();
            auditDomainConceptInstance.Audit = audit;
            auditDomainConceptInstance.Parent = lastAdci;
            auditDomainConceptInstance.DomainConceptInstanceId = dci.Id;
            auditDomainConceptInstance.DomainConceptId = dci.DomainConceptId;
            auditDomainConceptInstance.DomainConceptName = dci.DomainConceptName;
            auditDomainConceptInstance.Aorder = lastAdci == null ? 0 : lastAdci.Aorder + 1;

            foreach (DomainConceptInstanceFieldValue field in dci.Fields)
            {
                AuditDomainConceptInstanceFieldValue auditDomainConceptInstanceFieldValue = new AuditDomainConceptInstanceFieldValue();
                auditDomainConceptInstanceFieldValue.AuditDomainConceptInstanceFieldValueId = NextId();

                auditDomainConceptInstanceFieldValue.DomainConceptInstanceFieldValueId = field.Id;
                auditDomainConceptInstanceFieldValue.AuditDomainConceptInstance = auditDomainConceptInstance;
                auditDomainConceptInstanceFieldValue.DomainConceptId = field.DomainConceptId;
                auditDomainConceptInstanceFieldValue.DomainConceptName = field.DomainConceptName;
                auditDomainConceptInstanceFieldValue.DomainConceptFieldId = field.DomainConceptFieldId;
                auditDomainConceptInstanceFieldValue.DomainConceptFieldName = field.DomainConceptFieldName;
                auditDomainConceptInstanceFieldValue.Forder = field.Forder;
                auditDomainConceptInstanceFieldValue.FieldValue = field.FieldValue;

                auditDomainConceptInstance.AuditDomainConceptInstanceFieldValues.Add(auditDomainConceptInstanceFieldValue);
            }

            audit.AuditDomainConceptInstances.Add(auditDomainConceptInstance);
            if (lastAdci != null)
            {
                lastAdci.Child = auditDomainConceptInstance;
            }
            return auditDomainConceptInstance;

        }

        public static AuditDomainConceptInstance Adci(Func<AuditDomainConceptInstance, bool> func)
        {
            List<AuditDomainConceptInstance> adcs = ADcis(func);
            return adcs.Count > 0 ? adcs.First() : null;
        }

        public static AuditDomainConceptInstance Adci(Func<AuditDomainConceptInstance, bool> func, FractalContext fractalDb)
        {
            List<AuditDomainConceptInstance> adcis = ADcis(func, fractalDb);
            return adcis.Count > 0 ? adcis.First() : null;
        }


        public static List<AuditDomainConceptInstance> ADcis(Func<AuditDomainConceptInstance, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return ADcis(func, fractalDb);
            }
        }

        public static List<AuditDomainConceptInstance> ADcis(Func<AuditDomainConceptInstance, bool> func, FractalContext fractalDb)
        {
            return fractalDb.ADcis.Where(func).ToList();
        }




        public static DomainConceptInstance Dci(Func<DomainConceptInstance, bool> func)
        {
            List<DomainConceptInstance> dcs = Dcis(func, false);
            return dcs.Count > 0 ? dcs.First() : null;
        }

        public static DomainConceptInstance Dci(Func<DomainConceptInstance, bool> func, FractalContext fractalDb)
        {
            List<DomainConceptInstance> dcs = Dcis(func, fractalDb, true);
            return dcs.Count > 0 ? dcs.First() : null;
        }

        public static List<DomainConceptInstance> Dcis(Func<DomainConceptInstance, bool> func, bool includeConnections)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return Dcis(func, fractalDb, includeConnections);
            }
        }

        public static List<DomainConceptInstance> Dcis(Func<DomainConceptInstance, bool> func, FractalContext fractalDb, bool includeConnections)
        {
            List<DomainConceptInstance> domainConceptInstances = fractalDb.Dcis.Where(func).ToList();
            if (includeConnections)
            {
                domainConceptInstances.ForEach(dci=> { dci.AllLeftConnections();dci.AllRightConnections();});
            }
            return domainConceptInstances; ;
        }


        public static void UpdateDci(DomainConceptInstance dci)
        {
            WithAudit(dci.DomainConceptName, (dc, fractalDb) =>
            {
                fractalDb.Dcis.Attach(dci);
                return dci;
            }, (auditDomainConceptInstance, fractalDb) =>
            {
                bool foundChange = false;
                dci.Fields.ForEach(field =>
                {
                    if (auditDomainConceptInstance.AuditDomainConceptInstanceFieldValues.Find(adcfv => adcfv.DomainConceptFieldId == field.DomainConceptFieldId).HasChanged(field))
                    {
                        fractalDb.Entry(field).State = EntityState.Modified;
                        foundChange = true;
                    }
                });
                return foundChange;
            });
        }

        public static List<DomainConceptInstance> Select(string domainConceptName, WhereClause where)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return Dcis(dci => dci.DomainConceptName.Equals(domainConceptName) &&
                               dci.GetFields(fractalDb).Find(where.Check) != null, fractalDb, false);
            }
        }

        public static WhereClause Where(params string[] fieldsValues)
        {
            WhereClause whereClause = new WhereClause();
            for (int i = 0; i < fieldsValues.Length; i = i + 2)
            {
                whereClause.Ands.Add(new AndClause(fieldsValues[i], fieldsValues[i + 1]));
            }
            return whereClause;
        }

        public static ConnectionDescription CreateConnectionDescription(DomainConcept dcLeft, DomainConcept dcRight, string connectionName, Cardinality cardinality, bool directed, bool required, bool reciprical)
        {
            return WithAudit((fractalDb) =>
            {
                ConnectionDescription connectionDescription = new ConnectionDescription();
                connectionDescription.Id = NextId();
                connectionDescription.LeftDomainConcept = dcLeft;
                connectionDescription.RightDomainConcept = dcRight;
                connectionDescription.ConnectionName = connectionName;
                connectionDescription.Cardinality = cardinality;
                connectionDescription.Directed = directed;
                connectionDescription.Required = required;
                connectionDescription.Reciprical = reciprical;

                fractalDb.Dcs.Attach(dcLeft);
                fractalDb.Dcs.Attach(dcRight);
                dcLeft.RightConnectionDescriptions.Add(connectionDescription);
                dcRight.LeftConnectionDescriptions.Add(connectionDescription);
                fractalDb.Cds.Add(connectionDescription);

                return connectionDescription;

            }, (auditConnectionDescription, fractalDbPassed) => true
            );
        }

        private static ConnectionDescription WithAudit(FractalContext fractalDb, Func<FractalContext, ConnectionDescription> func, Func<AuditConnectionDescription, FractalContext, bool> auditFunc)
        {
            Audit audit = CreateAudit(fractalDb);
            ConnectionDescription connectionDescription = func(fractalDb);

            if (connectionDescription != null)
            {
                AuditDomainConcept auditLeftDomainConcept = CreateAuditDomainConcept(fractalDb, audit, connectionDescription.LeftDc);
                AuditDomainConcept auditRightDomainConcept = CreateAuditDomainConcept(fractalDb, audit, connectionDescription.RightDc);
                AuditConnectionDescription auditConnectionDescription = CreateAuditConnectionDescription(fractalDb, audit, connectionDescription, auditLeftDomainConcept, auditRightDomainConcept);

                auditFunc(auditConnectionDescription, fractalDb);

                fractalDb.Audits.Add(audit);
                fractalDb.ACds.Add(auditConnectionDescription);
                if (auditConnectionDescription.ACdps != null) { fractalDb.ACdps.AddRange(auditConnectionDescription.ACdps);}
                fractalDb.ADcs.Add(auditLeftDomainConcept);
                if (auditLeftDomainConcept.ADcfs != null) { fractalDb.ADcfs.AddRange(auditLeftDomainConcept.ADcfs); }
                fractalDb.ADcs.Add(auditRightDomainConcept);
                if (auditRightDomainConcept.ADcfs != null) { fractalDb.ADcfs.AddRange(auditRightDomainConcept.ADcfs); }
                fractalDb.SaveChanges();
            }
            return connectionDescription;
        }


        private static ConnectionDescription WithAudit(Func<FractalContext, ConnectionDescription> func, Func<AuditConnectionDescription, FractalContext, bool> auditFunc)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return WithAudit(fractalDb, func, auditFunc);
            }
        }


        private static AuditConnectionDescription CreateAuditConnectionDescription(FractalContext fractalDb, Audit audit, ConnectionDescription connectionDescription, AuditDomainConcept auditADomainConcept, AuditDomainConcept auditBDomainConcept)
        {
            AuditConnectionDescription lastAcd = AuditConnectionDescription(acd => acd.Child == null && acd.CdId == connectionDescription.Id, fractalDb);
            AuditConnectionDescription auditConnectionDescription = new AuditConnectionDescription();
            auditConnectionDescription.AuditConnectionDescriptionId = NextId();
            auditConnectionDescription.Audit = audit;
            auditConnectionDescription.Parent = lastAcd;
            auditConnectionDescription.Aorder = lastAcd == null ? 0 : lastAcd.Aorder + 1;

            auditConnectionDescription.CdId = connectionDescription.Id;
            auditConnectionDescription.LeftDomainConcept = auditADomainConcept;
            auditConnectionDescription.RightDomainConcept = auditBDomainConcept;
            auditConnectionDescription.ConnectionName = connectionDescription.ConnectionName;
            auditConnectionDescription.Reciprical = connectionDescription.Reciprical;
            auditConnectionDescription.Cardinality = connectionDescription.Cardinality;
            auditConnectionDescription.Directed = connectionDescription.Directed;
            auditConnectionDescription.Required = connectionDescription.Required;

            foreach (ConnectionDescriptionParameter connectionDescriptionParameter in connectionDescription.ConnectionDescriptionParameters)
            {
                AuditConnectionDescriptionParameter auditConnectionDescriptionParameter = new AuditConnectionDescriptionParameter();
                auditConnectionDescriptionParameter.AuditConnectionDescriptionParameterId = NextId();
                auditConnectionDescriptionParameter.ConnectionDescriptionParameterId = connectionDescriptionParameter.Id;
                auditConnectionDescriptionParameter.Code = connectionDescriptionParameter.Code;
                auditConnectionDescriptionParameter.Description = connectionDescriptionParameter.Description;
                auditConnectionDescriptionParameter.FunctionCode = connectionDescriptionParameter.FunctionCode;
            }

            audit.AuditConnectionDescriptions.Add(auditConnectionDescription);

            if (lastAcd != null)
            {
                lastAcd.Child = auditConnectionDescription;
            }
            return auditConnectionDescription;

        }


        public static AuditConnectionDescription AuditConnectionDescription(Func<AuditConnectionDescription, bool> func)
        {
            List<AuditConnectionDescription> adcs = AuditConnectionDescriptions(func);
            return adcs.Count > 0 ? adcs.First() : null;
        }

        public static AuditConnectionDescription AuditConnectionDescription(Func<AuditConnectionDescription, bool> func, FractalContext fractalDb)
        {
            List<AuditConnectionDescription> adcis = AuditConnectionDescriptions(func, fractalDb);
            return adcis.Count > 0 ? adcis.First() : null;
        }


        public static List<AuditConnectionDescription> AuditConnectionDescriptions(Func<AuditConnectionDescription, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return AuditConnectionDescriptions(func, fractalDb);
            }
        }

        public static List<AuditConnectionDescription> AuditConnectionDescriptions(Func<AuditConnectionDescription, bool> func, FractalContext fractalDb)
        {
            return fractalDb.ACds.Where(func).ToList();
        }


        public static List<AuditConnectionDescription> History(ConnectionDescription cd)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return fractalDb.ACds.Where(acd => acd.CdId == cd.Id).OrderBy(adc => adc.Aorder).ToList();
            }
        }



        public static ConnectionDescription Cd(Func<ConnectionDescription, bool> func)
        {
            List<ConnectionDescription> dcs = Cds(func);
            return dcs.Count > 0 ? dcs.First() : null;
        }

        public static ConnectionDescription Cd(Func<ConnectionDescription, bool> func, FractalContext fractalDb)
        {
            List<ConnectionDescription> cds = Cds(func, fractalDb);
            return cds.Count > 0 ? cds.First() : null;
        }


        public static List<ConnectionDescription> Cds(Func<ConnectionDescription, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return Cds(func, fractalDb);
            }
        }

        public static List<ConnectionDescription> Cds(Func<ConnectionDescription, bool> func, FractalContext fractalDb)
        {
            return fractalDb.Cds.Where(func).ToList();
        }

        
        
        public static Connection Connect(DomainConceptInstance dciLeft, DomainConceptInstance dciRight, string cdName)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
//                if (HasConnection(dciLeft, dciRight, "cdName"))
                Audit audit = CreateAudit(fractalDb);

                
                ConnectionDescription connectionDescription = Cd(cd=>cd.ConnectionName == cdName && cd.GetDcLeft(fractalDb).Name == dciLeft.DomainConceptName && cd.GetDcRight(fractalDb).Name == dciRight.DomainConceptName, fractalDb);
                if (connectionDescription == null)
                {
                    return null;
                }
                
                Connection connection = new Connection();
                connection.Id = NextId();
                connection.ConnectionDescription = connectionDescription;
                connection.LeftDomainConceptInstance = dciLeft;
                connection.RightDomainConceptInstance = dciRight;

                fractalDb.Dcis.Attach(dciLeft);
                fractalDb.Dcis.Attach(dciRight);
                PoofChildren(dciLeft, fractalDb);
                PoofChildren(dciRight, fractalDb);
                dciLeft.RightConnections.Add(connection);
                dciRight.LeftConnections.Add(connection);
                fractalDb.Cons.Add(connection);

                AuditDomainConceptInstance auditLeftDomainConceptInstance = CreateAuditDomainConceptInstance(fractalDb, audit, connection.LeftDci);
                AuditDomainConceptInstance auditRightDomainConceptInstance = CreateAuditDomainConceptInstance(fractalDb, audit, connection.RightDci);
                AuditConnection auditConnection = CreateAuditConnection(fractalDb, audit, connection, auditLeftDomainConceptInstance, auditRightDomainConceptInstance);

                fractalDb.Audits.Add(audit);
                fractalDb.ADcis.Add(auditLeftDomainConceptInstance);
                fractalDb.ADcis.Add(auditRightDomainConceptInstance);
                fractalDb.ACons.Add(auditConnection);

                fractalDb.SaveChanges();

                return connection;

            }
        }

        private static AuditConnection CreateAuditConnection(FractalContext fractalDb, Audit audit, Connection connection, AuditDomainConceptInstance auditLeftDomainConceptInstance, AuditDomainConceptInstance auditRightDomainConceptInstance)
        {
            AuditConnection lastAcon = AuditConnection(acon => acon.ChiId == null && acon.CdId == connection.Id, fractalDb);
            AuditConnection auditConnection = new AuditConnection();
            auditConnection.AuditConnectionId = NextId();
            auditConnection.Audit = audit;
            auditConnection.Parent = lastAcon;
            auditConnection.Aorder = lastAcon == null ? 0 : lastAcon.Aorder + 1;

            auditConnection.ConId = connection.Id;
            auditConnection.CdId = connection.CdId;
            auditConnection.LeftDomainConceptInstance = auditLeftDomainConceptInstance;
            auditConnection.RightDomainConceptInstance = auditRightDomainConceptInstance;

            audit.AddAuditConnection(auditConnection);

            if (lastAcon != null)
            {
                lastAcon.Child = auditConnection;
            }
            return auditConnection;
        }

        public static List<DomainConceptInstanceFieldValue> Fields(DomainConceptInstance dci)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return Fields(dci, fractalDb);
            }
        }

        public static List<DomainConceptInstanceFieldValue> Fields(DomainConceptInstance dci, FractalContext fractalDb)
        {
            List<DomainConceptInstanceFieldValue> domainConceptFields = fractalDb.Dcifs.Where(f => f.DciId == dci.Id).ToList();
            domainConceptFields.Sort(new DciFieldSorter());
            return domainConceptFields;
        }


        public static Connection Connection(Func<Connection, bool> func)
        {
            List<Connection> cons = Connections(func);
            return cons.Count > 0 ? cons.First() : null;
        }

        public static Connection Connection(Func<Connection, bool> func, FractalContext fractalDb)
        {
            List<Connection> cons = Connections(func, fractalDb);
            return cons.Count > 0 ? cons.First() : null;
        }


        public static List<Connection> Connections(Func<Connection, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return Connections(func, fractalDb);
            }
        }

        public static List<Connection> Connections(Func<Connection, bool> func, FractalContext fractalDb)
        {
            return fractalDb.Cons.Where(func).ToList();
        }


        public static Audit Audit(Func<Audit, bool> func)
        {
            List<Audit> audits = Audits(func);
            return audits.Count > 0 ? audits.First() : null;
        }

        public static Audit Audit(Func<Audit, bool> func, FractalContext fractalDb)
        {
            List<Audit> audits = Audits(func, fractalDb);
            return audits.Count > 0 ? audits.First() : null;
        }


        public static List<Audit> Audits(Func<Audit, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return Audits(func, fractalDb);
            }
        }

        public static List<Audit> Audits(Func<Audit, bool> func, FractalContext fractalDb)
        {
            return fractalDb.Audits.Where(func).ToList();
        }

        public static AuditDomainConceptField AuditDomainConceptField(Func<AuditDomainConceptField, bool> func)
        {
            List<AuditDomainConceptField> adcfs = AuditDomainConceptFields(func);
            return adcfs.Count > 0 ? adcfs.First() : null;
        }

        public static AuditDomainConceptField AuditDomainConceptField(Func<AuditDomainConceptField, bool> func, FractalContext fractalDb)
        {
            List<AuditDomainConceptField> adcfs = AuditDomainConceptFields(func, fractalDb);
            return adcfs.Count > 0 ? adcfs.First() : null;
        }


        public static List<AuditDomainConceptField> AuditDomainConceptFields(Func<AuditDomainConceptField, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return AuditDomainConceptFields(func, fractalDb);
            }
        }

        public static List<AuditDomainConceptField> AuditDomainConceptFields(Func<AuditDomainConceptField, bool> func, FractalContext fractalDb)
        {
            return fractalDb.ADcfs.Where(func).ToList();
        }


        public static AuditConnectionDescriptionParameter AuditConnectionDescriptionParameter(Func<AuditConnectionDescriptionParameter, bool> func)
        {
            List<AuditConnectionDescriptionParameter> adcifs = AuditConnectionDescriptionParameters(func);
            return adcifs.Count > 0 ? adcifs.First() : null;
        }

        public static AuditConnectionDescriptionParameter AuditConnectionDescriptionParameter(Func<AuditConnectionDescriptionParameter, bool> func, FractalContext fractalDb)
        {
            List<AuditConnectionDescriptionParameter> adcifs = AuditConnectionDescriptionParameters(func, fractalDb);
            return adcifs.Count > 0 ? adcifs.First() : null;
        }


        public static List<AuditConnectionDescriptionParameter> AuditConnectionDescriptionParameters(Func<AuditConnectionDescriptionParameter, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return AuditConnectionDescriptionParameters(func, fractalDb);
            }
        }

        public static List<AuditConnectionDescriptionParameter> AuditConnectionDescriptionParameters(Func<AuditConnectionDescriptionParameter, bool> func, FractalContext fractalDb)
        {
            return fractalDb.ACdps.Where(func).ToList();
        }


        public static AuditDomainConceptInstanceFieldValue AuditDomainConceptInstanceFieldValue(Func<AuditDomainConceptInstanceFieldValue, bool> func)
        {
            List<AuditDomainConceptInstanceFieldValue> adcifs = AuditDomainConceptInstanceFieldValues(func);
            return adcifs.Count > 0 ? adcifs.First() : null;
        }

        public static AuditDomainConceptInstanceFieldValue AuditDomainConceptInstanceFieldValue(Func<AuditDomainConceptInstanceFieldValue, bool> func, FractalContext fractalDb)
        {
            List<AuditDomainConceptInstanceFieldValue> adcifs = AuditDomainConceptInstanceFieldValues(func, fractalDb);
            return adcifs.Count > 0 ? adcifs.First() : null;
        }


        public static List<AuditDomainConceptInstanceFieldValue> AuditDomainConceptInstanceFieldValues(Func<AuditDomainConceptInstanceFieldValue, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return AuditDomainConceptInstanceFieldValues(func, fractalDb);
            }
        }

        public static List<AuditDomainConceptInstanceFieldValue> AuditDomainConceptInstanceFieldValues(Func<AuditDomainConceptInstanceFieldValue, bool> func, FractalContext fractalDb)
        {
            return fractalDb.ADcifs.Where(func).ToList();
        }


        public static ConnectionDescriptionParameter ConnectionDescriptionParameter(Func<ConnectionDescriptionParameter, bool> func)
        {
            List<ConnectionDescriptionParameter> cdps = ConnectionDescriptionParameters(func);
            return cdps.Count > 0 ? cdps.First() : null;
        }

        public static ConnectionDescriptionParameter ConnectionDescriptionParameter(Func<ConnectionDescriptionParameter, bool> func, FractalContext fractalDb)
        {
            List<ConnectionDescriptionParameter> cdps = ConnectionDescriptionParameters(func, fractalDb);
            return cdps.Count > 0 ? cdps.First() : null;
        }


        public static List<ConnectionDescriptionParameter> ConnectionDescriptionParameters(Func<ConnectionDescriptionParameter, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return ConnectionDescriptionParameters(func, fractalDb);
            }
        }

        public static List<ConnectionDescriptionParameter> ConnectionDescriptionParameters(Func<ConnectionDescriptionParameter, bool> func, FractalContext fractalDb)
        {
            return fractalDb.Cdps.Where(func).ToList();
        }


        public static AuditConnection AuditConnection(Func<AuditConnection, bool> func)
        {
            List<AuditConnection> acons = AuditConnections(func);
            return acons.Count > 0 ? acons.First() : null;
        }

        public static AuditConnection AuditConnection(Func<AuditConnection, bool> func, FractalContext fractalDb)
        {
            List<AuditConnection> acons = AuditConnections(func, fractalDb);
            return acons.Count > 0 ? acons.First() : null;
        }


        public static List<AuditConnection> AuditConnections(Func<AuditConnection, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return AuditConnections(func, fractalDb);
            }
        }

        public static List<AuditConnection> AuditConnections(Func<AuditConnection, bool> func, FractalContext fractalDb)
        {
            return fractalDb.ACons.Where(func).ToList();
        }


    }

    public class DciFieldSorter : IComparer<DomainConceptInstanceFieldValue>
    {
        public int Compare(DomainConceptInstanceFieldValue x, DomainConceptInstanceFieldValue y)
        {
            return x.Forder.CompareTo(y.Forder);
        }
    }


    public class ForderSorter : IComparer<DomainConceptField>
    {
        public int Compare(DomainConceptField x, DomainConceptField y)
        {
            return x.Forder.CompareTo(y.Forder);
        }
    }
}