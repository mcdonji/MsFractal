using System.Collections.Generic;
using System.Linq;
using Fractal.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fractal.Tests.Domain
{
    [TestClass]
    public class DomainConceptDataAccessTest
    {
        [TestInitialize]
        public void SetUp()
        {
            FractalDb.InitAudit();
            FractalDb.RemoveDomainConcept("Foo");
            FractalDb.RemoveDomainConcept("Foo2");
        }

        [TestMethod]
        public void TestCreateAndGetDc()
        {
            FractalDb.CreateDomainConcept("Foo");
            List<DomainConcept> dcs = FractalDb.Dcs(dc=>dc.Name == "Foo");
            Assert.AreEqual(1, dcs.Count());
            DomainConcept fooReturned = dcs.First();
            Assert.IsNotNull(fooReturned.Id);
            Assert.AreEqual("Foo", fooReturned.Name);
        }


        [TestMethod]
        public void TestRenameDc()
        {
            FractalDb.CreateDomainConcept("Foo");
            List<DomainConcept> dcs = FractalDb.Dcs(dc => dc.Name == "Foo");
            Assert.AreEqual(1, dcs.Count());
            DomainConcept fooReturned = dcs.First();
            Assert.IsNotNull(fooReturned.Id);
            Assert.AreEqual("Foo", fooReturned.Name);

            fooReturned.Name = "Foo2";

            FractalDb.UpdateDc(fooReturned);

            List<DomainConcept> dcsReturned = FractalDb.Dcs(dc => dc.Name == "Foo2");
            Assert.AreEqual(1, dcsReturned.Count());
            DomainConcept renamedFoo = dcsReturned.First();
            Assert.AreEqual(fooReturned.Id, renamedFoo.Id);
            Assert.AreEqual("Foo2", renamedFoo.Name);

            List<AuditDomainConcept> dcHistory = FractalDb.History(renamedFoo);
            Assert.AreEqual(2, dcHistory.Count());

        }


        [TestMethod]
        public void TestAddField()
        {
            FractalDb.CreateDomainConcept("Foo");
            FractalDb.AddDomainConceptField("Foo", "One");
            FractalDb.AddDomainConceptField("Foo", "Two");
            FractalDb.AddDomainConceptField("Foo", "Three");
            DomainConcept foo = FractalDb.Dc(dc => dc.Name == "Foo");
            Assert.AreEqual("Foo", foo.Name);
            Assert.AreEqual(3, foo.Fields.Count);
            Assert.AreEqual("One", foo.Fields[0].FieldName);
            Assert.AreEqual("Two", foo.Fields[1].FieldName);
            Assert.AreEqual("Three", foo.Fields[2].FieldName);

            List<AuditDomainConcept> dcHistory = FractalDb.History(foo);
            Assert.AreEqual(4, dcHistory.Count());

            FractalDb.CreateDomainConcept("Foo2", "One", "Two", "Three");
            DomainConcept foo2 = FractalDb.Dc(dc => dc.Name == "Foo2");
            Assert.AreEqual("Foo2", foo2.Name);
            Assert.AreEqual(3, foo2.Fields.Count);
            Assert.AreEqual("One", foo2.Fields[0].FieldName);
            Assert.AreEqual("Two", foo2.Fields[1].FieldName);
            Assert.AreEqual("Three", foo2.Fields[2].FieldName);

            FractalDb.RemoveDomainConceptField("Foo2", "Three");
            DomainConcept foo2WithRemoved = FractalDb.Dc(dc => dc.Name == "Foo2");
            Assert.AreEqual("Foo2", foo2WithRemoved.Name);
            Assert.AreEqual(2, foo2WithRemoved.Fields.Count);
            Assert.AreEqual("One", foo2WithRemoved.Fields[0].FieldName);
            Assert.AreEqual("Two", foo2WithRemoved.Fields[1].FieldName);
        }

        [TestMethod]
        public void TestSwapFieldOrder()
        {
            FractalDb.CreateDomainConcept("Foo2", "One", "Two", "Three");
            FractalDb.SwapFieldRight("Foo2", "One");
            FractalDb.SwapFieldRight("Foo2", "Three");
            FractalDb.SwapFieldLeft("Foo2", "Three");

            DomainConcept foo2 = FractalDb.Dc(dc => dc.Name == "Foo2");
            Assert.AreEqual("Foo2", foo2.Name);
            Assert.AreEqual(3, foo2.Fields.Count);
            Assert.AreEqual("Two", foo2.Fields[0].FieldName);
            Assert.AreEqual("Three", foo2.Fields[1].FieldName);
            Assert.AreEqual("One", foo2.Fields[2].FieldName);

            List<AuditDomainConcept> dcHistory = FractalDb.History(foo2);
            Assert.AreEqual(3, dcHistory.Count());
        }

        [TestMethod]
        public void TestAddConnectionDistinctDc()
        {
            DomainConcept foo = FractalDb.CreateDomainConcept("Foo", "One", "Two", "Three");
            DomainConcept foo2 = FractalDb.CreateDomainConcept("Foo2", "Three", "Four");
            ConnectionDescription fooToFoo2ConnectionDescription = FractalDb.CreateConnectionDescription(foo, foo2, "Things", Cardinality.OneToOne, false, false, true);
            Assert.AreEqual("Foo", fooToFoo2ConnectionDescription.DcA.Name);
            Assert.AreEqual("Foo2", fooToFoo2ConnectionDescription.DcB.Name);

            List<AuditDomainConcept> fooHistory = FractalDb.History(foo);
            Assert.AreEqual(2, fooHistory.Count());

            List<AuditDomainConcept> foo2History = FractalDb.History(foo2);
            Assert.AreEqual(2, foo2History.Count());

            List<AuditConnectionDescription> connectionDescriptionHistory = FractalDb.History(fooToFoo2ConnectionDescription);
            Assert.AreEqual(1, connectionDescriptionHistory.Count());

        }




    }
}
