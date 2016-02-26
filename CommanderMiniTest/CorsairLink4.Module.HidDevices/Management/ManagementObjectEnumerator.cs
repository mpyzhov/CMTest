using CorsairLink4.Common.Shared.Utils;
using CorsairLink4.Module.HidDevices.Management.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reactive.Management.Instrumentation;

namespace CorsairLink4.Management
{
    public sealed class ManagementObjectEnumerator
    {
        public static IEnumerable<T> Enumerate<T>(IManagementObjectFilter filter, Func<T> newObjectGenerator)
        {
            string className = GetClassName<T>();
            string namespaceName = GetNamespaceName<T>();
            using (var searcher = new ManagementObjectSearcher(namespaceName, "SELECT * FROM " + className + filter.QueryString))
            {
                using (var managementObjectCollection = searcher.Get())
                {
                    return managementObjectCollection
                        .Cast<ManagementBaseObject>()
                        .Select(mbo => ManagementObjectMapper.Map<T>(mbo, newObjectGenerator))
                        .ToList();
                }
            }
        }

        private static string GetNamespaceName<T>()
        {
            var type = typeof(T);
            var attribute = type.GetCustomAttributes(typeof(WmiClassAttribute), false).Cast<WmiClassAttribute>().SingleOrDefault();
            return attribute == null ? string.Empty : attribute.NamespaceName;
        }

        /// <summary>
        /// Gets the WMI class name from the given type based on an optional mapping.
        /// </summary>
        /// <typeparam name="T">Entity type to get the corresponding WMI class name for.</typeparam>
        /// <returns>WMI class name for the <typeparamref name="T">entity type</typeparamref>.</returns>
        private static string GetClassName<T>()
        {
            var type = typeof(T);
            var attribute = type.GetCustomAttributes(typeof(WmiClassAttribute), false).Cast<WmiClassAttribute>().SingleOrDefault();
            return attribute == null ? type.Name : attribute.ClassName;
        }
    }
}
