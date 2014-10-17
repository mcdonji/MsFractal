using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fractal.Domain
{
    public class Audit
    {
        public string Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ParId { get; set; }
        public Audit Par { get; set; }
        public string ChiId { get; set; }
        public Audit Chi { get; set; }
        public int Aorder { get; set; }
        public virtual List<AuditDomainConcept> ADcs { get; set; }
        public virtual List<AuditDomainConceptInstance> ADcis { get; set; }
        public virtual List<AuditConnectionDescription> ACds { get; set; }
        public virtual List<AuditConnection> ACons { get; set; }

        [NotMapped]
        public Audit Parent
        {
            get { return Par ?? (Par = FractalDb.Audit(a => a.Id == ParId)); }
            set
            {
                Par = value;
                ParId = value == null ? null : value.Id;
            }
        }

        [NotMapped]
        public Audit Child
        {
            get { return Chi ?? (Chi = FractalDb.Audit(a => a.Id == ChiId)); }
            set
            {
                Chi = value;
                ChiId = value == null ? null : value.Id;
            }
        }

        [NotMapped]
        public List<AuditDomainConcept> AuditDomainConcepts
        {
            get { return ADcs ?? (ADcs = FractalDb.AuditDomainConcepts(adc => adc.AudId == Id)); }
            set { ADcs = value; }
        }

        [NotMapped]
        public List<AuditDomainConceptInstance> AuditDomainConceptInstances
        {
            get { return ADcis ?? (ADcis = FractalDb.ADcis(adci => adci.AudId == Id)); }
            set { ADcis = value; }
        }

        [NotMapped]
        public List<AuditConnectionDescription> AuditConnectionDescriptions
        {
            get { return ACds ?? (ACds = FractalDb.AuditConnectionDescriptions(acd => acd.AudId == Id)); }
            set { ACds = value; }
        }

        public void AddAuditDomainConcept(AuditDomainConcept auditDomainConcept)
        {
            if (ADcs == null)
            {
                ADcs = new List<AuditDomainConcept>() { auditDomainConcept };
            }
            else
            {
                ADcs.Add(auditDomainConcept);
            }
        }

        public void AddAuditConnection(AuditConnection auditConnection)
        {
            if (ACons == null)
            {
                ACons = new List<AuditConnection>() { auditConnection };
            }
            else
            {
                ACons.Add(auditConnection);
            }
        }
    }
}