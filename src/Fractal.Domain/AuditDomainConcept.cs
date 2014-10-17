using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fractal.Domain
{
    public class AuditDomainConcept
    {
        [Key]
        public string AuditDomainConceptId { get; set; }
        public string AudId { get; set; }
        public Audit Aud { get; set; }
        public string ParId { get; set; }
        public AuditDomainConcept Par { get; set; }
        public string ChiId { get; set; }
        public AuditDomainConcept Chi { get; set; }
        public int Aorder { get; set; }
        public bool Deleted { get; set; }

        public string DomainConceptId { get; set; }
        public string Name { get; set; }
        public List<AuditDomainConceptField> ADcfs { get; set; }
        public List<AuditConnectionDescription> ALeftCds { get; set; }
        public List<AuditConnectionDescription> ARightCds { get; set; }


        [NotMapped]
        public Audit Audit
        {
            get
            {
                return Aud ?? (Aud = FractalDb.Audit(a => a.Id == AudId));
            }
            set
            {
                Aud = value;
                AudId = value.Id;
            }
        }

        [NotMapped]
        public AuditDomainConcept Parent
        {
            get { return Par ?? (Par = FractalDb.AuditDomainConcept(adc => adc.AuditDomainConceptId == ParId)); }
            set
            {
                Par = value;
                ParId = value == null ? null : value.AuditDomainConceptId;
            }
        }

        [NotMapped]
        public AuditDomainConcept Child
        {
            get { return Chi ?? (Chi = FractalDb.AuditDomainConcept(adc => adc.AuditDomainConceptId == ChiId)); }
            set
            {
                Chi = value;
                ChiId = value == null ? null : value.AuditDomainConceptId;
            }
        }

        [NotMapped]
        public List<AuditDomainConceptField> AuditDomainConceptFields
        {
            get { return ADcfs ?? (ADcfs = FractalDb.AuditDomainConceptFields(adcf => adcf.ADcId == AuditDomainConceptId)); }
            set { ADcfs = value; }
        }


        [NotMapped]
        public List<AuditConnectionDescription> AuditLeftConnectionDescriptions
        {
            get { return ALeftCds ?? (ALeftCds = FractalDb.AuditConnectionDescriptions(acd => acd.LeftDcId == AuditDomainConceptId)); }
            set { ALeftCds = value; }
        }



        [NotMapped]
        public List<AuditConnectionDescription> AuditRightConnectionDescriptions
        {
            get { return ARightCds ?? (ARightCds = FractalDb.AuditConnectionDescriptions(acd => acd.RightDcId == AuditDomainConceptId)); }
            set { ARightCds = value; }
        }




        public bool HasChanged(DomainConcept dc)
        {
            return Name != dc.Name;
        }

        public void AddAuditDomainConceptField(AuditDomainConceptField auditDomainConceptField)
        {
            if (ADcfs == null)
            {
                ADcfs = new List<AuditDomainConceptField>() { auditDomainConceptField };
            }
            else
            {
                ADcfs.Add(auditDomainConceptField);
            }
        }
    }
}