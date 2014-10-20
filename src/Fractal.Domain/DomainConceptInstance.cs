using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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


        public List<DomainConceptInstance> AllLeftDcis()
        {
            return LeftConnections.ConvertAll(con => con.LeftDomainConceptInstance);
        }
        public List<DomainConceptInstance> AllRightDcis()
        {
            return RightConnections.ConvertAll(con => con.RightDomainConceptInstance);
        }

        public List<Connection> GetRightConnections(string connectionName)
        {
            return RightConnections.FindAll(con => con.ConnectionDescription.ConnectionName == connectionName);
        } 

        public List<DomainConceptInstance> GetRightDcis(string connectionName)
        {
            List<DomainConceptInstance> domainConceptInstances = RightConnections.FindAll(con=>con.ConnectionDescription.ConnectionName == connectionName).ConvertAll(con => con.RightDomainConceptInstance);
            if (domainConceptInstances.Count > 0)
            {
                if (domainConceptInstances.First().IsOrdered())
                {
                    domainConceptInstances.Sort(new OrderedDciSort());
                }
            }
            return domainConceptInstances;
        }

        private bool IsOrdered()
        {
            return Fields.Any(f => f.DomainConceptFieldName == "Order");
        }

        public List<DomainConceptInstance> GetLeftDcis(string connectionName)
        {
            List<DomainConceptInstance> domainConceptInstances = LeftConnections.FindAll(con=>con.ConnectionDescription.ConnectionName == connectionName).ConvertAll(con => con.LeftDomainConceptInstance);
            if (domainConceptInstances.Count > 0)
            {
                if (domainConceptInstances.First().IsOrdered())
                {
                    domainConceptInstances.Sort(new OrderedDciSort());
                }
            }
            return domainConceptInstances;
        }

        public bool IsFunctionable()
        {
            return Fields.Any(fv=>fv.DomainConceptFieldName == FF.FFUNC);
        }

        public DomainConceptInstance FirstRight(string connectionName)
        {
            return GetRightDcis(connectionName).First();
        }
        public DomainConceptInstance FirstLeft(string connectionName)
        {
            return GetLeftDcis(connectionName).First();
        }
    }

    public class OrderedDciSort : IComparer<DomainConceptInstance>
    {
        public int Compare(DomainConceptInstance x, DomainConceptInstance y)
        {
            return Int32.Parse(x["Order"]).CompareTo(Int32.Parse(y["Order"]));
        }
    }
}