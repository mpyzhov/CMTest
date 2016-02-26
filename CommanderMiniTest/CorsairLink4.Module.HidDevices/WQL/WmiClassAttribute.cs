using System;

namespace System.Reactive.Management.Instrumentation
{
    /// <summary>
    /// Custom attribute used to tag a class with the WMI class it represents.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class WmiClassAttribute : Attribute
    {
        /// <summary>
        /// Creates a new WMI class mapping.
        /// </summary>
        /// <param name="className">Name of the WMI class represented by the class the attribute is applied to.</param>
        public WmiClassAttribute(string namespaceName, string className)
        {
            NamespaceName = namespaceName;
            ClassName = className;
        }

        /// <summary>
        /// Gets the WMI class name.
        /// </summary>
        public string ClassName { get; private set; }

        /// <summary>
        /// Gets the WMI class namespace
        /// </summary>
        public string NamespaceName { get; private set; }
    }
}
