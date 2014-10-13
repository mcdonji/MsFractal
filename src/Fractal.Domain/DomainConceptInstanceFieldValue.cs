namespace Fractal.Domain
{
    public class DomainConceptInstanceFieldValue
    {
        public string Id { get; set; }
        public virtual DomainConceptInstance Dci { get;set;}
        public string DomainConceptId { get; set; }
        public string DomainConceptName { get; set; }
        public string DomainConceptFieldId { get; set; }
        public string DomainConceptFieldName { get; set; }
        public int Forder { get; set; }

        public string FieldValue { get; set; }
    }
}