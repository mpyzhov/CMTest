using HumanInterfaceDevice.Types;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits
{
    internal class ReportIdWriterVisitor : IBufferVisitor
    {
        public byte ReportId { get; set; }

        public void Visit(byte[] buffer)
        {
            buffer[0] = ReportId;
        }
    }
}
