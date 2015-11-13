using System;
using System.Linq.Expressions;

namespace UtilJsonApiSerializer.Conventions
{
    /// <summary>
    /// Represents a convention for discovering the ID expression for a linked resource given the expression pointing to a reference of that resource.
    /// </summary>
    public interface ILinkIdConvention : IConvention
    {
        /// <summary>
        /// Method should return an expression for accessing the ID of the linked resource.
        /// If no expression is found, it should return null.
        /// </summary>
        Expression<Func<TMain, object>> GetIdExpression<TMain, TLinkedResource>(Expression<Func<TMain, TLinkedResource>> linkedResourceExpression);
    }
}
