using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management;

namespace CorsairLink4.Management
{
    public sealed class ManagementObjectMapper
    {
        private static Dictionary<Type, PropertyDescriptorCollection> propsMap = new Dictionary<Type, PropertyDescriptorCollection>();

        public static T Map<T>(ManagementBaseObject mbo, Func<T> newObjectGenerator)
        {
            T result = newObjectGenerator();
            var type = typeof(T);
            PropertyDescriptorCollection props;

            if (!propsMap.TryGetValue(type, out props))
            {
                props = TypeDescriptor.GetProperties(type);
                propsMap[type] = props;
            }

            using (mbo)
            {
                var dataCollection = mbo.Properties;
                PropertyData data = null;
                object value = null;
                foreach (PropertyDescriptor propertyDescriptor in props)
                {
                    data = dataCollection[propertyDescriptor.Name];
                    if (data == null)
                    {
                        continue;
                    }

                    value = data.Value;
                    if (value == null)
                    {
                        continue;
                    }

                    if (propertyDescriptor.Converter.CanConvertFrom(value.GetType()))
                    {
                        propertyDescriptor.SetValue(result, propertyDescriptor.Converter.ConvertFrom(value));
                    }
                    else
                    {
                        propertyDescriptor.SetValue(result, value);
                    }
                }
            }

            return result;
        }
    }
}
