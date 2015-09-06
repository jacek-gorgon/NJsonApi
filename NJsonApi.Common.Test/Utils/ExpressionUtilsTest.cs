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
            var action = typeof(Foo).GetProperty("Bar").ToCompiledSetterAction<Foo, string>();
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

            var function = typeof(Foo).GetProperty("Bar").ToCompiledGetterFunc<Foo, string>();
            var value = function(foo);
            value.ShouldEqual("bar");
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
        }
    }
}
