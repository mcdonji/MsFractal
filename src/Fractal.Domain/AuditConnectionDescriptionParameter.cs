using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fractal.Domain
{
    public class AuditConnectionDescriptionParameter
    {
        [Key]
        public string AuditConnectionDescriptionParameterId { get; set; }
        public bool Deleted { get; set; }

        public string ACdId { get; set; }
        public AuditConnectionDescription ACd { get; set; }
        public string ConnectionDescriptionParameterId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string FunctionCode { get; set; }

        [NotMapped]
        public AuditConnectionDescription AuditConnectionDescription
        {
            get { return ACd ?? (ACd = FractalDb.AuditConnectionDescription(acd => acd.AuditConnectionDescriptionId == ACdId)); }
            set
            {
                ACd = value;
                ACdId = value.AuditConnectionDescriptionId;
            }
        }

    }
}