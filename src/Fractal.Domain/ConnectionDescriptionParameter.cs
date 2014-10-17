using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fractal.Domain
{
    public class ConnectionDescriptionParameter
    {
        [Key]
        public string Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string FunctionCode { get; set; }
        public string CdId { get; set; }
        public ConnectionDescription Cd { get; set; }

        [NotMapped]
        public ConnectionDescription ConnectionDescription
        {
            get { return Cd ?? (Cd = FractalDb.Cd(cd => cd.Id == CdId)); }
            set
            {
                Cd = value;
                CdId = value.Id;
            }
        }

    }
}