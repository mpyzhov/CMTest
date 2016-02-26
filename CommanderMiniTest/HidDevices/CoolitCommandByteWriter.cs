using HumanInterfaceDevice.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    public class CoolitCommandByteWriter : IBufferVisitor
    {
        private const int DataLengthIndex = 1;
        private static byte commandId;

        public CoolitCommandOpCode OpCode { get; set; }

        public byte Channel { get; set; }

        public void Visit(byte[] buffer)
        {
            buffer[2] = commandId++;
            buffer[3] = (byte)((byte)OpCode | (Channel << 4));

            buffer[DataLengthIndex] += 2; // add 2 bytes to DataLength byte
        }
    }
}
