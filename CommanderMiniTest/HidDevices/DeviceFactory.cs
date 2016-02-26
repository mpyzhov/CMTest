using HidDevices.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    public class DeviceFactory
    {
        public static Device CreateDevice()
        {
            return new Device();
        }

        public static CoolitDeviceEntity DeviceEntityFromDevice(Device device)
        {
            string pidPrefix = "PID_";
            int index = device.DeviceID.IndexOf("PID_") + pidPrefix.Length;
            string currentPID = device.DeviceID.Substring(index, 4).ToLower();

            if (currentPID == "0c04")
            {
                var result = new CoolitDeviceEntity();

                result.DeviceID = device.DeviceID;
                return result;
            }
            return null;
        }
    }
}
