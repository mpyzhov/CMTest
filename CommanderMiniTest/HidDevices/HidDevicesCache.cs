using HidDevices.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    internal class HidDevicesCache
    {
        private static readonly TimeSpan CacheInvalidateTimeout = TimeSpan.FromSeconds(10);

        private readonly IManagementObjectFilter filter = new WhereFilter("DeviceID").StartsWith("hid");
        private List<Device> devices = null;
        private DateTime lastUpdateTimestamp = DateTime.MinValue;

        public HidDevicesCache()
        {
        }

        public Device[] GetCachedDevices()
        {
            if (IsNeedUpdate())
            {
                UpdateDeviceList();
            }
            return devices.ToArray();
        }

        private void UpdateDeviceList()
        {
            devices = ManagementObjectEnumerator.Enumerate<Device>(filter, DeviceFactory.CreateDevice).ToList();
            lastUpdateTimestamp = DateTime.Now;
        }

        private bool IsNeedUpdate()
        {
            return devices == null || DateTime.Now - lastUpdateTimestamp > CacheInvalidateTimeout;
        }
    }
}
