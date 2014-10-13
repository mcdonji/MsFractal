using System.ComponentModel.DataAnnotations;

namespace Fractal.Domain
{
    public class AuditDomainConceptField
    {
        [Key]
        public string AuditDomainConceptFieldId { get; set; }
        public bool Deleted { get; set; }

        public string DcfId { get; set; }
        public virtual AuditDomainConcept ADc { get; set; }
        public int Forder { get; set; }
        public string FieldName { get; set; }
        public string DefaultValue { get; set; }

        public bool HasChanged(DomainConceptField field)
        {
            return FieldName != field.FieldName || Forder != field.Forder || DefaultValue != field.DefaultValue;
        }
    }
}