using HumanInterfaceDevice;
using System;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits
{
    public class PowerSupplyUnitHidDevice : HidDevice
    {
        public PowerSupplyUnitHidDevice(string deviceId, IHidDeviceManager deviceManager)
            : base(deviceId, deviceManager)
        {
        }
    }
}
