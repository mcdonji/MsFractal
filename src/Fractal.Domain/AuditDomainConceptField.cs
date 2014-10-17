using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fractal.Domain
{
    public class AuditDomainConceptField
    {
        [Key]
        public string AuditDomainConceptFieldId { get; set; }
        public bool Deleted { get; set; }

        public string DcfId { get; set; }
        public string ADcId { get; set; }
        public AuditDomainConcept Adc { get; set; }
        public int Forder { get; set; }
        public string FieldName { get; set; }
        public string DefaultValue { get; set; }

        [NotMapped]
        public AuditDomainConcept AuditDomainConcept
        {
            get { return Adc ?? (Adc = FractalDb.AuditDomainConcept(adc => adc.AuditDomainConceptId == ADcId)); }
            set
            {
                Adc = value;
                ADcId = value.AuditDomainConceptId;
            }
        }


        public bool HasChanged(DomainConceptField field)
        {
            return FieldName != field.FieldName || Forder != field.Forder || DefaultValue != field.DefaultValue;
        }
    }
}