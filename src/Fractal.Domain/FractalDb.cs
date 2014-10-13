using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Fractal.Common;
using Fractal.Domain.Migrations;

namespace Fractal.Domain
{
    public class FractalDb
    {
        public static void InitAudit()
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                if (!fractalDb.Audits.Any())
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
                fractalDb.Dcs.Attach(dc);
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

                foreach (ConnectionDescription aConnectionDescription in dc.AConnectionDescriptions)
                {
                    AuditConnectionDescription lastAcdA = ACd(acd => acd.Child == null && acd.CdId == aConnectionDescription.Id, fractalDb);
                    lastAcdA.DcA = auditDomainConcept;
                    fractalDb.Entry(lastAcdA).State = EntityState.Modified;
                }
                foreach (ConnectionDescription bConnectionDescription in dc.BConnectionDescriptions)
                {
                    AuditConnectionDescription lastAcdB = ACd(acd => acd.Child == null && acd.CdId == bConnectionDescription.Id, fractalDb);
                    lastAcdB.DcB = auditDomainConcept;
                    fractalDb.Entry(lastAcdB).State = EntityState.Modified;
                }


                fractalDb.Audits.Add(audit);
                fractalDb.ADcs.Add(auditDomainConcept);
                fractalDb.ADcfs.AddRange(auditDomainConcept.AuditDomainConceptFields);
                fractalDb.SaveChanges();
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

                foreach (string field in fields)
                {
                    DomainConceptField domainConceptField = new DomainConceptField();
                    domainConceptField.Id = NextId();
                    domainConceptField.FieldName = field;
                    domainConceptField.Forder = dc.Fields.Count + 1;
                    dc.Fields.Add(domainConceptField);
                }

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
            Audit lastAudit = fractalDb.Audits.First(a => a.Child == null);
            Audit audit = new Audit();
            audit.Id = NextId();
            audit.CreatedBy = "Init";
            audit.CreatedDate = Clock.Now;
            audit.Parent = lastAudit;
            lastAudit.Child = audit;
            return audit;
        }

        public static void RemoveDomainConcept(string domainConceptName)
        {
            WithAudit((fractalDb) =>
            {
                DomainConcept dc = Dc(tdc => tdc.Name == domainConceptName, fractalDb);
                List<DomainConceptInstance> dcis = Dcis(dci=>dci.DomainConceptId == dc.Id, fractalDb);
                foreach (DomainConceptInstance dci in dcis)
                {
                    RemoveDomainConceptInstance(dc, dci, fractalDb);
                }
                if (dc != null)
                {
                    dc.Fields.ForEach(f => fractalDb.Dcfs.Remove(f));
                    dc.Fields.ForEach(f => fractalDb.Entry(f).State = EntityState.Deleted);
                    dc.AConnectionDescriptions.ForEach(acd => acd.DcB.BConnectionDescriptions.Remove(acd));
                    dc.AConnectionDescriptions.ForEach(cd=>fractalDb.Cds.Remove(cd));
                    dc.AConnectionDescriptions.ForEach(cd=>fractalDb.Entry(cd).State = EntityState.Deleted);
                    dc.BConnectionDescriptions.ForEach(acd => acd.DcA.BConnectionDescriptions.Remove(acd));
                    dc.BConnectionDescriptions.ForEach(cd => fractalDb.Cds.Remove(cd));
                    dc.BConnectionDescriptions.ForEach(cd => fractalDb.Entry(cd).State = EntityState.Deleted);

                    fractalDb.Dcs.Remove(dc);
                    fractalDb.Entry(dc).State = EntityState.Deleted;
                }
                return dc;
            }, (fractalDb, auditDomainConcept, dc) =>
            {
                Audit audit = auditDomainConcept.Audit;
                foreach (ConnectionDescription aConnectionDescription in dc.AConnectionDescriptions)
                {
                    AuditDomainConcept auditADomainConcept = auditDomainConcept;
                    AuditDomainConcept auditBDomainConcept = CreateAuditDomainConcept(fractalDb, audit, aConnectionDescription.DcB);

                    AuditConnectionDescription auditConnectionDescription = CreateAuditConnectionDescription(fractalDb, audit, aConnectionDescription, auditADomainConcept, auditBDomainConcept);
                    auditConnectionDescription.Deleted = true;
                    auditConnectionDescription.AuditConnectionDescriptionParameters.ForEach(acdp=>acdp.Deleted = true);
                    fractalDb.ACds.Add(auditConnectionDescription);
                    fractalDb.ACdps.AddRange(auditConnectionDescription.AuditConnectionDescriptionParameters);
                    fractalDb.ADcs.Add(auditBDomainConcept);
                    fractalDb.ADcfs.AddRange(auditBDomainConcept.AuditDomainConceptFields);
                    
                }
                foreach (ConnectionDescription bConnectionDescription in dc.BConnectionDescriptions)
                {
                    AuditDomainConcept auditADomainConcept = CreateAuditDomainConcept(fractalDb, audit, bConnectionDescription.DcA);
                    AuditDomainConcept auditBDomainConcept = auditDomainConcept;
                    AuditConnectionDescription auditConnectionDescription = CreateAuditConnectionDescription(fractalDb, audit, bConnectionDescription, auditADomainConcept, auditBDomainConcept);
                    auditConnectionDescription.Deleted = true;
                    auditConnectionDescription.AuditConnectionDescriptionParameters.ForEach(acdp => acdp.Deleted = true);
                    fractalDb.ACds.Add(auditConnectionDescription);
                    fractalDb.ACdps.AddRange(auditConnectionDescription.AuditConnectionDescriptionParameters);
                    fractalDb.ADcs.Add(auditADomainConcept);
                    fractalDb.ADcfs.AddRange(auditADomainConcept.AuditDomainConceptFields);
                }

                auditDomainConcept.Deleted = true;
                auditDomainConcept.AuditDomainConceptFields.ForEach(adcf => adcf.Deleted = true);
                return true;
            });
        }

        private static void RemoveDomainConceptInstance(DomainConcept adc, DomainConceptInstance dci, FractalContext fractalDb)
        {
            WithAudit(fractalDb, adc.Name, (dc, fractaDb) =>
            {
                dci.Fields.ForEach(fv=> fractaDb.Dcifs.Remove(fv));
                dci.Fields.ForEach(fv=> fractaDb.Entry(fv).State = EntityState.Deleted );
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


        public static DomainConcept Dc(Func<DomainConcept, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                List<DomainConcept> domainConcepts = Dcs(func, fractalDb);
                if (domainConcepts.Count > 0)
                {
                    DomainConcept dc = domainConcepts.First();
                    // Force lazy load WTF? Order WTF?
                    dc.Fields.Sort(new ForderSorter());
                    return dc;
                }
                return null;
            }
        }

        public static DomainConcept Dc(Func<DomainConcept, bool> func, FractalContext fractalDb)
        {
            List<DomainConcept> domainConcepts = Dcs(func, fractalDb);
            return domainConcepts.Count > 0 ? domainConcepts.First() : null;
        }


        public static AuditDomainConcept Adc(Func<AuditDomainConcept, bool> func)
        {
            List<AuditDomainConcept> adcs = ADcs(func);
            return adcs.Count>0?adcs.First():null;
        }

        public static AuditDomainConcept Adc(Func<AuditDomainConcept, bool> func, FractalContext fractalDb)
        {
            List<AuditDomainConcept> adcs = ADcs(func, fractalDb);
            return adcs.Count > 0 ? adcs.First() : null;
        }


        public static List<AuditDomainConcept> ADcs(Func<AuditDomainConcept, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return ADcs(func, fractalDb);
            }
        }


        public static List<AuditDomainConcept> ADcs(Func<AuditDomainConcept, bool> func, FractalContext fractalDb)
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
                return fractalDb.ADcs.Where(adc => adc.DcId == dc.Id).OrderBy(adc=>adc.Aorder).ToList();
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
            AuditDomainConcept lastAdc = Adc(adc => adc.Child == null && adc.DcId == dc.Id, fractalDb);
            AuditDomainConcept auditDomainConcept = new AuditDomainConcept();
            auditDomainConcept.AuditDomainConceptId = NextId();
            auditDomainConcept.Audit = audit;
            auditDomainConcept.Parent = lastAdc;
            auditDomainConcept.DcId = dc.Id;
            auditDomainConcept.Name = dc.Name;
            auditDomainConcept.Aorder = lastAdc==null?0:lastAdc.Aorder + 1;

            foreach (DomainConceptField field in dc.Fields)
            {
                AuditDomainConceptField auditDomainConceptField = new AuditDomainConceptField();
                auditDomainConceptField.AuditDomainConceptFieldId = NextId();
                auditDomainConceptField.DcfId = field.Id;
                auditDomainConceptField.FieldName = field.FieldName;
                auditDomainConceptField.Forder = field.Forder;
                auditDomainConceptField.ADc = auditDomainConcept;
                auditDomainConcept.AuditDomainConceptFields.Add(auditDomainConceptField);
            }

            audit.AuditDomainConcepts.Add(auditDomainConcept);
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

        private static void WithAudit(Func<FractalContext, DomainConcept> func, Func<FractalContext, AuditDomainConcept,DomainConcept, bool> auditFunc)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                Audit audit = CreateAudit(fractalDb);
                DomainConcept dc = func(fractalDb);
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
            WithAudit((fractalDb) =>
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

        private static DomainConceptInstance WithAudit(FractalContext fractalDb, string domainConceptName, Func<DomainConcept, FractalContext, DomainConceptInstance> func, Func<AuditDomainConceptInstance, FractalContext, bool> auditFunc)
        {
            Audit audit = CreateAudit(fractalDb);
            DomainConcept dc = Dc(adc => adc.Name == domainConceptName, fractalDb);
            DomainConceptInstance dci = func(dc, fractalDb);

            if (dci != null)
            {
                AuditDomainConceptInstance auditDomainConceptInstance = CreateAuditDomainConceptInstance(fractalDb, audit, dci);
                auditFunc(auditDomainConceptInstance, fractalDb);
                fractalDb.Audits.Add(audit);
                fractalDb.ADcis.Add(auditDomainConceptInstance);
                fractalDb.ADcifs.AddRange(auditDomainConceptInstance.AuditDomainConceptInstanceFieldValues);
                fractalDb.SaveChanges();
            }
            return dci;
        }

        private static DomainConceptInstance WithAudit(string domainConceptName, Func<DomainConcept, FractalContext, DomainConceptInstance> func, Func<AuditDomainConceptInstance, FractalContext, bool> auditFunc)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return WithAudit(fractalDb, domainConceptName, func, auditFunc);
            }
        }

        public static DomainConceptInstance CreateDomainConceptInstance(string domainConceptName, params string[] fieldValues)
        {
            return 
                WithAudit(domainConceptName, (dc, fractalDb) =>
                {
                    DomainConceptInstance dci = new DomainConceptInstance();
                    dci.Id = NextId();
                    dci.DomainConceptName = dc.Name;
                    dci.DomainConceptId = dc.Id;

                    int i = 0;
                    foreach (DomainConceptField domainConceptField in dc.Fields.OrderBy(f=>f.Forder))
                    {
                        DomainConceptInstanceFieldValue dcif = new DomainConceptInstanceFieldValue();
                        dcif.Id = NextId();
                        dcif.Dci = dci;
                        dcif.DomainConceptName = dc.Name;
                        dcif.DomainConceptId = dc.Id;
                        dcif.DomainConceptFieldId = domainConceptField.Id;
                        dcif.DomainConceptFieldName = domainConceptField.FieldName;
                        dcif.Forder = domainConceptField.Forder;
                        dcif.FieldValue = fieldValues[i++];

                        dci.Fields.Add(dcif);
                    }
                    dci.Fields.Sort(new DciFieldSorter());

                    // Objects
                    fractalDb.Dcis.Add(dci);
                    fractalDb.Dcifs.AddRange(dci.Fields);

                    return dci;

                }, (auditDomainConceptInstance, fractalDbPassed) => true);
        }

        private static AuditDomainConceptInstance CreateAuditDomainConceptInstance(FractalContext fractalDb, Audit audit, DomainConceptInstance dci)
        {
            AuditDomainConceptInstance lastAdci = Adci(adci => adci.Child == null && adci.DciId == dci.Id, fractalDb);
            AuditDomainConceptInstance auditDomainConceptInstance = new AuditDomainConceptInstance();
            auditDomainConceptInstance.AuditDomainConceptInstanceId = NextId();
            auditDomainConceptInstance.Audit = audit;
            auditDomainConceptInstance.Parent = lastAdci;
            auditDomainConceptInstance.DciId = dci.Id;
            auditDomainConceptInstance.DomainConceptId = dci.DomainConceptId;
            auditDomainConceptInstance.DomainConceptName = dci.DomainConceptName;
            auditDomainConceptInstance.Aorder = lastAdci == null ? 0 : lastAdci.Aorder + 1;

            foreach (DomainConceptInstanceFieldValue field in dci.Fields)
            {
                AuditDomainConceptInstanceFieldValue auditDomainConceptInstanceFieldValue = new AuditDomainConceptInstanceFieldValue();
                auditDomainConceptInstanceFieldValue.AuditDomainConceptInstanceFieldValueId = NextId();

                auditDomainConceptInstanceFieldValue.DomainConceptInstanceFieldValueId = field.Id;
                auditDomainConceptInstanceFieldValue.ADci = auditDomainConceptInstance;
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
            List<DomainConceptInstance> dcs = Dcis(func);
            return dcs.Count > 0 ? dcs.First() : null;
        }

        public static DomainConceptInstance Dci(Func<DomainConceptInstance, bool> func, FractalContext fractalDb)
        {
            List<DomainConceptInstance> dcs = Dcis(func, fractalDb);
            return dcs.Count > 0 ? dcs.First() : null;
        }


        public static List<DomainConceptInstance> Dcis(Func<DomainConceptInstance, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return Dcis(func, fractalDb);
            }
        }

        public static List<DomainConceptInstance> Dcis(Func<DomainConceptInstance, bool> func, FractalContext fractalDb)
        {
            return fractalDb.Dcis.Where(func).ToList();
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
            return Dcis(dci => dci.DomainConceptName.Equals(domainConceptName) &&
                               dci.Fields.Find(where.Check) != null);
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

        public static ConnectionDescription CreateConnectionDescription(DomainConcept dcA, DomainConcept dcB, string connectionName, Cardinality cardinality, bool directed, bool required, bool reciprical)
        {
            return WithAudit((fractalDb) =>
            {
                ConnectionDescription connectionDescription = new ConnectionDescription();
                connectionDescription.Id = NextId();
                connectionDescription.DcA = dcA;
                connectionDescription.DcB = dcB;
                connectionDescription.ConnectionName = connectionName;
                connectionDescription.Cardinality = cardinality;
                connectionDescription.Directed = directed;
                connectionDescription.Required = required;
                connectionDescription.Reciprical = reciprical;

                fractalDb.Dcs.Attach(dcA);
                fractalDb.Dcs.Attach(dcB);
                dcA.AConnectionDescriptions.Add(connectionDescription);
                dcB.BConnectionDescriptions.Add(connectionDescription);
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
                AuditDomainConcept auditADomainConcept = CreateAuditDomainConcept(fractalDb, audit, connectionDescription.DcA);
                AuditDomainConcept auditBDomainConcept = CreateAuditDomainConcept(fractalDb, audit, connectionDescription.DcB);
                AuditConnectionDescription auditConnectionDescription = CreateAuditConnectionDescription(fractalDb, audit, connectionDescription, auditADomainConcept, auditBDomainConcept);

                auditFunc(auditConnectionDescription, fractalDb);

                fractalDb.Audits.Add(audit);
                fractalDb.ACds.Add(auditConnectionDescription);
                fractalDb.ACdps.AddRange(auditConnectionDescription.AuditConnectionDescriptionParameters);
                fractalDb.ADcs.Add(auditADomainConcept);
                fractalDb.ADcfs.AddRange(auditADomainConcept.AuditDomainConceptFields);
                fractalDb.ADcs.Add(auditBDomainConcept);
                fractalDb.ADcfs.AddRange(auditBDomainConcept.AuditDomainConceptFields);
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
            AuditConnectionDescription lastAcd = ACd(acd => acd.Child == null && acd.CdId == connectionDescription.Id, fractalDb);
            AuditConnectionDescription auditConnectionDescription = new AuditConnectionDescription();
            auditConnectionDescription.AuditConnectionDescriptionId = NextId();
            auditConnectionDescription.Audit = audit;
            auditConnectionDescription.Parent = lastAcd;
            auditConnectionDescription.Aorder = lastAcd == null ? 0 : lastAcd.Aorder + 1;

            auditConnectionDescription.CdId = connectionDescription.Id;
            auditConnectionDescription.DcA = auditADomainConcept;
            auditConnectionDescription.DcB = auditBDomainConcept;
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


        public static AuditConnectionDescription ACd(Func<AuditConnectionDescription, bool> func)
        {
            List<AuditConnectionDescription> adcs = ACds(func);
            return adcs.Count > 0 ? adcs.First() : null;
        }

        public static AuditConnectionDescription ACd(Func<AuditConnectionDescription, bool> func, FractalContext fractalDb)
        {
            List<AuditConnectionDescription> adcis = ACds(func, fractalDb);
            return adcis.Count > 0 ? adcis.First() : null;
        }


        public static List<AuditConnectionDescription> ACds(Func<AuditConnectionDescription, bool> func)
        {
            using (FractalContext fractalDb = new FractalContext())
            {
                return ACds(func, fractalDb);
            }
        }

        public static List<AuditConnectionDescription> ACds(Func<AuditConnectionDescription, bool> func, FractalContext fractalDb)
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