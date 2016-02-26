using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    public class CoolitSensor
    {
        public CoolitSensorAddress Channel { get; set; }

        public string Name { get; set; }

        public int Value { get; set; }

        public CoolitSensorType SensorType { get; set; }
    }
}
