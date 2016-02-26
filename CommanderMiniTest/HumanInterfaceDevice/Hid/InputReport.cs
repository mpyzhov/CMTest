using HumanInterfaceDevice.Types;

namespace HumanInterfaceDevice
{
    public class InputReport
    {
        private Buffer data;

        public InputReport(byte[] bytes)
        {
            data = new Buffer(bytes);
        }

        public int Id
        {
            get
            {
                return Data.ElementAt(0);
            }
        }

        public IBuffer Data
        {
            get { return data; }
        }
    }
}
