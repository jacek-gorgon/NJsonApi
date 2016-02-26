using System;
using NJsonApi.Utils;
using System.Diagnostics;
using Xunit;
using System.Reflection;

namespace NJsonApi.Common.Test.Utils
{
     public class ExpressionUtilsTest
    {
        [Fact]
        public void ToCompiledSetterAction_ReturnsWorkingDelegate()
        {
            // Arrange
            var foo = new Foo();
            var action = typeof(Foo).GetProperty(nameof(foo.Bar)).ToCompiledSetterAction<Foo, string>();

            // Act
            action(foo, "bar");

            // Assert
            Assert.Equal("bar", foo.Bar);
        }

        [Fact]
        public void ToCompiledGetterFuncTest_ReturnsWorkingDelegate()
        {
            // Arrange
            var foo = new Foo()
            {
                Bar = "bar"
            };

            var function = typeof(Foo).GetProperty(nameof(foo.Bar)).ToCompiledGetterFunc<Foo, string>();

            // Act
            var value = function(foo);

            // Assert
            Assert.Equal("bar", value);

            var del = typeof(Foo).GetProperty(nameof(foo.Bar)).ToCompiledGetterDelegate(typeof(Foo), typeof(string));
        }

        [Fact]
        public void ToCompiledSetterAction_GivenDerivedType_ReturnsWorkingDelegate()
        {
            // Arrange
            var foo = new Foo();
            var action = typeof(Foo).GetProperty(nameof(foo.Baz)).ToCompiledSetterAction<Foo, string>();

            // Act
            action(foo, "baz");

            // Assert
            Assert.Equal("baz", foo.Baz);
        }

        [Fact]
        public void ToCompiledSetterAction_GivenBaseType_ReturnsWorkingDelegate()
        {
            // Arrange
            var foo = new Foo();
            var action = typeof(Foo).GetProperty(nameof(foo.Bar)).ToCompiledSetterAction<Foo, object>();

            // Act
            action(foo, "bar");

            // Assert
            Assert.Equal("bar", foo.Bar);
        }

        [Fact]
        public void ToCompiledSetterAction_GivenBaseTypeAndInvalidObject_ThrowsInvalidCastException()
        {
            // Arrange
            var foo = new Foo();
            var action = typeof(Foo).GetProperty(nameof(foo.Bar)).ToCompiledSetterAction<Foo, object>();

            // Act/Assert
            Assert.Throws<InvalidCastException>(() => action(foo, new object()));
        }

        [Fact]
        public void ToCompiledSetterAction_GivenMismatchedType_ThrowsInvalidOperationException()
        {
            // Arrange
            var foo = new Foo();
            var pi = typeof(Foo).GetProperty(nameof(foo.Bar));

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => pi.ToCompiledSetterAction<Foo, int>());
        }

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
            for (int i = 0; i < count; i++)
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