using CorsairLink4.Module.HidDevices.Core.Helpers;
using HumanInterfaceDevice.Types;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits
{
    internal class BusWriteVisitor : IBufferVisitor
    {
        public byte Address { get; set; }

        public void Visit(byte[] buffer)
        {
            buffer[1] = Address.ShiftLeft(0);
        }
    }
}
