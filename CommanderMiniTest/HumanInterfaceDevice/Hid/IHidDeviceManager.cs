using System;
using System.IO;

namespace HumanInterfaceDevice
{
    public interface IHidDeviceManager
    {
        Guid GetHidGuid();

        IDeviceHandle OpenDevice(string devicePath, HidDeviceAccess access);

        HidDeviceCapabilities GetDeviceCapabilities(IDeviceHandle deviceHandle);

        Stream GetDeviceStream(IDeviceHandle deviceHandle);

        byte[] GetInputReport(IDeviceHandle deviceHandle, int reportSize);
    }
}
