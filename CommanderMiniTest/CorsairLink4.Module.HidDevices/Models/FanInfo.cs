using System;

namespace CorsairLink4.Module.HidDevices.Models
{
    [Serializable]
    public class FanInfo
    {
        public int Frequency { get; set; }

        public int Power { get; set; }
    }
}
