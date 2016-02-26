using HidDevices.WQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    [DataContract]
    [WmiClass(@"root\CIMV2", "Win32_PnPEntity")]
    public class Device
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Manufacturer { get; set; }

        [DataMember]
        public string Caption { get; set; }

        [DataMember]
        public string ClassGUID { get; set; }

        [DataMember]
        public string DeviceID { get; set; }

        [DataMember]
        public string PNPDeviceID { get; set; }
    }
}
