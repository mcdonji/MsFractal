using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fractal.Domain
{
    public class DomainConcept
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual List<DomainConceptField> Fields { get; set; }
        public virtual List<ConnectionDescription> AConnectionDescriptions { get; set; }
        public virtual List<ConnectionDescription> BConnectionDescriptions { get; set; }        

        public DomainConcept()
        {
            Fields = new List<DomainConceptField>();
            AConnectionDescriptions = new List<ConnectionDescription>();
            BConnectionDescriptions = new List<ConnectionDescription>();
        }
    }
}
