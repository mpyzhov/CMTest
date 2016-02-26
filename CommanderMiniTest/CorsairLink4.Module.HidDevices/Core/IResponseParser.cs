using System;

namespace CorsairLink4.Module.HidDevices.Core
{
    public interface IResponseParser
    {
        object Parse(byte[] rawData);
    }

    internal class NullParser : IResponseParser
    {
        public object Parse(byte[] rawData)
        {
            return null;
        }
    }
}
