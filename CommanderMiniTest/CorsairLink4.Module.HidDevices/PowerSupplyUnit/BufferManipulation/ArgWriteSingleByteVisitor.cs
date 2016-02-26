using HumanInterfaceDevice.Types;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits
{
    internal class ArgWriteSingleByteVisitor : IBufferVisitor
    {
        public byte Arg { get; set; }

        public void Visit(byte[] buffer)
        {
            buffer[3] = Arg;
        }
    }
}
