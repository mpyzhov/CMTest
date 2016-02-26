using System;
using System.Linq.Expressions;

namespace System.Reactive.Management.Instrumentation.Linq
{
    /// <summary>
    /// Representation of a WQL query with a filter.
    /// </summary>
    /// <typeparam name="T">Entity type for queried events.</typeparam>
    public sealed class WqlFilteredQuery<T>
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
        /// Filter predicate.
        /// </summary>
        private readonly Expression<Func<T, bool>> filterPredicate;

        /// <summary>
        /// Creates a new WQL query with a filter.
        /// </summary>
        /// <param name="source">WQL query source.</param>
        /// <param name="within">WITHIN clause value.</param>
        /// <param name="filter">Filter predicate.</param>
        internal WqlFilteredQuery(ISource source, TimeSpan within, Expression<Func<T, bool>> filter)
        {
            wqlSource = source;
            withinClause = within;
            filterPredicate = filter;
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
            return new WqlProjectedQuery<T, R>(wqlSource, withinClause, filterPredicate, selector);
        }

        /// <summary>
        /// Obtains an observable sequence object to receive the WQL query results.
        /// </summary>
        /// <returns>Observable sequence for query results.</returns>
        public IObservable<T> AsObservable()
        {
            return new WqlProjectedQuery<T, T>(wqlSource, withinClause, filterPredicate, null).AsObservable();
        }
    }
}
