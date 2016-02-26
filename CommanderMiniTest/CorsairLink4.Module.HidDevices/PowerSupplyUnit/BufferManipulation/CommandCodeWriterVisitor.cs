using HumanInterfaceDevice.Types;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits
{
    internal class CommandCodeWriterVisitor : IBufferVisitor
    {
        public PsuCommandCode Code { get; set; }

        public void Visit(byte[] buffer)
        {
            buffer[2] = (byte)Code;
        }
    }
}
