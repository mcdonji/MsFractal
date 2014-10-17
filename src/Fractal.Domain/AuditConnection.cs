using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fractal.Domain
{
    public class AuditConnection
    {
        [Key]
        public string AuditConnectionId { get; set; }
        public string AudId { get; set; }
        public Audit Aud { get; set; }
        public string ParId { get; set; }
        public AuditConnection Par { get; set; }
        public string ChiId { get; set; }
        public AuditConnection Chi { get; set; }
        public int Aorder { get; set; }
        public bool Deleted { get; set; }

        public string ConId { get; set; }
        public string CdId { get; set; }
        public string LeftDciId { get; set; }
        public AuditDomainConceptInstance LeftDci { get; set; }
        public string RightDciId { get; set; }
        public AuditDomainConceptInstance RightDci { get; set; }
        
        [NotMapped]
        public Audit Audit
        {
            get { return Aud ?? (Aud = FractalDb.Audit(a => a.Id == AudId)); }
            set
            {
                Aud = value;
                AudId = value.Id;
            }
        }

        [NotMapped]
        public AuditConnection Parent
        {
            get { return Par ?? (Par = FractalDb.AuditConnection(acd => acd.AuditConnectionId == ParId)); }
            set
            {
                Par = value;
                ParId = value == null ? null : value.AuditConnectionId;
            }
        }

        [NotMapped]
        public AuditConnection Child
        {
            get { return Chi ?? (Chi = FractalDb.AuditConnection(acd => acd.AuditConnectionId == ChiId)); }
            set
            {
                Chi = value;
                ChiId = value == null ? null : value.AuditConnectionId;
            }
        }

        [NotMapped]
        public AuditDomainConceptInstance LeftDomainConceptInstance
        {
            get { return LeftDci ?? (LeftDci = FractalDb.Adci(adc => adc.AuditDomainConceptInstanceId == LeftDciId)); }
            set
            {
                LeftDci = value;
                LeftDciId = value.AuditDomainConceptInstanceId;
            }
        }

        [NotMapped]
        public AuditDomainConceptInstance RightDomainConceptInstance
        {
            get { return RightDci ?? (RightDci = FractalDb.Adci(adc => adc.AuditDomainConceptInstanceId == RightDciId)); }
            set
            {
                RightDci = value;
                RightDciId = value.AuditDomainConceptInstanceId;
            }
        }



    }
}