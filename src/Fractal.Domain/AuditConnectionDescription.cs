using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fractal.Domain
{
    public class AuditConnectionDescription
    {
        [Key]
        public string AuditConnectionDescriptionId { get; set; }
        public Audit Audit { get; set; }
        public AuditConnectionDescription Parent { get; set; }
        public AuditConnectionDescription Child { get; set; }
        public int Aorder { get; set; }
        public bool Deleted { get; set; }

        public string CdId { get; set; }
        public AuditDomainConcept DcA { get; set; }
        public AuditDomainConcept DcB { get; set; }
        public string ConnectionName { get; set; }
        public bool Reciprical { get; set; }
        public Cardinality Cardinality { get; set; }
        public bool Directed { get; set; }
        public bool Required { get; set; }
        public virtual List<AuditConnectionDescriptionParameter> AuditConnectionDescriptionParameters { get; set; }

        public AuditConnectionDescription()
        {
            AuditConnectionDescriptionParameters = new List<AuditConnectionDescriptionParameter>();
        }
    }
}