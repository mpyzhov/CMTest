using System;

namespace HumanInterfaceDevice
{
    public interface IDeviceHandle : IDisposable
    {
        bool IsInvalid { get; }

        void Close();
    }
}
