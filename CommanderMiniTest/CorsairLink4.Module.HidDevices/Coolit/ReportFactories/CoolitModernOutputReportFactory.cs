using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorsairLink4.Module.HidDevices.Coolit.BufferManipulation;
using CorsairLink4.Module.HidDevices.Coolit.CommandCodes;
using CorsairLink4.Module.HidDevices.Core;
using HumanInterfaceDevice;
using HumanInterfaceDevice.Types;

namespace CorsairLink4.Module.HidDevices.Coolit.ReportFactories
{
    public class CoolitModernOutputReportFactory : IReportFactory
    {
        private readonly byte outputReportId;
        private HidDevice device;
        private CoolitCommandByteWriter commandWriter = new CoolitCommandByteWriter();

        public CoolitModernOutputReportFactory(HidDevice hidDevice, byte reportId = 0)
        {
            Contract.Requires(hidDevice != null);

            device = hidDevice;
            outputReportId = reportId;
            Channel = (byte)CoolitChannelTag.BridgeDevice;
        }

        public byte Channel { get; set; }

        public byte FanMode { get; set; }

        public byte CurrentSensor { get; set; }

        public byte FanPWM { get; set; }

        public short FanRPM { get; set; }

        public short FanManualTemperature { get; set; }

        public short LedManualTemperature { get; set; }

        public byte[] Payload { get; set; }

        public byte LedMode { get; set; }

        OutputReport IReportFactory.Create(byte id)
        {
            return Create((CoolitModernCommandCode)id);
        }

        public OutputReport Create(CoolitModernCommandCode code)
        {
            var report = device.CreateOutputReport(outputReportId);
            switch (code)
            {
                case CoolitModernCommandCode.DEVICE_ID:
                    FillReport(report, CoolitCommandOpCode.ReadWord, CoolitRegisterData.ModernRegisterAddress.DeviceID);
                    break;
                case CoolitModernCommandCode.FW_VERSION:
                    FillReport(report, CoolitCommandOpCode.ReadWord, CoolitRegisterData.ModernRegisterAddress.FirmwareVersion);
                    break;
                case CoolitModernCommandCode.FAN_CURRENT_RPM:
                    FillReport(report, CoolitCommandOpCode.ReadWord, CoolitRegisterData.ModernRegisterAddress.FanCurrentRPM);
                    break;
                case CoolitModernCommandCode.LED_CURRENT_COLOR:
                    FillReport(report, CoolitCommandOpCode.ReadBlock, CoolitRegisterData.ModernRegisterAddress.LEDCurrentColor);
                    break;
                case CoolitModernCommandCode.TEMPERATURE_CURRENT_TEMPERATURE:
                    FillReport(report, CoolitCommandOpCode.ReadWord, CoolitRegisterData.ModernRegisterAddress.TSCurrentTemperature);
                    break;
                case CoolitModernCommandCode.FAN_MODE_GET:
                    FillReport(report, CoolitCommandOpCode.ReadByte, CoolitRegisterData.ModernRegisterAddress.FanMode);
                    break;
                case CoolitModernCommandCode.FAN_MODE_SET:
                    FillReport(report, CoolitCommandOpCode.WriteByte, CoolitRegisterData.ModernRegisterAddress.FanMode, new byte[] { FanMode });
                    break;
                case CoolitModernCommandCode.FAN_PWM:
                    FillReport(report, CoolitCommandOpCode.WriteByte, CoolitRegisterData.ModernRegisterAddress.FanTargetPWM, new byte[] { FanPWM });
                    break;
                case CoolitModernCommandCode.FAN_RPM:
                    FillReport(report, CoolitCommandOpCode.WriteWord, CoolitRegisterData.ModernRegisterAddress.FanTargetRPM, BitConverter.GetBytes(FanRPM));
                    break;
                case CoolitModernCommandCode.CURRENT_FAN:
                    FillReport(report, CoolitCommandOpCode.WriteByte, CoolitRegisterData.ModernRegisterAddress.CurrentFan, new byte[] { CurrentSensor });
                    break;
                case CoolitModernCommandCode.CURRENT_TEMPERATURE:
                    FillReport(report, CoolitCommandOpCode.WriteByte, CoolitRegisterData.ModernRegisterAddress.CurrentTemperatureSensor, new byte[] { CurrentSensor });
                    break;
                case CoolitModernCommandCode.CURRENT_LED:
                    FillReport(report, CoolitCommandOpCode.WriteByte, CoolitRegisterData.ModernRegisterAddress.CurrentLED, new byte[] { CurrentSensor });
                    break;
                case CoolitModernCommandCode.NUMBER_OF_TEMPERATURES:
                    FillReport(report, CoolitCommandOpCode.ReadByte, CoolitRegisterData.ModernRegisterAddress.NumberOfTemperatureSensors);
                    break;
                case CoolitModernCommandCode.NUMBER_OF_FANS:
                    FillReport(report, CoolitCommandOpCode.ReadByte, CoolitRegisterData.ModernRegisterAddress.NumberOfFans);
                    break;
                case CoolitModernCommandCode.NUMBER_OF_LEDS:
                    FillReport(report, CoolitCommandOpCode.ReadByte, CoolitRegisterData.ModernRegisterAddress.NumberOfLEDChannels);
                    break;
                case CoolitModernCommandCode.LED_MODE_GET:
                    FillReport(report, CoolitCommandOpCode.ReadByte, CoolitRegisterData.ModernRegisterAddress.LEDMode);
                    break;
                case CoolitModernCommandCode.LED_MODE_SET:
                    FillReport(report, CoolitCommandOpCode.WriteByte, CoolitRegisterData.ModernRegisterAddress.LEDMode, new byte[] { LedMode });
                    break;
                case CoolitModernCommandCode.FAN_RPM_TABLE:
                    FillReport(report, CoolitCommandOpCode.WriteBlock, CoolitRegisterData.ModernRegisterAddress.FanRPMTable, Payload);
                    break;
                case CoolitModernCommandCode.FAN_TEMPERATURE_TABLE:
                    FillReport(report, CoolitCommandOpCode.WriteBlock, CoolitRegisterData.ModernRegisterAddress.FanTemperatureTable, Payload);
                    break;
                case CoolitModernCommandCode.FAN_MANUAL_TEMPERATURE:
                    FillReport(report, CoolitCommandOpCode.WriteWord, CoolitRegisterData.ModernRegisterAddress.FanManualTemperature, BitConverter.GetBytes(FanManualTemperature));
                    break;
                case CoolitModernCommandCode.LED_MANUAL_TEMPERATURE:
                    FillReport(report, CoolitCommandOpCode.WriteWord, CoolitRegisterData.ModernRegisterAddress.LEDManualTemperature, BitConverter.GetBytes(LedManualTemperature));
                    break;
                case CoolitModernCommandCode.LED_CYCLE_COLORS:
                    FillReport(report, CoolitCommandOpCode.WriteBlock, CoolitRegisterData.ModernRegisterAddress.LEDCycleColors, Payload);
                    break;
                case CoolitModernCommandCode.LED_TEMPERATURE_MODE_COLORS:
                    FillReport(report, CoolitCommandOpCode.WriteBlock, CoolitRegisterData.ModernRegisterAddress.LEDTMColors, Payload);
                    break;
                case CoolitModernCommandCode.LED_TEMPERATURE_MODE_TEMPERATURES:
                    FillReport(report, CoolitCommandOpCode.WriteBlock, CoolitRegisterData.ModernRegisterAddress.LEDTMTemperatures, Payload);
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
