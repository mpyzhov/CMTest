using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    public enum CoolitCommandOpCode : byte
    {
        WriteByte = 0x06,
        WriteWord = 0x08,
        WriteBlock = 0x0A,

        ReadByte = 0x07,
        ReadWord = 0x09,
        ReadBlock = 0x0B,
        ReadBridge = 0x0F,
    };
}
