using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fractal.Domain
{
    public class ConnectionDescription
    {
        [Key]
        public string Id { get; set; }
        public string LeftDcId { get; set; }
        public DomainConcept LeftDc { get; set; }
        public string RightDcId { get; set; }
        public DomainConcept RightDc { get; set; }
        public string ConnectionName { get; set; }
        public bool Reciprical { get; set; }
        public Cardinality Cardinality { get; set; }
        public bool Directed { get; set; }
        public bool Required { get; set; }
        public List<ConnectionDescriptionParameter> Cdps { get; set; }

        [NotMapped]
        public DomainConcept LeftDomainConcept
        {
            get { return LeftDc ?? (LeftDc = FractalDb.Dc(dc => dc.Id == LeftDcId)); }
            set
            {
                LeftDc = value;
                LeftDcId = value.Id;
            }
        }

        public DomainConcept GetDcLeft(FractalContext fractalDb)
        {
            return LeftDc ?? (LeftDc = FractalDb.Dc(dc => dc.Id == LeftDcId, fractalDb));
        }


        [NotMapped]
        public DomainConcept RightDomainConcept
        {
            get { return RightDc ?? (RightDc = FractalDb.Dc(dc => dc.Id == RightDcId)); }
            set
            {
                RightDc = value;
                RightDcId = value.Id;
            }
        }

        public DomainConcept GetDcRight(FractalContext fractalDb)
        {
            return RightDc ?? (RightDc = FractalDb.Dc(dc => dc.Id == RightDcId, fractalDb));
        }


        [NotMapped]
        public List<ConnectionDescriptionParameter> ConnectionDescriptionParameters
        {
            get { return Cdps ?? (Cdps = FractalDb.ConnectionDescriptionParameters(cdp => cdp.CdId == Id)); }
            set { Cdps = value; }
        }


    }
}