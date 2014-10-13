using System.Collections.Generic;

namespace Fractal.Domain
{
    public class DomainConceptInstance
    {
        public string Id { get; set; }
        public string DomainConceptId { get; set; }
        public string DomainConceptName { get; set; }
        public virtual List<DomainConceptInstanceFieldValue> Fields { get; set; }

        public DomainConceptInstance()
        {
            Fields = new List<DomainConceptInstanceFieldValue>();
        }

        public string this[string field]
        {
            get { return Get(field); }
        }

        private string Get(string field)
        {
            return getFieldValue(field).FieldValue;
        }

        public void Set(string fieldName, string value)
        {
            getFieldValue(fieldName).FieldValue = value;
        }

        private DomainConceptInstanceFieldValue getFieldValue(string field)
        {
            return Fields.Find(f => f.DomainConceptFieldName.Equals(field));
        }


        
    }
}