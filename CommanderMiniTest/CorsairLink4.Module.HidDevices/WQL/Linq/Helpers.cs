using System;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Reactive.Management.Instrumentation.Linq
{
    /// <summary>
    /// Misc. helpers class.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Info-of operator for methods.
        /// </summary>
        /// <typeparam name="T">First generic parameter.</typeparam>
        /// <typeparam name="R">Second generic parameter.</typeparam>
        /// <param name="f">Expression tree whose body is supposed to be a method call expression.</param>
        /// <returns>MethodInfo for the method called in the expression tree.</returns>
        public static MethodInfo InfoOf<T, R>(Expression<Func<T, R>> f)
        {
            return ((MethodCallExpression)f.Body).Method;
        }
    }
}
