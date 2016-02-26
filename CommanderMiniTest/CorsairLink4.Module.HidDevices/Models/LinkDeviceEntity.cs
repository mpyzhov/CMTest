using CorsairLink4.Common.Shared.DevicesData;
using System;

namespace CorsairLink4.Module.HidDevices.Models
{
    [Serializable]
    public class LinkDeviceEntity : BaseDeviceEntity
    {
        public double TemperatureI2C { get; set; }

        public double TemperatureWire { get; set; }

        public FanInfo[] Fans { get; set; }
    }
}
