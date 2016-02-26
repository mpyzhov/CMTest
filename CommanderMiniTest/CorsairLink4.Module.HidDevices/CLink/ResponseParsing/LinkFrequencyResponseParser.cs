using CorsairLink4.Module.HidDevices.Core;

namespace CorsairLink4.Module.HidDevices.CLink.ResponseParsing
{
    internal class LinkFrequencyResponseParser : IResponseParser
    {
        public object Parse(byte[] rawData)
        {
            int hi = rawData[3] << 8;
            int lo = rawData[2];
            return hi | lo;
        }
    }
}
