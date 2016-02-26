using CorsairLink4.Management;
using CorsairLink4.Module.HidDevices.Coolit;
using CorsairLink4.Module.HidDevices.Core;
using CorsairLink4.Module.HidDevices.Management.Filters;
using CorsairLink4.Module.HidDevices.Models;
using CorsairLink4.Module.HidDevices.PowerSupplyUnits;
using CorsairLink4.Module.HidDevices.Robbins;
using HumanInterfaceDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices
{
    public class HidDevicesComponent
    {
        private readonly HidDevicesCache devicesCache = new HidDevicesCache();

        public List<CoolitDevice> EnumerateCoolitBridgeDevices()
        {
            var result = new List<CoolitDevice>();
            var manager = new HidDeviceManager();

            foreach (var device in devicesCache.GetCachedDevices())
            {
                var deviceEntity = DeviceFactory.DeviceEntityFromDevice(device);
                if (deviceEntity is CoolitDeviceEntity)
                {
                    result.Add(new CoolitDevice(new CoolitHidDevice((CoolitDeviceEntity)deviceEntity, manager)));
                }
            }

            return result;
        }


        private async Task AcceptCoolitBridgeDevices(CoolitDevice coolitDevice)
        {
            await coolitDevice.UpdateProperties();
            await coolitDevice.UpdateInfo();
            await coolitDevice.Accept(visitor);
        }
    }
}
