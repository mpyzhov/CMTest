using CorsairLink4.Module.HidDevices.Core;

namespace CorsairLink4.Module.HidDevices.CLink.ResponseParsing
{
    internal class LinkFanPowerResponseParser : IResponseParser
    {
        public object Parse(byte[] rawData)
        {
            return rawData[2];
        }
    }
}
