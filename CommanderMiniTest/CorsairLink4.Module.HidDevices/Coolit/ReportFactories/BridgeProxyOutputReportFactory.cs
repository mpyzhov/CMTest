using CorsairLink4.Module.HidDevices.Coolit.BufferManipulation;
using CorsairLink4.Module.HidDevices.Core;
using HumanInterfaceDevice;
using HumanInterfaceDevice.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit.ReportFactories
{
    internal class BridgeProxyOutputReportFactory : IReportFactory
    {
        private readonly CoolitCommandByteWriter commandWriter = new CoolitCommandByteWriter();
        private readonly IBufferVisitor blockDataWriter;
        private readonly HidDevice device;
        private readonly byte channel;
        private readonly CoolitCommandOpCode operation;

        private BridgeProxyOutputReportFactory(HidDevice hidDevice, byte address, CoolitCommandOpCode code, byte[] data, byte length)
        {
            device = hidDevice;
            operation = code;
            channel = address;

            blockDataWriter = code == CoolitCommandOpCode.WriteBlock ?
                (IBufferVisitor)new CoolitWriteBlockBufferVisitor(data) : new CoolitReadBlockBufferVisitor(length);

        }

        public static BridgeProxyOutputReportFactory CreateReadBlockFactory(HidDevice device, byte channel, int len)
        {
            return new BridgeProxyOutputReportFactory(device, channel, CoolitCommandOpCode.ReadBlock, null, (byte)len);
        }

        public static BridgeProxyOutputReportFactory CreateWriteBlockFactory(HidDevice device, byte channel, byte[] data)
        {
            return new BridgeProxyOutputReportFactory(device, channel, CoolitCommandOpCode.WriteBlock, data, 0);
        }

        public OutputReport Create(byte id)
        {
            var report = device.CreateOutputReport();
            commandWriter.Channel = channel;
            commandWriter.OpCode = operation;
            report.Data.Accept(commandWriter);
            report.Data.Accept(new CoolitSingleRegisterByteWriterVisitor(id));
            report.Data.Accept(blockDataWriter);
            return report;
        }
    }
}
