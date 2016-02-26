using HumanInterfaceDevice;
using System;

namespace CorsairLink4.Module.HidDevices.Core
{
    public interface IReportFactory
    {
        OutputReport Create(byte id);
    }
}
