using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fractal.Domain
{
    public class AuditDomainConcept
    {
        [Key]
        public string AuditDomainConceptId { get; set; }
        public Audit Audit { get; set; }
        public AuditDomainConcept Parent { get; set; }
        public AuditDomainConcept Child { get; set; }
        public int Aorder { get; set; }
        public bool Deleted { get; set; }

        public string DcId { get; set; }
        public string Name { get; set; }
        public virtual List<AuditDomainConceptField> AuditDomainConceptFields { get; set; }
        public virtual List<AuditConnectionDescription> AuditAConnectionDescriptions { get; set; }
        public virtual List<AuditConnectionDescription> AuditBConnectionDescriptions { get; set; }

        public AuditDomainConcept()
        {
            AuditDomainConceptFields = new List<AuditDomainConceptField>();
            AuditAConnectionDescriptions = new List<AuditConnectionDescription>();
            AuditBConnectionDescriptions = new List<AuditConnectionDescription>();
        }

        public bool HasChanged(DomainConcept dc)
        {
            return Name != dc.Name;
        }
    }
}