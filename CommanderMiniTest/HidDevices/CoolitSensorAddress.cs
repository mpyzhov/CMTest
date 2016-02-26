using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    public enum CoolitSensorType
    {
        FanRpm,
        PumpRpm,
        Temperature,
        Led,
        DeviceName,
        FirmwareVersion,
    }

    public enum CoolitSensorAddress
    {
        Port0,
        Port1,
        Port2,
        Port3,
        Port4,
        Port5,
    }
}
