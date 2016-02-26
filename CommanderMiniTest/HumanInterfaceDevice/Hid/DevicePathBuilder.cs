using System;

namespace HumanInterfaceDevice
{
    internal sealed class DevicePathBuilder
    {
        private string format;

        public DevicePathBuilder(Guid hidGuid)
        {
            format = "\\\\.\\{0}#{{" + hidGuid.ToString() + "}}";
        }

        public string Build(string deviceId)
        {
            return string.Format(format, deviceId.Replace('\\', '#'));
        }
    }
}
