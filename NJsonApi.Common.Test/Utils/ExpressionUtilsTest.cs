using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonApi.Common.Utils;
using SoftwareApproach.TestingExtensions;
using System.Diagnostics;

namespace NJsonApi.Common.Test.Utils
{
    [TestClass]
    public class ExpressionUtilsTest
    {        
        [TestMethod]
        public void ToCompiledSetterAction_ReturnsWorkingDelegate()
        {
            var foo = new Foo();
            var action = typeof(Foo).GetProperty(nameof(foo.Bar)).ToCompiledSetterAction<Foo, string>();
            action(foo, "bar");
            foo.Bar.ShouldEqual("bar");
        }

        [TestMethod]
        public void ToCompiledGetterFuncTest_ReturnsWorkingDelegate()
        {
            var foo = new Foo()
            {
                Bar = "bar"
            };

            var function = typeof(Foo).GetProperty(nameof(foo.Bar)).ToCompiledGetterFunc<Foo, string>();
            var value = function(foo);
            value.ShouldEqual("bar");

            var del = typeof(Foo).GetProperty(nameof(foo.Bar)).ToCompiledGetterDelegate(typeof(Foo), typeof(string));
        }

        [TestMethod]
        public void ToCompiledSetterAction_GivenDerivedType_ReturnsWorkingDelegate()
        {
            var foo = new Foo();
            var action = typeof(Foo).GetProperty(nameof(foo.Baz)).ToCompiledSetterAction<Foo, string>();
            action(foo, "baz");
            foo.Baz.ShouldEqual("baz");
        }

        [TestMethod]
        public void ToCompiledSetterAction_GivenBaseType_ReturnsWorkingDelegate()
        {
            var foo = new Foo();
            var action = typeof(Foo).GetProperty(nameof(foo.Bar)).ToCompiledSetterAction<Foo, object>();
            action(foo, "bar");
            foo.Bar.ShouldEqual("bar");
        }

        [TestMethod]
        public void ToCompiledSetterAction_GivenBaseTypeAndInvalidObject_ThrowsInvalidCastException()
        {
            var foo = new Foo();
            var action = typeof(Foo).GetProperty(nameof(foo.Bar)).ToCompiledSetterAction<Foo, object>();
            Testing.ShouldThrowException<InvalidCastException>(() => action(foo, new object()), "InvalidCastException expected.");
        }

        [TestMethod]
        public void ToCompiledSetterAction_GivenMismatchedType_ThrowsInvalidOperationException()
        {
            var foo = new Foo();
            var pi = typeof(Foo).GetProperty(nameof(foo.Bar));
            
            Testing.ShouldThrowException<InvalidOperationException>(() => pi.ToCompiledSetterAction<Foo, int>(), "InvalidOperationException expected.");
        }

        [TestMethod]
        public void ReflectionVsCompiledLambda_Benchmark()
        {
            var foo = new Foo()
            {
                Bar = "bar"
            };

            var pi = typeof(Foo).GetProperty("Bar");
            var getterMi = pi.GetGetMethod();
            var compiledLambda = pi.ToCompiledGetterFunc<Foo, string>();

            int count = 1000000;
            var watch = new Stopwatch();
            watch.Start();
            for(int i = 0; i < count; i++)
            {
                getterMi.Invoke(foo, null);
            }
            watch.Stop();
            Trace.WriteLine($"{count} invocations of MethodInfo.Invoke() took {watch.ElapsedMilliseconds}ms.");

            watch.Restart();
            for (int i = 0; i < count; i++)
            {
                compiledLambda(foo);
            }
            watch.Stop();
            Trace.WriteLine($"{count} invocations of compiled lambda took {watch.ElapsedMilliseconds}ms.");
        } 

        private class Foo
        {
            public string Bar { get; set; }
            public object Baz { get; set; }
        }
    }
}
