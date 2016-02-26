using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices.General
{
    [Serializable]
    public struct DeviceInstance
    {
        public DeviceInstance(DeviceDistinguishInfo info, string serial)
            : this()
        {
            this.DeviceTypeInfo = info;
            this.SerialNumber = serial;
        }

        public DeviceInstance(string pid, string vid, string serial)
            : this(new DeviceDistinguishInfo(pid, vid), serial)
        {
        }

        public DeviceDistinguishInfo DeviceTypeInfo { get; set; }

        public string SerialNumber { get; set; }

        public static bool operator ==(DeviceInstance a, DeviceInstance b)
        {
            return a.SerialNumber == b.SerialNumber && a.DeviceTypeInfo == b.DeviceTypeInfo;
        }

        public static bool operator !=(DeviceInstance a, DeviceInstance b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return string.Format("Device instance: vid={0}, pid={1}, serial={2}", DeviceTypeInfo.Vid, DeviceTypeInfo.Pid, SerialNumber);
        }

        public override bool Equals(object obj)
        {
            if (obj is DeviceInstance)
                return this == (DeviceInstance)obj;
            return false;
        }

        public override int GetHashCode()
        {
            return SerialNumber.GetHashCode() ^ DeviceTypeInfo.GetHashCode();
        }
    }
}
