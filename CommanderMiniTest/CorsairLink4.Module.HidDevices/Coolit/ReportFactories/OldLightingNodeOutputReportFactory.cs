using CorsairLink4.Module.HidDevices.Coolit.BufferManipulation;
using CorsairLink4.Module.HidDevices.Coolit.CommandCodes;
using CorsairLink4.Module.HidDevices.Core;
using HumanInterfaceDevice;
using System;
using System.Diagnostics.Contracts;

namespace CorsairLink4.Module.HidDevices.Coolit.ReportFactories
{
    public class OldLightingNodeOutputReportFactory : IReportFactory
    {
        private readonly byte outputReportId;
        private HidDevice device;
        private CoolitCommandByteWriter commandWriter = new CoolitCommandByteWriter();

        public OldLightingNodeOutputReportFactory(HidDevice hidDevice, byte reportId = 0)
        {
            Contract.Requires(hidDevice != null);

            device = hidDevice;
            outputReportId = reportId;
            Channel = (byte)CoolitChannelTag.BridgeDevice;
        }

        public byte CurrentLed { get; set; }

        public byte Channel { get; set; }

        public byte[] Payload { get; set; }

        public byte LedMode { get; set; }

        public short ManualTemperature { get; set; }

        OutputReport IReportFactory.Create(byte id)
        {
            return Create((LedNodeCommandOpCode)id);
        }

        public OutputReport Create(LedNodeCommandOpCode code)
        {
            var report = device.CreateOutputReport(outputReportId);
            switch (code)
            {
                case LedNodeCommandOpCode.DEVICE_ID:
                    FillReport(report, CoolitCommandOpCode.ReadWord, CoolitRegisterData.OldLightingNodeRegisterAddress.DeviceId);
                    break;
                case LedNodeCommandOpCode.FW_VERSION:
                    FillReport(report, CoolitCommandOpCode.ReadWord, CoolitRegisterData.OldLightingNodeRegisterAddress.FirmwareVersion);
                    break;
                case LedNodeCommandOpCode.SET_COLORS:
                    FillReport(report,
                        CoolitCommandOpCode.WriteBlock,
                        CurrentLed == 0x00
                                ? CoolitRegisterData.OldLightingNodeRegisterAddress.CycleColors1
                                : CoolitRegisterData.OldLightingNodeRegisterAddress.CycleColors2,
                        Payload);
                    break;
                case LedNodeCommandOpCode.SET_MANUAL_TEMPERATURE:
                    FillReport(report,
                        CoolitCommandOpCode.WriteWord,
                        CurrentLed == 0x00
                                ? CoolitRegisterData.OldLightingNodeRegisterAddress.ManualTemperature1
                                : CoolitRegisterData.OldLightingNodeRegisterAddress.ManualTemperature2,
                        BitConverter.GetBytes(ManualTemperature)
                    );
                    break;
                case LedNodeCommandOpCode.SET_COLORS_AND_TEMPERATURES:
                    FillReport(report,
                        CoolitCommandOpCode.WriteBlock,
                        CurrentLed == 0x00
                            ? CoolitRegisterData.OldLightingNodeRegisterAddress.TemperaturesAndColors1
                            : CoolitRegisterData.OldLightingNodeRegisterAddress.TemperaturesAndColors2,
                        Payload);
                    break;
                case LedNodeCommandOpCode.SET_MODE:
                    FillReport(report,
                        CoolitCommandOpCode.WriteByte,
                        CurrentLed == 0x00
                            ? CoolitRegisterData.OldLightingNodeRegisterAddress.CycleMode1
                            : CoolitRegisterData.OldLightingNodeRegisterAddress.CycleMode2,
                        new byte[] { LedMode });
                    break;
                case LedNodeCommandOpCode.GET_MODE:
                    FillReport(report,
                        CoolitCommandOpCode.ReadByte,
                        CurrentLed == 0x00
                            ? CoolitRegisterData.OldLightingNodeRegisterAddress.CycleMode1
                            : CoolitRegisterData.OldLightingNodeRegisterAddress.CycleMode2);
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
