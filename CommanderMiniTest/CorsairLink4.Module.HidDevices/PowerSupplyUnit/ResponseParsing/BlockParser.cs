using CorsairLink4.Module.HidDevices.Core;
using System.Text;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits.ResponseParsing
{
    internal class BlockParser : IResponseParser
    {
        private const int StartByteIndex = 3;
        private int blockSize;

        public BlockParser(int blockSize)
        {
            this.blockSize = blockSize;
        }

        public object Parse(byte[] rawData)
        {
            StringBuilder resultBuilder = new StringBuilder();
            for (int i = StartByteIndex; i < StartByteIndex + blockSize; i++)
            {
                if (rawData[i] == 0)
                {
                    break;
                }

                resultBuilder.Append((char)rawData[i]);
            }

            return resultBuilder.ToString();
        }
    }
}
