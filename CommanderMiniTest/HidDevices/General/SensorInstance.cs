using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices.General
{
    public struct SensorInstance
    {
        public SensorInstance(DeviceInstance instance, string originalName)
            : this()
        {
            OriginalName = originalName;
            ParentDevice = instance;
        }

        public string OriginalName { get; set; }

        public DeviceInstance ParentDevice { get; set; }
    }
}
