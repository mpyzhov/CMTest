using CorsairLink4.Module.HidDevices.Core;
using CorsairLink4.Module.HidDevices.Core.Helpers;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits.ResponseParsing
{
    internal class BytesAsDoubleParser : IResponseParser
    {
        private int li;

        public BytesAsDoubleParser(int lowByteIndex)
        {
            li = lowByteIndex;
        }

        public object Parse(byte[] rawData)
        {
            return NumConverter.ToDouble((rawData[li + 1] << 8) | rawData[li]);
        }
    }
}
