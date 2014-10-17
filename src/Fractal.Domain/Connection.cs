using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fractal.Domain
{
    public class Connection
    {
        [Key]
        public string Id { get; set; }
        public string CdId { get; set; }
        public ConnectionDescription Cd { get; set; }
        public string LeftDciId { get; set; }
        public DomainConceptInstance LeftDci { get; set; }
        public string RightDciId { get; set; }
        public DomainConceptInstance RightDci { get; set; }

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

        [NotMapped]
        public DomainConceptInstance LeftDomainConceptInstance
        {
            get { return LeftDci ?? (LeftDci = FractalDb.Dci(dci => dci.Id == LeftDciId)); }
            set
            {
                LeftDci = value;
                LeftDciId = value.Id;
            }
        }
 
        [NotMapped]
        public DomainConceptInstance RightDomainConceptInstance
        {
            get { return RightDci ?? (RightDci = FractalDb.Dci(dci => dci.Id == RightDciId)); }
            set
            {
                RightDci = value;
                RightDciId = value.Id;
            }
        }
 

    }
}