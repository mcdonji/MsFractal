using System.ComponentModel.DataAnnotations.Schema;

namespace Fractal.Domain
{
    public class DomainConceptInstanceFieldValue
    {
        public string Id { get; set; }
        public string DciId { get; set; }
        public DomainConceptInstance Dci { get;set;}
        public string DomainConceptId { get; set; }
        public string DomainConceptName { get; set; }
        public string DomainConceptFieldId { get; set; }
        public string DomainConceptFieldName { get; set; }
        public int Forder { get; set; }

        public string FieldValue { get; set; }

        [NotMapped]
        public DomainConceptInstance DomainConceptInstance
        {
            get { return Dci ?? (Dci = FractalDb.Dci(a => a.Id == DciId)); }
            set
            {
                Dci = value;
                DciId = value.Id;
            }
        }

    }
}