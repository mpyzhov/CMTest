using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
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

    public class ModelParser : IResponseParser
    {
        public object Parse(byte[] rawData)
        {
            if (rawData[4] != 0)
            {
                return CoolitModel.Unknown;
            }

            return (CoolitModel)rawData[3];
        }
    }

    public class WordParser : IResponseParser
    {
        public object Parse(byte[] rawData)
        {
            short word = (short)(((rawData[4] << 8) | rawData[3]) & 0xffff);
            return word;
        }
    }

    public class ByteParser : IResponseParser
    {
        public static byte ParseResponse(byte[] rawData)
        {
            return rawData[3];
        }

        public object Parse(byte[] rawData)
        {
            return ParseResponse(rawData);
        }
    }

    public class BCDFWVersionParser : IResponseParser
    {
        public object Parse(byte[] rawData)
        {
            return string.Format("{0}.{1}.{2}", (rawData[4] & 0xF0) >> 4, rawData[4] & 0x0F, rawData[3]);
        }
    }
}
