using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Fractal.Domain
{
    public class DomainConcept
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<DomainConceptField> Dcfs { get; set; }
        public List<ConnectionDescription> LeftCds { get; set; }
        public List<ConnectionDescription> RightCds { get; set; }


        [NotMapped]
        public List<DomainConceptField> Fields 
        {
            get { return Dcfs ?? (Dcfs = FractalDb.Fields(this)); }
            set { Dcfs = value; }
        }

        [NotMapped]
        public List<ConnectionDescription> LeftConnectionDescriptions
        {
            get { return LeftCds ?? (LeftCds = FractalDb.Cds(cd => cd.RightDomainConcept.Id == Id)); }
            set { LeftCds = value; }
        }

        [NotMapped]
        public List<ConnectionDescription> RightConnectionDescriptions
        {
            get { return RightCds ?? (RightCds = FractalDb.Cds(cd => cd.LeftDomainConcept.Id == Id)); }
            set { RightCds = value; }
        }

    }
}
