using CorsairLink4.Module.HidDevices.Core;
using System;

namespace CorsairLink4.Module.HidDevices.Coolit.ResponseParsing
{
    public class WordParser : IResponseParser
    {
        public object Parse(byte[] rawData)
        {
            short word = (short)(((rawData[4] << 8) | rawData[3]) & 0xffff);
            return word;
        }
    }
}
