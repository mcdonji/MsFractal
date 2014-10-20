using System;
using Fractal.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fractal.Tests.Domain
{
    [TestClass]
    public class FunctionsTest
    {

        [TestInitialize]
        public void SetUp()
        {
            FractalDb.InitAudit();
            FractalDb.RemoveDomainConcept("Foo");
        }
        
        [TestMethod]
        public void TestGetFunction()
        {
            FractalDb.CreateDomainConcept("Foo", "One", "Two", FF.HANDLE, FF.FFUNC);
            FractalDb.CreateDomainConceptInstance("Foo", "Jim", "M", "AddNums", "public int AddNums(int a, int b) {return a + b;}");
            object returnedValue  = FF.Rf("Foo", "AddNums", 1, 2);
            int sum = (int) returnedValue;
            Assert.AreEqual(3, sum);
            object returnedValue2 = FF.Rf("Foo", "AddNums", 3, 2);
            int sum2 = (int)returnedValue2;
            Assert.AreEqual(5, sum2);
        }
    }
}
