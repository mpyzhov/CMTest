using System;

namespace CorsairLink4.Module.HidDevices.Management.Filters
{
    public interface IManagementObjectFilter
    {
        string QueryString { get; }
    }
}
