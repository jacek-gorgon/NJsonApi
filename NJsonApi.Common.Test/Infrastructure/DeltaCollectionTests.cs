using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonApi.Common.Infrastructure;
using System.Collections.Generic;
using SoftwareApproach.TestingExtensions;
using System.Linq;

namespace NJsonApi.Common.Test.Infrastructure
{
    [TestClass]
    public class DeltaCollectionTests
    {
        class Foo
        {
            public int Id { get; set; }
        }

        [TestMethod]
        public void AddedElements_CalculateProperly()
        {
            var testCollection = new List<Foo>
            {
                new Foo { Id = 1 },
                new Foo { Id = 2 },
                new Foo { Id = 3 },
            };

            var delta = new CollectionDelta<Foo>(f => f.Id)
            {
                Elements = new List<Foo>()
                {
                    new Foo { Id = 2 },
                    new Foo { Id = 3 },
                    new Foo { Id = 4 },
                }
            };

            var added = delta.AddedElements(testCollection);
            added.ShouldHaveCountOf(1);
            added.First().Id.ShouldEqual(4);
        }

        [TestMethod]
        public void RemovedElements_CalculateProperly()
        {
            var testCollection = new List<Foo>
            {
                new Foo { Id = 1 },
                new Foo { Id = 2 },
                new Foo { Id = 3 },
            };

            var delta = new CollectionDelta<Foo>(f => f.Id)
            {
                Elements = new List<Foo>()
                {
                    new Foo { Id = 2 },
                    new Foo { Id = 3 },
                    new Foo { Id = 4 },
                }
            };

            var removed = delta.RemovedElements(testCollection);
            removed.ShouldHaveCountOf(1);
            removed.First().Id.ShouldEqual(1);
        }

        [TestMethod]
        public void UnchangedElements_CalculateProperly()
        {
            var testCollection = new List<Foo>
            {
                new Foo { Id = 1 },
                new Foo { Id = 2 },
                new Foo { Id = 3 },
            };

            var delta = new CollectionDelta<Foo>(f => f.Id)
            {
                Elements = new List<Foo>()
                {
                    new Foo { Id = 2 },
                    new Foo { Id = 3 },
                    new Foo { Id = 4 },
                }
            };

            var unchanged = delta.UnchangedElements(testCollection);
            unchanged.ShouldHaveCountOf(2);
            unchanged.Count(f => f.Id == 2).ShouldEqual(1);
            unchanged.Count(f => f.Id == 3).ShouldEqual(1);
        }
    }
}
