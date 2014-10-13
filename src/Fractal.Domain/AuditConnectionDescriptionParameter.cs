using System.ComponentModel.DataAnnotations;

namespace Fractal.Domain
{
    public class AuditConnectionDescriptionParameter
    {
        [Key]
        public string AuditConnectionDescriptionParameterId { get; set; }
        public bool Deleted { get; set; }

        public string ConnectionDescriptionParameterId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string FunctionCode { get; set; }
             
    }
}