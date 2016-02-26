using System;

namespace HumanInterfaceDevice
{
    public struct HidDeviceCapabilities
    {
        public ushort Usage;
        public ushort UsagePage;
        public ushort InputReportByteLength;
        public ushort OutputReportByteLength;
        public ushort VersionNumber;
    }
}
