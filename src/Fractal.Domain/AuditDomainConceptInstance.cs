using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fractal.Domain
{
    public class AuditDomainConceptInstance
    {
        [Key]
        public string AuditDomainConceptInstanceId { get; set; }
        public string AudId { get; set; }
        public virtual Audit Aud { get; set; }
        public string ParId { get; set; }
        public virtual AuditDomainConceptInstance Par { get; set; }
        public string ChiId { get; set; }
        public virtual AuditDomainConceptInstance Chi { get; set; }
        public int Aorder { get; set; }
        public bool Deleted { get; set; }

        public string DomainConceptInstanceId { get; set; }
        public string DomainConceptId { get; set; }
        public string DomainConceptName { get; set; }
        public virtual List<AuditDomainConceptInstanceFieldValue> ADcfvs { get; set; }

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
        public AuditDomainConceptInstance Parent
        {
            get { return Par ?? (Par = FractalDb.Adci(adc => adc.AuditDomainConceptInstanceId == ParId)); }
            set
            {
                Par = value;
                ParId = value == null ? null : value.AuditDomainConceptInstanceId;
            }
        }

        [NotMapped]
        public AuditDomainConceptInstance Child
        {
            get { return Chi ?? (Chi = FractalDb.Adci(adc => adc.AuditDomainConceptInstanceId == ChiId)); }
            set
            {
                Chi = value;
                ChiId = value == null ? null : value.AuditDomainConceptInstanceId;
            }
        }

        [NotMapped]
        public List<AuditDomainConceptInstanceFieldValue> AuditDomainConceptInstanceFieldValues
        {
            get { return ADcfvs ?? (ADcfvs = FractalDb.AuditDomainConceptInstanceFieldValues(adcifv => adcifv.ADciId == AuditDomainConceptInstanceId)); }
            set { ADcfvs = value; }
        }



    }
}