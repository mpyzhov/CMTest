using System;

namespace CorsairLink4.Module.HidDevices.Core.Helpers
{
    public static class ByteExt
    {
        public static byte ShiftLeft(this byte value, byte lastbit)
        {
            return (byte)((value << 1) | (lastbit & 1));
        }
    }
}
