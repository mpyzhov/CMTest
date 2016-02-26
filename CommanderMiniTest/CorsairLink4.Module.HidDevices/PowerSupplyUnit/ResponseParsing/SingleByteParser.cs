using CorsairLink4.Module.HidDevices.Core;
using System;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits.ResponseParsing
{
    internal class SingleByteParser : IResponseParser
    {
        private const int BitsInByte = 8; // just to avoid magic numbers
        private int bitsCountToIgnore;

        public SingleByteParser(int bitsCountToIgnore = 0)
        {
            this.bitsCountToIgnore = bitsCountToIgnore;
        }

        public object Parse(byte[] rawData)
        {
            byte val = rawData[3];
            int bitsNum = BitsInByte - bitsCountToIgnore;
            byte mask = (byte)((1 << bitsNum) - 1);
            val &= mask;
            return val;
        }
    }
}
