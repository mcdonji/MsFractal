using System.ComponentModel.DataAnnotations.Schema;

namespace Fractal.Domain
{
    public class DomainConceptField
    {
        public string Id { get; set; }
        public string DcId { get; set; }
        public DomainConcept Dc { get; set; }
        public int Forder { get; set; }
        public string FieldName { get; set; }
        public string DefaultValue { get; set; }

        [NotMapped]
        public DomainConcept DomainConcept
        {
            get { return Dc ?? (Dc = FractalDb.Dc(a => a.Id == DcId)); }
            set
            {
                Dc = value;
                DcId = value.Id;
            }
        }
    }
}