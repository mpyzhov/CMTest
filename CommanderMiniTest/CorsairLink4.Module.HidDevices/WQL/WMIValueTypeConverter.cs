using System.ComponentModel;
using System.Globalization;
using System.Management;

namespace System.Reactive.Management.Instrumentation
{
    public class WMIValueTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string date = (string)value;
                if (!string.IsNullOrWhiteSpace(date))
                {
                    return ManagementDateTimeConverter.ToDateTime(date);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
