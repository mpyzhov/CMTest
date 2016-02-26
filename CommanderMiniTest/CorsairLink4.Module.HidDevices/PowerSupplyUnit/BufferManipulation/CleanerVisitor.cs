using HumanInterfaceDevice.Types;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits
{
    internal class CleanerVisitor : IBufferVisitor
    {
        public void Visit(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0;
            }
        }
    }
}
