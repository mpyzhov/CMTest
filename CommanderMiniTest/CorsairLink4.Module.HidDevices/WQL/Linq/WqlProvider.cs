using System;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace System.Reactive.Management.Instrumentation.Linq
{
    /// <summary>
    /// Qbservable query provider for WQL.
    /// </summary>
    public sealed class WqlProvider : IQbservableProvider
    {
        /// <summary>
        /// Singleton instance of the provider.
        /// </summary>
        private static Lazy<WqlProvider> instance = new Lazy<WqlProvider>(() => new WqlProvider());

        /// <summary>
        /// Private constructor to enable factory pattern.
        /// </summary>
        private WqlProvider()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the provider.
        /// </summary>
        /// <remarks>
        /// This object is publicly exposed and can be used to access certain operators (or specific overloads thereof) that don't have a single source sequence (e.g. Amb).
        /// For WQL, no such operators are currently supported, though we show the exposure of the provider instance nonetheless.
        /// </remarks>
        public static WqlProvider Instance
        {
            get { return instance.Value; }
        }

        /// <summary>
        /// Creates a new query targeting the WQL qbservable query provider using the given expression.
        /// </summary>
        /// <typeparam name="TResult">Entity result type of the query represented by the expression.</typeparam>
        /// <param name="expression">Expression representing the query to be created as a qbservable object.</param>
        /// <returns>Qbservable representation of the given query expression.</returns>
        public IQbservable<TResult> CreateQuery<TResult>(Expression expression)
        {
            return new WqlQbservableSource<TResult>(expression);
        }
    }
}
