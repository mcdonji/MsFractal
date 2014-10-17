using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fractal.Domain
{
    public class DomainConceptInstance
    {
        public string Id { get; set; }
        public string DomainConceptId { get; set; }
        public string DomainConceptName { get; set; }
        public List<DomainConceptInstanceFieldValue> Dcifs { get; set; }
        public List<Connection> LeftCons{ get; set; }
        public List<Connection> RightCons { get; set; }

        [NotMapped]
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

        [NotMapped]
        public List<DomainConceptInstanceFieldValue> Fields
        {
            get { return Dcifs ?? (Dcifs = FractalDb.Fields(this)); }
            set { Dcifs = value; }
        }

        public List<DomainConceptInstanceFieldValue> GetFields(FractalContext fractalDb)
        {
            return Dcifs ?? (Dcifs = FractalDb.Fields(this, fractalDb));
        }


        [NotMapped]
        public List<Connection> LeftConnections
        {
            get { return LeftCons ?? (LeftCons = FractalDb.Connections(con => con.RightDciId == Id)); }
            set { LeftCons = value; }
        }


        [NotMapped]
        public List<Connection> RightConnections
        {
            get { return RightCons ?? (RightCons = FractalDb.Connections(con => con.LeftDciId == Id)); }
            set { RightCons = value; }
        }


        public List<DomainConceptInstance> AllLeftConnections()
        {
            return LeftConnections.ConvertAll(con => con.LeftDomainConceptInstance);
        }
        public List<DomainConceptInstance> AllRightConnections()
        {
            return RightConnections.ConvertAll(con => con.RightDomainConceptInstance);
        }

    }
}