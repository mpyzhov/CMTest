using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices.General
{
    [Serializable]
    public struct DeviceDistinguishInfo
    {
        public DeviceDistinguishInfo(string pid, string vid)
            : this()
        {
            this.Pid = pid;
            this.Vid = vid;
        }

        public string Pid { get; set; }

        public string Vid { get; set; }

        public static bool operator ==(DeviceDistinguishInfo a, DeviceDistinguishInfo b)
        {
            return a.Pid == b.Pid && a.Vid == b.Vid;
        }

        public static bool operator !=(DeviceDistinguishInfo a, DeviceDistinguishInfo b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is DeviceDistinguishInfo)
                return this == (DeviceDistinguishInfo)obj;
            return false;
        }

        public override int GetHashCode()
        {
            return Pid.GetHashCode() ^ Vid.GetHashCode();
        }
    }
}
