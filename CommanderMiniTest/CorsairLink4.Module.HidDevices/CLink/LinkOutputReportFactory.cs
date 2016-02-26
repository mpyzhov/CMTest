//using CorsairLink4.Service.Devices.Buffer;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CorsairLink4.Service.Devices.CLink
//{
//    internal class LinkOutputReportFactory : IReportFactory
//    {
//        private int size;
//        private LinkCommandCodeWriterVisitor commandCode = new LinkCommandCodeWriterVisitor();
//        private WriteElementVisitor addressWriter = new WriteElementVisitor() { Index = 2 };
//        private WriteElementVisitor fanFrequencyWriter = new WriteElementVisitor() { Index = 3 };

//        public LinkOutputReportFactory(int outputReportByteLength)
//        {
//            size = outputReportByteLength;
//        }

//        public byte Address
//        {
//            get { return addressWriter.Value; }
//            set { addressWriter.Value = value; }
//        }

//        public byte FanFrequency
//        {
//            get { return fanFrequencyWriter.Value; }
//            set { fanFrequencyWriter.Value = value; }
//        }

//        OutputReport IReportFactory.Create(byte id)
//        {
//            return Create((LinkCommandCode)id);
//        }

//        public OutputReport Create(LinkCommandCode code)
//        {
//            var report = new OutputReport(size);
//            commandCode.Code = code;
//            report.Data.Accept(commandCode);
//            report.Data.Accept(addressWriter);
//            switch (code)
//            {
//                case LinkCommandCode.READ_STATUS:
//                    break;
//                case LinkCommandCode.READ_TEMPERATURE:
//                    break;
//                case LinkCommandCode.READ_FREQUENCY:
//                    break;
//                case LinkCommandCode.WRITE_FAN_PWM:
//                    report.Data.Accept(fanFrequencyWriter);
//                    break;
//                default:
//                    break;
//            }

//            return report;
//        }
//    }
//}
