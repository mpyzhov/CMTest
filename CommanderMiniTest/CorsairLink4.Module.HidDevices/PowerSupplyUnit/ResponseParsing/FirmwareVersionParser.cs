using CorsairLink4.Module.HidDevices.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits.ResponseParsing
{
    class FirmwareVersionParser : IResponseParser
    {
        private const int Size = 4;
        private const int StartByteIndex = 3;

        public FirmwareVersionParser()
        {
        }

        public object Parse(byte[] rawData)
        {
            var resultBuilder = new StringBuilder();
            for (int i = StartByteIndex; i < StartByteIndex + Size; i++)
            {
                resultBuilder.AppendFormat(".{0}", rawData[i]);
            }

            return resultBuilder.ToString().Substring(1);
        }
    }
}
