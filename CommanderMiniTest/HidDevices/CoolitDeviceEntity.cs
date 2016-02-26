using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    [Serializable]
    public class CoolitDeviceEntity
    {
        public string DeviceID { get; set; }

        public override string ToString()
        {
            return DeviceID;
        }
    }
}
