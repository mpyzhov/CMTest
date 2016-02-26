using CorsairLink4.Module.HidDevices.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit.ResponseParsing
{
    class BlockParser : IResponseParser
    {
        public const int DataStartIndex = 3;

        public static byte[] ParseResponse(byte[] rawData)
        {
            byte len = rawData[DataStartIndex];
            return rawData.Skip(DataStartIndex + 1).Take(len).ToArray();
        }

        public object Parse(byte[] rawData)
        {
            return ParseResponse(rawData);
        }
    }
}
