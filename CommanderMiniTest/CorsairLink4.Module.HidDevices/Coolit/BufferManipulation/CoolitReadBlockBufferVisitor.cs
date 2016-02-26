using HumanInterfaceDevice.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit.BufferManipulation
{
    class CoolitReadBlockBufferVisitor : IBufferVisitor
    {
        private readonly byte length;

        public CoolitReadBlockBufferVisitor(byte readLength)
        {
            Contract.Requires(readLength > 0);
            length = readLength;
        }

        public void Visit(byte[] buffer)
        {
            int offset = CoolitRegisterDataWriterVisitor.Offset + 1;
            buffer[offset++] = length;
            buffer[CoolitRegisterDataWriterVisitor.DataLengthIndex]++;
        }
    }
}
