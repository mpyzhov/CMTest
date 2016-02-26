using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorsairLink4.Module.HidDevices.Core;

namespace CorsairLink4.Module.HidDevices.Coolit.ResponseParsing
{
    public class BCDFWVersionParser : IResponseParser
    {
        public object Parse(byte[] rawData)
        {
            return string.Format("{0}.{1}.{2}", (rawData[4] & 0xF0) >> 4, rawData[4] & 0x0F, rawData[3]);
        }
    }
}
