namespace Fractal.Domain
{
    public class DomainConceptField
    {
        public string Id { get; set; }
        public virtual DomainConcept Dc { get; set; }
        public int Forder { get; set; }
        public string FieldName { get; set; }
        public string DefaultValue { get; set; }         
    }
}