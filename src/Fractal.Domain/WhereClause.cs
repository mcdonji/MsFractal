using System.Collections.Generic;

namespace Fractal.Domain
{
    public class WhereClause
    {
        public List<AndClause> Ands { get; set; }

        public WhereClause()
        {
            Ands = new List<AndClause>();
        }

        public bool Check(DomainConceptInstanceFieldValue fv)
        {
            bool result = true;
            foreach (AndClause andClause in Ands)
            {
                bool andResult = fv.DomainConceptFieldName.Equals(andClause.Column) && fv.FieldValue.Equals(andClause.Value);
                result = result && andResult;
            }
            return result;
        }
    }
}