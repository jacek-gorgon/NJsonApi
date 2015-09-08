using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonApi.Common.Infrastructure;
using SoftwareApproach.TestingExtensions;

namespace NJsonApi.Common.Test.Infrastructure
{
    [TestClass]
    public class DeltaTest
    {
        [TestMethod]
        public void SimpleTestOfFunction()
        {
            //Arange
            var simpleObject = new SimpleTestClass();
            var classUnderTest = new Delta<SimpleTestClass>();

            classUnderTest.AddFilter(t => t.Prop1NotIncluded);
            classUnderTest.ObjectPropertyValues = new Dictionary<string, object>()
                                         {
                                           {"Prop2","b"}
                                         };
            //Act
            classUnderTest.Apply(simpleObject);
            //Assert
            simpleObject.Prop2.ShouldNotBeNull();
            simpleObject.Prop2.ShouldEqual("b");
            simpleObject.Prop1NotIncluded.ShouldBeNull();
        }

        [TestMethod]
        public void TestNotIncludedProperties()
        {
            //Arrange
            var simpleObject = new SimpleTestClass();
            var objectUnderTest = new Delta<SimpleTestClass>();

            objectUnderTest.AddFilter(t => t.Prop1NotIncluded);
            objectUnderTest.ObjectPropertyValues = new Dictionary<string, object>()
                                         {
                                           {"Prop2","b"},
                                           {"Prop1NotIncluded",5} 
                                         };
            //Act
            objectUnderTest.Apply(simpleObject);
            //Assert
            simpleObject.Prop2.ShouldNotBeNull();
            simpleObject.Prop2.ShouldEqual("b");
            simpleObject.Prop1NotIncluded.ShouldBeNull();
        }

        [TestMethod]
        public void TestEmptyPropertiesValues()
        {
            //Arrange
            var simpleObject = new SimpleTestClass();
            var objectUnderTest = new Delta<SimpleTestClass>();
            //Act
            objectUnderTest.AddFilter(t => t.Prop1NotIncluded);
            objectUnderTest.Apply(simpleObject);
            //Assert
            simpleObject.Prop1NotIncluded.ShouldBeNull();
            simpleObject.Prop1.ShouldBeNull();
            simpleObject.Prop2.ShouldBeNull();
        }
    }

    public class SimpleTestClass
    {
        public string Prop1 { get; set; }
        public string Prop2 { get; set; }
        public int? Prop1NotIncluded { get; set; }
    }
    public class SecondSimpleTestClass
    {
        public string Prop1 { get; set; }
        public string Prop2 { get; set; }
        public int Prop1NotIncluded { get; set; }
    }
}
