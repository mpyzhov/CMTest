using HumanInterfaceDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    public class CoolitHidDevice : HidDevice
    {
        public CoolitHidDevice(CoolitDeviceEntity coolitDeviceEntity, IHidDeviceManager deviceManager)
            : base(coolitDeviceEntity.DeviceID, deviceManager)
        {
        }
    }
}
