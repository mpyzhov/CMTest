using CorsairLink4.Module.HidDevices.Core;
using CorsairLink4.Module.HidDevices.Core.Helpers;

namespace CorsairLink4.Module.HidDevices.CLink.ResponseParsing
{
    internal class LinkTemperatureResponseParser : IResponseParser
    {
        public object Parse(byte[] rawData)
        {
            const int LowByteIndex = 2;
            return NumConverter.ToHalfPrecisionFloatingPoint((rawData[LowByteIndex] << 8) | rawData[LowByteIndex + 1]);
        }
    }
}
