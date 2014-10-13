using System.ComponentModel.DataAnnotations;

namespace Fractal.Domain
{
    public class ConnectionDescriptionParameter
    {
        [Key]
        public string Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string FunctionCode { get; set; }
    }
}