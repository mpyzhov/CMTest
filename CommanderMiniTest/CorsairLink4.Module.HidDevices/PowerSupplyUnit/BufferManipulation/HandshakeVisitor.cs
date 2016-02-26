using CorsairLink4.Module.HidDevices.Core.Helpers;
using HumanInterfaceDevice.Types;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits
{
    internal class HandshakeVisitor : IBufferVisitor
    {
        public byte Address { get; set; }

        public void Visit(byte[] buffer)
        {
            buffer[1] = (byte)PsuCommandCode.HANDSHAKE;
            buffer[2] = (byte)Address.ShiftLeft(1);
        }
    }
}
