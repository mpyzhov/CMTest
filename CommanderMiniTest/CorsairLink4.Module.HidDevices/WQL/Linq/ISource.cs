using System;
using System.IO;

namespace System.Reactive.Management.Instrumentation.Linq
{
    /// <summary>
    /// Queryable WMI source.
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// Gets the WMI event class name.
        /// </summary>
        string ClassName { get; }

        /// <summary>
        /// Gets the logger used to trace generated WQL queries.
        /// </summary>
        TextWriter Logger { get; }
    }
}
