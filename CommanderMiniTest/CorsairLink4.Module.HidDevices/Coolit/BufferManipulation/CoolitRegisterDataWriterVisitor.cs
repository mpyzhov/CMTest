using HumanInterfaceDevice.Types;
using System;

namespace CorsairLink4.Module.HidDevices.Coolit.BufferManipulation
{
    public class CoolitRegisterDataWriterVisitor : IBufferVisitor
    {
        public const int DataLengthIndex = 1;
        public const int Offset = 4; // skip ReportId, DataLength, CommandId and OpCode

        private CoolitRegisterData registerData;
        private byte[] payloadData = null;

        public CoolitRegisterDataWriterVisitor(CoolitRegisterData register, byte[] payload = null)
        {
            registerData = register;
            payloadData = payload;
        }

        public void Visit(byte[] buffer)
        {
            int offset = Offset;
            buffer[offset++] = registerData.AddressByteRepresentation;
            buffer[DataLengthIndex]++; // increment whole packet length

            if (registerData.Length > 2) // byte and word commands have fixed payloads
            {
                buffer[offset++] = (byte)registerData.Length;
                buffer[DataLengthIndex]++; // increment whole packet length
            }

            if (payloadData != null)
            {
                int count = Math.Min(payloadData.Length, buffer.Length - offset);
                Array.Copy(payloadData, 0, buffer, offset, count);
                buffer[DataLengthIndex] += (byte)count; // add payload length to the whole packet length
            }
        }
    }
}
