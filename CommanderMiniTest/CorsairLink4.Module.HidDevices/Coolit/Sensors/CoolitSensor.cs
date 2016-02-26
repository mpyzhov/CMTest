using CorsairLink4.Common.DevicesDefinitions.Coolit;
using System;

namespace CorsairLink4.Module.HidDevices.Coolit.Sensors
{
    public class CoolitSensor
    {
        public CoolitSensorAddress Channel { get; set; }

        public string Name { get; set; }

        public int Value { get; set; }

        public CoolitSensorType SensorType { get; set; }
    }
}
