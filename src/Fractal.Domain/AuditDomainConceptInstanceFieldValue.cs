using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fractal.Domain
{
    public class AuditDomainConceptInstanceFieldValue
    {
        [Key]
        public string AuditDomainConceptInstanceFieldValueId { get; set; }
        public bool Deleted { get; set; }

        public string DomainConceptInstanceFieldValueId { get; set; }
        public string ADciId  { get; set; }
        public virtual AuditDomainConceptInstance ADci { get; set; }
        public string DomainConceptId { get; set; }
        public string DomainConceptName { get; set; }
        public string DomainConceptFieldId { get; set; }
        public string DomainConceptFieldName { get; set; }
        public int Forder { get; set; }

        public string FieldValue { get; set; }

        [NotMapped]
        public AuditDomainConceptInstance AuditDomainConceptInstance
        {
            get { return ADci ?? (ADci = FractalDb.Adci(adc => adc.AuditDomainConceptInstanceId == ADciId)); }
            set
            {
                ADci = value;
                ADciId = value.AuditDomainConceptInstanceId;
            }
        }


        public bool HasChanged(DomainConceptInstanceFieldValue field)
        {
            return FieldValue.Equals(field.FieldValue);
        }
    }
}