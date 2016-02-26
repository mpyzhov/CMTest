using CorsairLink4.Module.HidDevices.Coolit.BufferManipulation;
using CorsairLink4.Module.HidDevices.Core;
using HumanInterfaceDevice;
using System.Diagnostics.Contracts;

namespace CorsairLink4.Module.HidDevices.Coolit.ReportFactories
{
    public class CoolitBridgeOutputReportFactory : IReportFactory
    {
        private readonly byte outputReportId;
        private HidDevice device;
        private CoolitCommandByteWriter commandWriter = new CoolitCommandByteWriter();

        public CoolitBridgeOutputReportFactory(HidDevice hidDevice, byte reportId = 0)
        {
            Contract.Requires(hidDevice != null);

            device = hidDevice;
            outputReportId = reportId;
            Channel = 0x0;
        }

        public byte Channel { get; set; }

        OutputReport IReportFactory.Create(byte id)
        {
            return Create((CoolitBridgeCommandCode)id);
        }

        public OutputReport Create(CoolitBridgeCommandCode code)
        {
            var report = device.CreateOutputReport(outputReportId);

            switch (code)
            {
                case CoolitBridgeCommandCode.MODEL:
                    commandWriter.OpCode = CoolitCommandOpCode.ReadByte;
                    commandWriter.Channel = Channel;
                    report.Data.Accept(commandWriter);
                    var dataWriter = new CoolitRegisterDataWriterVisitor(CoolitRegisterDataUtils.CreateRegisterData(CoolitRegisterData.ModernRegisterAddress.DeviceID));
                    report.Data.Accept(dataWriter);
                    break;
                case CoolitBridgeCommandCode.DEVICE_ADDRESS:
                    var regDataDeviceAddress = CoolitRegisterDataUtils.CreateRegisterData(CoolitRegisterData.BridgeRegisterAddress.DeviceAddress);
                    commandWriter.OpCode = CoolitCommandOpCode.ReadBridge;
                    commandWriter.Channel = (byte)(regDataDeviceAddress.AddressByteRepresentation >> 4);
                    report.Data.Accept(commandWriter);
                    break;
                case CoolitBridgeCommandCode.UDID:
                    var regDataUDID = CoolitRegisterDataUtils.CreateRegisterData(CoolitRegisterData.BridgeRegisterAddress.UDID);
                    commandWriter.OpCode = CoolitCommandOpCode.ReadBridge;
                    commandWriter.Channel = (byte)(regDataUDID.AddressByteRepresentation >> 4);
                    report.Data.Accept(commandWriter);
                    break;
                default:
                    break;
            }

            return report;
        }
    }
}
