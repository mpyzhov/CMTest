using CorsairLink4.Module.HidDevices.Core;
using System;

namespace CorsairLink4.Module.HidDevices.Coolit.ResponseParsing
{
    public class ByteParser : IResponseParser
    {
        public static byte ParseResponse(byte[] rawData)
        {
            return rawData[3];
        }

        public object Parse(byte[] rawData)
        {
            return ParseResponse(rawData);
        }
    }
}
