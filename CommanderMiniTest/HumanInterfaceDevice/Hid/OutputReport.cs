using HumanInterfaceDevice.Types;

namespace HumanInterfaceDevice
{
    public class OutputReport
    {
        private Buffer data;

        public OutputReport(int reportLength)
        {
            data = new Buffer(reportLength);
        }

        public IBuffer Data
        {
            get { return data; }
        }
    }
}
