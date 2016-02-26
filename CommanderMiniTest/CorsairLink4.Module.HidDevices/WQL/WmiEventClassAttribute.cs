using System;

namespace System.Reactive.Management.Instrumentation
{
    /// <summary>
    /// Custom attribute used to tag a class with the WMI event it represents.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class WmiEventClassAttribute : Attribute
    {
        /// <summary>
        /// Creates a new WMI event class mapping.
        /// </summary>
        /// <param name="className">Name of the WMI event represented by the class the attribute is applied to.</param>
        public WmiEventClassAttribute(string className)
        {
            ClassName = className;
        }

        /// <summary>
        /// Gets the WMI event class name.
        /// </summary>
        public string ClassName { get; private set; }
    }
}
