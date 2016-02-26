using CorsairLink4.Module.HidDevices.Core;
using System;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits.ResponseValidation
{
    internal class PsuResponseValidator : IResponseValidator
    {
        private const byte ErrorCode = 0xFE;

        public bool IsInvalid { get; private set; }

        public void Visit(byte[] buffer)
        {
            if (buffer[1] == (byte)PsuCommandCode.HANDSHAKE)
            {
                IsInvalid = buffer[3] == ErrorCode;
            }
            else
            {
                IsInvalid = buffer[2] == ErrorCode;
            }
        }
    }
}
