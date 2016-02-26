using CorsairLink4.Common.Shared.Communication;
using CorsairLink4.Module.HidDevices.Core;
using CorsairLink4.Module.HidDevices.Models;
using HumanInterfaceDevice;
using System;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit
{
    public class CoolitHidDevice : HidDevice
    {
        public CoolitHidDevice(CoolitDeviceEntity coolitDeviceEntity, IHidDeviceManager deviceManager)
            : base(coolitDeviceEntity.DeviceID, deviceManager)
        {
        }
    }
}
