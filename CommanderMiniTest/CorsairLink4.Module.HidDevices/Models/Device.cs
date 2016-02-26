using System.Reactive.Management.Instrumentation;
using System.Runtime.Serialization;

namespace CorsairLink4.Module.HidDevices.Models
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
