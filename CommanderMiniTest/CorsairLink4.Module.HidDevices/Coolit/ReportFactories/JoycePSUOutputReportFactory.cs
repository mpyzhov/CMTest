using CorsairLink4.Module.HidDevices.Core;
using CorsairLink4.Module.HidDevices.PowerSupplyUnits;
using HumanInterfaceDevice;
using HumanInterfaceDevice.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit.ReportFactories
{
    internal class JoycePSUOutputReportFactory : IReportFactory
    {
        private readonly IReportFactory parentFactory;
        private readonly HidDevice device;
        private readonly byte channel;

        public JoycePSUOutputReportFactory(HidDevice hidDevice, byte address, IReportFactory psuFactory)
        {
            parentFactory = psuFactory;
            device = hidDevice;
            channel = address;
        }

        public OutputReport Create(byte id)
        {
            var report = parentFactory.Create(id);
            byte len = CommandLengthHelper.GetCommandArgLength(id);
            var rawDataExtractor = new BufferDataExtractor();
            report.Data.Accept(rawDataExtractor);
            var bridgeFactory = CreateBridgeFactory(len, rawDataExtractor.Data);
            return bridgeFactory.Create(id);
        }

        private BridgeProxyOutputReportFactory CreateBridgeFactory(byte len, byte[] reportData)
        {
            var readBit = reportData[1] & 1;
            return (readBit == 1) ?
                BridgeProxyOutputReportFactory.CreateReadBlockFactory(device, channel, len) :
                BridgeProxyOutputReportFactory.CreateWriteBlockFactory(device, channel, ExtractReportArg(len, reportData));
        }

        private byte[] ExtractReportArg(byte len, byte[] data)
        {
            // data[0]: report id
            // data[1]: bus address
            // data[2]: command
            // data[3-..]: request data
            return data.Skip(3).Take(len).ToArray();
        }

        private class BufferDataExtractor : IBufferVisitor
        {
            public byte[] Data { get; private set; }

            public void Visit(byte[] buffer)
            {
                Data = buffer.ToArray();
            }
        }
    }
}
