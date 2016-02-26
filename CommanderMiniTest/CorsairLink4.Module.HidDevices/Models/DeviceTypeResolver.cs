using CorsairLink4.Common.DevicesDefinitions.Common;

namespace CorsairLink4.Module.HidDevices.Models
{
    internal static class DeviceTypeResolver
    {
        public static DeviceType GetDeviceType(Device device)
        {
            return GetDeviceType(device.DeviceID);
        }

        public static DeviceType GetDeviceType(string deviceID)
        {
            // UNDONE: dummy test implementation
            string pidPrefix = "PID_";
            int index = deviceID.IndexOf("PID_") + pidPrefix.Length;
            string currentPID = deviceID.Substring(index, DeviceIdentity.Corsair.Pid.CorsairPSU_HX850i.Length).ToLower();
            var type = DeviceTypeDetector.GetDeviceType(CreateInstanceFromPid(currentPID));
            if (IsHidDeviceSupported(type))
            {
                return type;
            }

            return DeviceType.NotSupported;
        }

        private static DeviceInstance CreateInstanceFromPid(string pid)
        {
            return new DeviceInstance(pid, DeviceIdentity.Corsair.Vid, "");
        }

        private static bool IsHidDeviceSupported(DeviceType type)
        {
            return type == DeviceType.Link ||
                   type == DeviceType.PowerSupply ||
                   type == DeviceType.Coolit ||
                   type == DeviceType.RobbinsPSU;
        }
    }
}
