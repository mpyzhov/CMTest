using CorsairLink4.Common.DevicesDefinitions.Common;
using CorsairLink4.Common.Shared.DevicesData;
using CorsairLink4.Common.Shared.Utils;
using CorsairLink4.Management;
using System;
using System.Linq;
using System.Management;

namespace CorsairLink4.Module.HidDevices.Models
{
    public class DeviceFactory
    {
        public static Device CreateDevice()
        {
            return new Device();
        }

        public static BaseDeviceEntity CreateDeviceEntity(ManagementBaseObject mbo)
        {
            Device device = ManagementObjectMapper.Map<Device>(mbo, CreateDevice);
            return DeviceEntityFromDevice(device);
        }

        public static BaseDeviceEntity DeviceEntityFromDevice(Device device)
        {
            DeviceType type = DeviceTypeResolver.GetDeviceType(device);
            BaseDeviceEntity result = null;
            switch (type)
            {
                case DeviceType.PowerSupply:
                    result = new PowerSupplyUnitDeviceEntity();
                    break;
                case DeviceType.Link:
                    result = new LinkDeviceEntity()
                    {
                        Fans = Enumerable.Repeat(0, 2).Select(index => new FanInfo()).ToArray()
                    };
                    break;
                case DeviceType.Coolit:
                    result = new CoolitDeviceEntity();
                    break;
                case DeviceType.RobbinsPSU:
                    result = new RobbinsPsuDeviceEntity();
                    break;
                case DeviceType.NotSupported:
                    result = new BaseDeviceEntity();
                    break;
                default:
                    break;
            }

            result.DeviceID = device.DeviceID;
            return result;
        }
    }
}
