using HumanInterfaceDevice.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit.BufferManipulation
{
    class CoolitSingleRegisterByteWriterVisitor : IBufferVisitor
    {
        public const int RegisterIndex = 4;
        private readonly byte reg;

        public CoolitSingleRegisterByteWriterVisitor(byte register)
        {
            reg = register;
        }

        public void Visit(byte[] buffer)
        {
            buffer[RegisterIndex] = reg;
            buffer[CoolitRegisterDataWriterVisitor.DataLengthIndex]++;
        }
    }
}
