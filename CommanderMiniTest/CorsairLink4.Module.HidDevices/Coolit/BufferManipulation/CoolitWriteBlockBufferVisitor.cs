using HumanInterfaceDevice.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit.BufferManipulation
{
    class CoolitWriteBlockBufferVisitor : IBufferVisitor
    {
        private readonly byte[] data;

        public CoolitWriteBlockBufferVisitor(byte[] writeData)
        {
            Contract.Requires(data != null);
            data = writeData;
        }

        public void Visit(byte[] buffer)
        {
            int offset = CoolitSingleRegisterByteWriterVisitor.RegisterIndex + 1;
            byte length = (byte)data.Length;
            buffer[offset++] = length;
            Array.Copy(data, 0, buffer, offset, length);
            buffer[CoolitRegisterDataWriterVisitor.DataLengthIndex] += (byte)(1 + length);
        }
    }
}
