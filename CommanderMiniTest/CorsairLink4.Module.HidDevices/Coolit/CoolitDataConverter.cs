using System;
using System.Diagnostics.Contracts;

namespace CorsairLink4.Module.HidDevices.Coolit
{
    public class CoolitDataConverter
    {
        public static double WordBytesToTemperature(byte[] data)
        {
            Contract.Requires(data.Length == 2, "You should only try to covert 2-byte words");

            return BitConverter.ToInt16(data, 0) / 256.0;
        }

        public static double WordToTemperature(short data)
        {
            return data / 256.0;
        }

        public static byte[] TemperatureToWordBytes(double temperature)
        {
            return BitConverter.GetBytes(TemperatureToWord(temperature));
        }

        public static short TemperatureToWord(double temperature)
        {
            return (short)(temperature * 256);
        }
    }
}
