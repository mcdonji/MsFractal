﻿using System;
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

    }

}