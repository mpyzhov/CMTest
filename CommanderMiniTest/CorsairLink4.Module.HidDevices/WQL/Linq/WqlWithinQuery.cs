using System;
using System.Linq.Expressions;

namespace System.Reactive.Management.Instrumentation.Linq
{
    /// <summary>
    /// Representation of a WQL query with a WITHIN clause.
    /// </summary>
    /// <typeparam name="T">Entity type for queried events.</typeparam>
    public sealed class WqlWithinQuery<T>
    {
        /// <summary>
        /// WQL query source.
        /// </summary>
        private readonly ISource wqlSource;

        /// <summary>
        /// WITHIN clause value.
        /// </summary>
        private readonly TimeSpan withinClause;

        /// <summary>
        /// Creates a new WQL query with a WITHIN clause.
        /// </summary>
        /// <param name="source">WQL query source.</param>
        /// <param name="within">WITHIN clause value.</param>
        internal WqlWithinQuery(ISource source, TimeSpan within)
        {
            wqlSource = source;
            withinClause = within;
        }

        /// <summary>
        /// Applies a filter to the query.
        /// </summary>
        /// <param name="filter">Filter predicate.</param>
        /// <returns>Representation of the query with a filter applied.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public WqlFilteredQuery<T> Where(Expression<Func<T, bool>> filter)
        {
            return new WqlFilteredQuery<T>(wqlSource, withinClause, filter);
        }

        /// <summary>
        /// Applies a projection clause to the query.
        /// </summary>
        /// <typeparam name="R">Projection result type.</typeparam>
        /// <param name="selector">Selector function.</param>
        /// <returns>Representation of the query with a projection clause applied.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public WqlProjectedQuery<T, R> Select<R>(Expression<Func<T, R>> selector)
        {
            return new WqlProjectedQuery<T, R>(wqlSource, withinClause, null, selector);
        }

        /// <summary>
        /// Obtains an observable sequence object to receive the WQL query results.
        /// </summary>
        /// <returns>Observable sequence for query results.</returns>
        public IObservable<T> AsObservable()
        {
            return new WqlProjectedQuery<T, T>(wqlSource, withinClause, null, null).AsObservable();
        }
    }
}
