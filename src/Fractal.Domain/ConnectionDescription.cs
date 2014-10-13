using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fractal.Domain
{
    public class ConnectionDescription
    {
        [Key]
        public string Id { get; set; }
        public DomainConcept DcA { get; set; }
        public DomainConcept DcB { get; set; }
        public string ConnectionName { get; set; }
        public bool Reciprical { get; set; }
        public Cardinality Cardinality { get; set; }
        public bool Directed { get; set; }
        public bool Required { get; set; }
        public List<ConnectionDescriptionParameter> ConnectionDescriptionParameters { get; set; }

        public ConnectionDescription()
        {
            ConnectionDescriptionParameters = new List<ConnectionDescriptionParameter>();
        }
    }
}