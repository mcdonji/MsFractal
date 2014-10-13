using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fractal.Domain
{
    public class AuditDomainConceptInstance
    {
        [Key]
        public string AuditDomainConceptInstanceId { get; set; }
        public Audit Audit { get; set; }
        public AuditDomainConceptInstance Parent { get; set; }
        public AuditDomainConceptInstance Child { get; set; }
        public int Aorder { get; set; }
        public bool Deleted { get; set; }

        public string DciId { get; set; }
        public string DomainConceptId { get; set; }
        public string DomainConceptName { get; set; }
        public virtual List<AuditDomainConceptInstanceFieldValue> AuditDomainConceptInstanceFieldValues { get; set; }

        public AuditDomainConceptInstance()
        {
            AuditDomainConceptInstanceFieldValues = new List<AuditDomainConceptInstanceFieldValue>();
        }
    }
}