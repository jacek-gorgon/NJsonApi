using NJsonApi.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NJsonApi.Common.Test.Infrastructure
{
    public class DeltaCollectionTests
    {
        class Foo
        {
            public int Id { get; set; }
        }

        [Fact]
        public void AddedElements_CalculateProperly()
        {
            // Arrange
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

            // Act
            var added = delta.AddedElements(testCollection);

            // Assert
            Assert.Equal(1, added.Count());
            Assert.Equal(4, added.First().Id);
        }

        [Fact]
        public void RemovedElements_CalculateProperly()
        {
            // Arrange
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

            // Act
            var result = delta.RemovedElements(testCollection);

            // Assert
            Assert.Equal(1, result.Count());
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void UnchangedElements_CalculateProperly()
        {
            // Arrange
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

            // Act
            var unchanged = delta.UnchangedElements(testCollection);

            // Assert
            unchanged.Single(f => f.Id == 2);
            unchanged.Single(f => f.Id == 3);
            Assert.Equal(2, unchanged.Count());
        }
    }
}
