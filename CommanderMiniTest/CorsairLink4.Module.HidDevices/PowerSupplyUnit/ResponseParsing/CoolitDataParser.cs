using CorsairLink4.Module.HidDevices.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits.ResponseParsing
{
    public class CoolitDataParser : IResponseParser
    {
        private readonly IResponseParser originalParser;

        public CoolitDataParser(IResponseParser originalParser)
        {
            this.originalParser = originalParser;
        }

        public object Parse(byte[] rawData)
        {
            // rawData comes directly from coolit device 
            // first of all need to parse it with coolit block parser
            // since original parser expects to find data starting from index 3, we need to shift data
            var data = Enumerable.Repeat<byte>(0, 3).Concat(CorsairLink4.Module.HidDevices.Coolit.ResponseParsing.BlockParser.ParseResponse(rawData)).ToArray();
            return originalParser.Parse(data);
        }
    }
}
