using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    public enum CoolitFanMode
    {
        FixedPWM = 0x02,
        FixedRPM = 0x04,
        Default = 0x06,
        Quiet = 0x08,
        Balanced = 0x0A,
        Performance = 0x0C,
        Custom = 0x0E,
        FanModeMax,
    }
}
