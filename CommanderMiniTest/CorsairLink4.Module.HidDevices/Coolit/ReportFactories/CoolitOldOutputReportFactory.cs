using CorsairLink4.Module.HidDevices.Coolit.BufferManipulation;
using CorsairLink4.Module.HidDevices.Coolit.CommandCodes;
using CorsairLink4.Module.HidDevices.Core;
using HumanInterfaceDevice;
using System;
using System.Diagnostics.Contracts;

namespace CorsairLink4.Module.HidDevices.Coolit.ReportFactories
{
    public class CoolitOldOutputReportFactory : IReportFactory
    {
        private readonly byte outputReportId;
        private HidDevice device;
        private CoolitCommandByteWriter commandWriter = new CoolitCommandByteWriter();

        public CoolitOldOutputReportFactory(HidDevice hidDevice, byte reportId = 0)
        {
            Contract.Requires(hidDevice != null);

            device = hidDevice;
            outputReportId = reportId;
            Channel = (byte)CoolitChannelTag.BridgeDevice;
        }

        public byte CurrentSensor { get; set; }

        public byte Channel { get; set; }

        public short FanMode { get; set; }

        public short FanRPM { get; set; }

        public short FanPWM { get; set; }

        public short FanManualTemperature { get; set; }

        public byte[] Payload { get; set; }

        OutputReport IReportFactory.Create(byte id)
        {
            return Create((CoolitOldCommandCode)id);
        }

        public OutputReport Create(CoolitOldCommandCode code)
        {
            var report = device.CreateOutputReport(outputReportId);
            switch (code)
            {
                case CoolitOldCommandCode.DEVICE_ID:
                    FillReport(report, CoolitCommandOpCode.ReadWord, CoolitRegisterData.OldRegisterAddress.DeviceId);
                    break;
                case CoolitOldCommandCode.FW_VERSION:
                    FillReport(report, CoolitCommandOpCode.ReadWord, CoolitRegisterData.OldRegisterAddress.FirmwareVersion);
                    break;
                case CoolitOldCommandCode.TEMPERATURE_CURRENT_TEMPERATURE:
                    FillReport(report, CoolitCommandOpCode.ReadWord, CoolitRegisterData.OldRegisterAddress.CurrentTemperature1 + CurrentSensor);
                    break;
                case CoolitOldCommandCode.FAN_CURRENT_RPM:
                    FillReport(report, CoolitCommandOpCode.ReadWord, CoolitRegisterData.OldRegisterAddress.FanCurrentRPM1 + CurrentSensor);
                    break;
                case CoolitOldCommandCode.FAN_RPM:
                    FillReport(report, CoolitCommandOpCode.WriteWord, CoolitRegisterData.OldRegisterAddress.FanTargetRPM1 + 0x10 * CurrentSensor, BitConverter.GetBytes(FanRPM));
                    break;
                case CoolitOldCommandCode.FAN_PWM:
                    FillReport(report, CoolitCommandOpCode.WriteWord, CoolitRegisterData.OldRegisterAddress.FanTargetPWM1 + 0x10 * CurrentSensor, BitConverter.GetBytes(FanPWM));
                    break;
                case CoolitOldCommandCode.FAN_RPM_TABLE:
                    FillReport(report, CoolitCommandOpCode.WriteBlock, CoolitRegisterData.OldRegisterAddress.FanRPMTable1 + 0x10 * CurrentSensor, Payload);
                    break;
                case CoolitOldCommandCode.FAN_TEMPERATURE_TABLE:
                    FillReport(report, CoolitCommandOpCode.WriteBlock, CoolitRegisterData.OldRegisterAddress.FanTemperatureTable1 + 0x10 * CurrentSensor, Payload);
                    break;
                case CoolitOldCommandCode.FAN_MANUAL_TEMPERATURE:
                    FillReport(report, CoolitCommandOpCode.WriteWord, CoolitRegisterData.OldRegisterAddress.FanManualTemperature1 + CurrentSensor, BitConverter.GetBytes(FanManualTemperature));
                    break;
                case CoolitOldCommandCode.FAN_MODE_GET:
                    FillReport(report, CoolitCommandOpCode.ReadWord, CoolitRegisterData.OldRegisterAddress.FanMode1 + 0x10 * CurrentSensor);
                    break;
                case CoolitOldCommandCode.FAN_MODE_SET:
                    FillReport(report, CoolitCommandOpCode.WriteWord, CoolitRegisterData.OldRegisterAddress.FanMode1 + 0x10 * CurrentSensor, BitConverter.GetBytes(FanMode));
                    break;
                default:
                    break;
            }

            return report;
        }

        private void FillReport(OutputReport report, CoolitCommandOpCode opCode, IConvertible regAddress, byte[] payLoad = null)
        {
            commandWriter.Channel = Channel;
            commandWriter.OpCode = opCode;
            report.Data.Accept(commandWriter);
            var datawriter = new CoolitRegisterDataWriterVisitor(regAddress.ToRegisterData(), payLoad);
            report.Data.Accept(datawriter);
        }
    }
}
