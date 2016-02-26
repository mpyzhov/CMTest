//using CorsairLink4.Module.HidDevices.Core;
//using System;

//namespace CorsairLink4.Service.Devices.CLink
//{
//    internal class LinkResponseValidator : IResponseValidator
//    {
//        private const byte SuccessCode = 0x01;

//        public bool IsInvalid { get; private set; }

//        public void Visit(byte[] buffer)
//        {
//            IsInvalid = buffer[1] != SuccessCode;
//        }
//    }
//}
