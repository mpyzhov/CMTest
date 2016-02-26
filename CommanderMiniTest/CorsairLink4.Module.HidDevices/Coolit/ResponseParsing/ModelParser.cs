using CorsairLink4.Module.HidDevices.Core;
using System;

namespace CorsairLink4.Module.HidDevices.Coolit.ResponseParsing
{
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
}
