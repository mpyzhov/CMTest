using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    public enum CoolitModel
    {
        Unknown = 0x00,
        H80 = 0x37,
        CoolingNode = 0x38,
        LightingNode = 0x39,
        H100 = 0x3A,
        H80i = 0x3B,
        H100i = 0x3C,
        Whiptail = 0x3D,
        H100iGT = 0x40,
        H110iGT = 0x41,
        H110i = 0x42,
    };
}
