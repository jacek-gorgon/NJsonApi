using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.Common.Infrastructure
{
    public class CollectionDelta<TElement> : ICollectionDelta<TElement>
    {
        public IEnumerable<TElement> Elements { get; set; }
        
        IEnumerable ICollectionDelta.Elements
        {
            get { return Elements; }
            set { Elements = value.Cast<TElement>().ToList(); }
        }

        private IEqualityComparer<TElement> EqualityComparer;

        public CollectionDelta(Func<TElement, object> idGetter)
        {
            EqualityComparer = new IdAndTypeComparer<TElement>(idGetter);
        }

        public void Apply(ICollection<TElement> input)
        {
            RemovedElements(input)
                .ToList()
                .ForEach(e => input.Remove(e));

            AddedElements(input)
                .ToList()
                .ForEach(e => input.Add(e));
        }

        public IEnumerable<TElement> AddedElements(ICollection<TElement> input)
        {
            return Elements.Except(input, EqualityComparer);
        }

        public IEnumerable<TElement> RemovedElements(ICollection<TElement> input)
        {
            return input.Except(Elements, EqualityComparer);
        }

        public IEnumerable<TElement> UnchangedElements(ICollection<TElement> input)
        {
            return Elements.Intersect(input, EqualityComparer);
        }

        public void Apply(ICollection input)
        {
            throw new NotImplementedException();
        }
    }
}
