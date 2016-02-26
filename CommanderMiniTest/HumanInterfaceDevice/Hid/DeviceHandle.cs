using Microsoft.Win32.SafeHandles;

namespace HumanInterfaceDevice
{
    public class DeviceHandle : IDeviceHandle
    {
        public DeviceHandle(SafeFileHandle safeHandle)
        {
            SafeFileHandle = safeHandle;
        }

        public SafeFileHandle SafeFileHandle { get; private set; }

        public bool IsInvalid
        {
            get { return SafeFileHandle.IsInvalid; }
        }

        public void Close()
        {
            SafeFileHandle.Close();
        }

        public void Dispose()
        {
            SafeFileHandle.Dispose();
        }
    }
}
