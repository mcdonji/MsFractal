using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fractal.Domain
{
    public class AuditConnectionDescription
    {
        [Key]
        public string AuditConnectionDescriptionId { get; set; }
        public string AudId { get; set; }
        public Audit Aud { get; set; }
        public string ParId { get; set; }
        public AuditConnectionDescription Par { get; set; }
        public string ChiId { get; set; }
        public AuditConnectionDescription Chi { get; set; }
        public int Aorder { get; set; }
        public bool Deleted { get; set; }

        public string CdId { get; set; }
        public string LeftDcId { get; set; }
        public AuditDomainConcept LeftDc { get; set; }
        public string RightDcId { get; set; }
        public AuditDomainConcept RightDc { get; set; }
        public string ConnectionName { get; set; }
        public bool Reciprical { get; set; }
        public Cardinality Cardinality { get; set; }
        public bool Directed { get; set; }
        public bool Required { get; set; }
        public List<AuditConnectionDescriptionParameter> ACdps { get; set; }

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
        public AuditConnectionDescription Parent
        {
            get { return Par ?? (Par = FractalDb.AuditConnectionDescription(acd => acd.AuditConnectionDescriptionId == ParId)); }
            set
            {
                Par = value;
                ParId = value == null ? null : value.AuditConnectionDescriptionId;
            }
        }

        [NotMapped]
        public AuditConnectionDescription Child
        {
            get { return Chi ?? (Chi = FractalDb.AuditConnectionDescription(acd => acd.AuditConnectionDescriptionId == ChiId)); }
            set
            {
                Chi = value;
                ChiId = value == null ? null : value.AuditConnectionDescriptionId;
            }
        }

        [NotMapped]
        public AuditDomainConcept LeftDomainConcept
        {
            get { return LeftDc ?? (LeftDc = FractalDb.AuditDomainConcept(adc => adc.AuditDomainConceptId == LeftDcId)); }
            set
            {
                LeftDc = value;
                LeftDcId = value.AuditDomainConceptId;
            }
        }

        [NotMapped]
        public AuditDomainConcept RightDomainConcept
        {
            get { return RightDc ?? (RightDc = FractalDb.AuditDomainConcept(adc => adc.AuditDomainConceptId == RightDcId)); }
            set
            {
                RightDc = value;
                RightDcId = value.AuditDomainConceptId;
            }
        }

        [NotMapped]
        public List<AuditConnectionDescriptionParameter> AuditConnectionDescriptions
        {
            get { return ACdps ?? (ACdps = FractalDb.AuditConnectionDescriptionParameters(acdp => acdp.ACdId == AuditConnectionDescriptionId)); }
            set { ACdps = value; }
        }

    }
}