using System.Collections;
using System.Collections.Generic;

namespace NJsonApi.Common.Infrastructure
{
    public interface ICollectionDelta
    {
        IEnumerable Elements { get; set; }
        void Apply(ICollection input);
    }

    public interface ICollectionDelta<TElement> : ICollectionDelta
    {
        new IEnumerable<TElement> Elements { get; set; }
        void Apply(ICollection<TElement> input);

        IEnumerable<TElement> AddedElements(ICollection<TElement> input);
        IEnumerable<TElement> RemovedElements(ICollection<TElement> input);
        IEnumerable<TElement> UnchangedElements(ICollection<TElement> input);
        
    }
}
