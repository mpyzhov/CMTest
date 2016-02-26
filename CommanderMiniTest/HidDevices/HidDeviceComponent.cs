using HumanInterfaceDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    public class HidDeviceComponent
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
    }
}
