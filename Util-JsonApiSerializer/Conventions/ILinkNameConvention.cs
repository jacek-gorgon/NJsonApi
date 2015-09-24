using System;
using System.Linq.Expressions;

namespace UtilJsonApiSerializer.Conventions
{
    /// <summary>
    /// Represents a convention for link names given the expression pointing and the linked resource property.
    /// </summary>
    public interface ILinkNameConvention : IConvention
   {
       string GetLinkNameFromExpression<TResource, TLinkedResource>(Expression<Func<TResource, TLinkedResource>> propertyExpression);
   }
}
