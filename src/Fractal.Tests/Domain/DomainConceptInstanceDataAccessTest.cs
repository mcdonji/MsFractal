using System;
using System.Collections.Generic;
using System.Linq;
using Fractal.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fractal.Tests.Domain
{
    [TestClass]
    public class DomainConceptInstanceDataAccessTest
    {
        [TestInitialize]
        public void SetUp()
        {
            FractalDb.InitAudit();
            FractalDb.RemoveDomainConcept("Foo");
            FractalDb.RemoveDomainConcept("Foo2");
        }

        [TestMethod]
        public void TestCreateDci()
        {
            FractalDb.CreateDomainConcept("Foo", "One", "Two", "Three");
            DomainConceptInstance dci = FractalDb.CreateDomainConceptInstance("Foo", "1", "2", "3");
            Assert.IsNotNull(dci.Id);
            Assert.IsNotNull(dci.DomainConceptId);
            Assert.AreEqual("Foo", dci.DomainConceptName);
            Assert.AreEqual("1", dci["One"]);
            Assert.AreEqual("2", dci["Two"]);
            Assert.AreEqual("3", dci["Three"]);
        }

        [TestMethod]
        public void TestSelectDci()
        {
            FractalDb.CreateDomainConcept("Foo", "One", "Two", "Three");
            DomainConceptInstance foo1 = FractalDb.CreateDomainConceptInstance("Foo", "a", "b", "c");
            DomainConceptInstance foo2 = FractalDb.CreateDomainConceptInstance("Foo", "d", "e", "f");
            DomainConceptInstance foo3 = FractalDb.CreateDomainConceptInstance("Foo", "g", "h", "c");

            List<DomainConceptInstance> foos = FractalDb.Select("Foo", FractalDb.Where("Three", "c"));

            Assert.AreEqual(2, foos.Count);
            List<DomainConceptInstance> orderedFoos = foos.OrderBy(f => f["One"]).ToList();
            Assert.AreEqual("a", orderedFoos[0]["One"]);
            Assert.AreEqual("b", orderedFoos[0]["Two"]);
            Assert.AreEqual("c", orderedFoos[0]["Three"]);
            Assert.AreEqual("g", orderedFoos[1]["One"]);
            Assert.AreEqual("h", orderedFoos[1]["Two"]);
            Assert.AreEqual("c", orderedFoos[1]["Three"]);
        }

        [TestMethod]
        public void TestUpdateDci()
        {
            FractalDb.CreateDomainConcept("Foo", "One", "Two", "Three");
            DomainConceptInstance foo1 = FractalDb.CreateDomainConceptInstance("Foo", "a", "b", "c");
            DomainConceptInstance foo2 = FractalDb.CreateDomainConceptInstance("Foo", "d", "e", "f");
            DomainConceptInstance foo3 = FractalDb.CreateDomainConceptInstance("Foo", "g", "h", "c");

            foo2.Set("Three", "c");
            FractalDb.UpdateDci(foo2);

            List<DomainConceptInstance> foos = FractalDb.Select("Foo", FractalDb.Where("Three", "c"));
            Assert.AreEqual(3, foos.Count);
        }

        [TestMethod]
        public void TestCreateConnection()
        {
            DomainConcept foo = FractalDb.CreateDomainConcept("Foo", "One", "Two", "Three");
            DomainConcept foo2 = FractalDb.CreateDomainConcept("Foo2", "Three", "Four");
            FractalDb.CreateConnectionDescription(foo, foo2, "Things", Cardinality.OneToMany, false, false, true);
            FractalDb.CreateConnectionDescription(foo, foo2, "OtherThings", Cardinality.OneToMany, false, false, true);

            //ConnectionDescriptionParameter cdp = FractalDb.CreateConnectionDescriptionParameter(foo, foo2, "Things", "Traversal");

            DomainConceptInstance jim = FractalDb.CreateDomainConceptInstance("Foo", "Jim", "M", "McDonald");
            DomainConceptInstance blue = FractalDb.CreateDomainConceptInstance("Foo2", "Blue", "Tomato");
            DomainConceptInstance red = FractalDb.CreateDomainConceptInstance("Foo2", "Red", "Ant");
            DomainConceptInstance green = FractalDb.CreateDomainConceptInstance("Foo2", "Green", "Leaves");

            Connection con1 = FractalDb.Connect(jim, blue, "Things");
            Connection con2 = FractalDb.Connect(jim, red, "Things");
            Connection con3 = FractalDb.Connect(jim, green, "OtherThings");

            DomainConceptInstance returnedJim = FractalDb.Dci(dci => dci.DomainConceptName == "Foo" && dci.Id == jim.Id);

            List<DomainConceptInstance> allLeft = returnedJim.AllLeftDcis();
            List<DomainConceptInstance> allRight = returnedJim.AllRightDcis();

            Assert.AreEqual(0, allLeft.Count);
            Assert.AreEqual(3, allRight.Count);

        }




    }

}
