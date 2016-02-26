using HumanInterfaceDevice.Types;

namespace HumanInterfaceDevice
{
    public class ReportDataExtractorVisitor : IBufferVisitor
    {
        public byte[] Data { get; set; }

        public void Visit(byte[] buffer)
        {
            Data = (byte[])buffer.Clone();
        }
    }
}
