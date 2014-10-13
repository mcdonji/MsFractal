using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fractal.Domain
{
    public class Audit
    {
        public string Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Audit Parent { get; set; }
        public Audit Child{ get; set; }
        public int Aorder { get; set; }
        public virtual List<AuditDomainConcept> AuditDomainConcepts { get; set; }
        public virtual List<AuditDomainConceptInstance> AuditDomainConceptInstances { get; set; }
        public virtual List<AuditConnectionDescription> AuditConnectionDescriptions { get; set; }

        public Audit()
        {
            AuditDomainConcepts = new List<AuditDomainConcept>();
            AuditDomainConceptInstances = new List<AuditDomainConceptInstance>();
            AuditConnectionDescriptions = new List<AuditConnectionDescription>();
        }
    }
}