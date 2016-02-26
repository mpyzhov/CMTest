using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorsairLink4.Module.HidDevices.Coolit.ReportFactories;
using ModernRegisterAddress = CorsairLink4.Module.HidDevices.Coolit.CoolitRegisterData.ModernRegisterAddress;
using BridgeRegisterAddress = CorsairLink4.Module.HidDevices.Coolit.CoolitRegisterData.BridgeRegisterAddress;
using OldRegisterAddress = CorsairLink4.Module.HidDevices.Coolit.CoolitRegisterData.OldRegisterAddress;
using OldLightingNodeRegisterAddress = CorsairLink4.Module.HidDevices.Coolit.CoolitRegisterData.OldLightingNodeRegisterAddress;

namespace CorsairLink4.Module.HidDevices.Coolit
{
    public static class CoolitRegisterDataUtils
    {
        public static CoolitRegisterData ToRegisterData(this IConvertible adress)
        {
            // TODO: [checked] pure evil here, don't know how to implement it correctly [Bushoc: I don't explore what does it mean, TODO]
            if (adress is BridgeRegisterAddress)
            {
                return CreateRegisterData((BridgeRegisterAddress)adress);
            }
            else if (adress is ModernRegisterAddress)
            {
                return CreateRegisterData((ModernRegisterAddress)adress);
            }
            else if (adress is OldRegisterAddress)
            {
                return CreateRegisterData((OldRegisterAddress)adress);
            }
            else if (adress is OldLightingNodeRegisterAddress)
            {
                return CreateRegisterData((OldLightingNodeRegisterAddress)adress);
            }
            else
            {
                throw new InvalidEnumArgumentException();
            }
        }

        public static CoolitRegisterData CreateRegisterData(ModernRegisterAddress address)
        {
            return new CoolitRegisterData(address.ToAccess(), address.ToLength(), (byte)address);
        }
        public static CoolitRegisterData CreateRegisterData(BridgeRegisterAddress address)
        {
            return new CoolitRegisterData(address.ToAccess(), address.ToLength(), (byte)address);
        }
        public static CoolitRegisterData CreateRegisterData(OldRegisterAddress address)
        {
            return new CoolitRegisterData(address.ToAccess(), address.ToLength(), (byte)address);
        }
        public static CoolitRegisterData CreateRegisterData(OldLightingNodeRegisterAddress address)
        {
            return new CoolitRegisterData(address.ToAccess(), address.ToLength(), (byte)address);
        }

        public static RegisterAccess ToAccess(this ModernRegisterAddress address)
        {
            switch (address)
            {
                case ModernRegisterAddress.CurrentLED:
                case ModernRegisterAddress.LEDMode:
                case ModernRegisterAddress.LEDManualTemperature:
                case ModernRegisterAddress.LEDTMTemperatures:
                case ModernRegisterAddress.LEDTMColors:
                case ModernRegisterAddress.LEDCycleColors:
                case ModernRegisterAddress.CurrentTemperatureSensor:
                case ModernRegisterAddress.CurrentFan:
                case ModernRegisterAddress.FanMode:
                case ModernRegisterAddress.FanTargetPWM:
                case ModernRegisterAddress.FanTargetRPM:
                case ModernRegisterAddress.FanManualTemperature:
                case ModernRegisterAddress.FanRPMTable:
                case ModernRegisterAddress.FanTemperatureTable:
                    return RegisterAccess.ReadWrite;
                #region defaultcases
                case ModernRegisterAddress.DeviceID:
                case ModernRegisterAddress.FirmwareVersion:
                case ModernRegisterAddress.ProductName:
                case ModernRegisterAddress.Status:
                case ModernRegisterAddress.NumberOfLEDChannels:
                case ModernRegisterAddress.LEDCurrentColor:
                case ModernRegisterAddress.NumberOfTemperatureSensors:
                case ModernRegisterAddress.TSCurrentTemperature:
                case ModernRegisterAddress.NumberOfFans:
                case ModernRegisterAddress.FanCurrentRPM:
                case ModernRegisterAddress.FanMaxRPM:
                #endregion defaultcases
                default: return RegisterAccess.ReadOnly;
            }
        }

        public static int ToLength(this ModernRegisterAddress address)
        {
            switch (address)
            {

                case ModernRegisterAddress.FirmwareVersion:
                case ModernRegisterAddress.TSCurrentTemperature:
                case ModernRegisterAddress.LEDManualTemperature:
                case ModernRegisterAddress.FanTargetRPM:
                case ModernRegisterAddress.FanManualTemperature:
                case ModernRegisterAddress.FanCurrentRPM:
                case ModernRegisterAddress.FanMaxRPM:
                    return 2;
                case ModernRegisterAddress.LEDCurrentColor:
                    return 3;
                case ModernRegisterAddress.LEDTMTemperatures:
                    return 6;
                case ModernRegisterAddress.ProductName:
                    return 8;
                case ModernRegisterAddress.LEDTMColors:
                    return 9;
                case ModernRegisterAddress.FanRPMTable:
                case ModernRegisterAddress.FanTemperatureTable:
                    return 10;
                case ModernRegisterAddress.LEDCycleColors:
                    return 12;
                #region defaultCases
                case ModernRegisterAddress.DeviceID:
                case ModernRegisterAddress.CurrentLED:
                case ModernRegisterAddress.LEDMode:
                case ModernRegisterAddress.CurrentTemperatureSensor:
                case ModernRegisterAddress.CurrentFan:
                case ModernRegisterAddress.FanMode:
                case ModernRegisterAddress.FanTargetPWM:
                case ModernRegisterAddress.Status:
                case ModernRegisterAddress.NumberOfLEDChannels:
                case ModernRegisterAddress.NumberOfTemperatureSensors:
                case ModernRegisterAddress.NumberOfFans:
                #endregion defaultCases
                default: return 1;
            }
        }

        public static RegisterAccess ToAccess(this BridgeRegisterAddress address)
        {
            return RegisterAccess.ReadOnly;
        }

        public static int ToLength(this BridgeRegisterAddress address)
        {
            switch (address)
            {
                case BridgeRegisterAddress.DeviceAddress:
                    return 8;
                case BridgeRegisterAddress.UDID:
                default:
                    return 4;
            }
        }

        public static RegisterAccess ToAccess(this OldRegisterAddress address)
        {
            switch (address)
            {
                case OldRegisterAddress.FirmwareVersion:

                case OldRegisterAddress.CurrentTemperature1:
                case OldRegisterAddress.CurrentTemperature2:
                case OldRegisterAddress.CurrentTemperature3:
                case OldRegisterAddress.CurrentTemperature4:

                case OldRegisterAddress.FanCurrentRPM1:
                case OldRegisterAddress.FanCurrentRPM2:
                case OldRegisterAddress.FanCurrentRPM3:
                case OldRegisterAddress.FanCurrentRPM4:
                case OldRegisterAddress.FanCurrentRPM5:
                    return RegisterAccess.ReadOnly;
                #region defaultCases
                case OldRegisterAddress.FanManualTemperature1:
                case OldRegisterAddress.FanManualTemperature2:
                case OldRegisterAddress.FanManualTemperature3:
                case OldRegisterAddress.FanManualTemperature4:
                case OldRegisterAddress.FanManualTemperature5:

                case OldRegisterAddress.FanMode1:
                case OldRegisterAddress.FanMode2:
                case OldRegisterAddress.FanMode3:
                case OldRegisterAddress.FanMode4:
                case OldRegisterAddress.FanMode5:

                case OldRegisterAddress.FanTargetPWM1:
                case OldRegisterAddress.FanTargetPWM2:
                case OldRegisterAddress.FanTargetPWM3:
                case OldRegisterAddress.FanTargetPWM4:
                case OldRegisterAddress.FanTargetPWM5:

                case OldRegisterAddress.FanTargetRPM1:
                case OldRegisterAddress.FanTargetRPM2:
                case OldRegisterAddress.FanTargetRPM3:
                case OldRegisterAddress.FanTargetRPM4:
                case OldRegisterAddress.FanTargetRPM5:

                case OldRegisterAddress.FanRPMTable1:
                case OldRegisterAddress.FanRPMTable2:
                case OldRegisterAddress.FanRPMTable3:
                case OldRegisterAddress.FanRPMTable4:
                case OldRegisterAddress.FanRPMTable5:

                case OldRegisterAddress.FanTemperatureTable1:
                case OldRegisterAddress.FanTemperatureTable2:
                case OldRegisterAddress.FanTemperatureTable3:
                case OldRegisterAddress.FanTemperatureTable4:
                case OldRegisterAddress.FanTemperatureTable5:
                #endregion defaultCases
                default:
                    return RegisterAccess.ReadWrite;
            }
        }

        public static int ToLength(this OldRegisterAddress address)
        {
            switch (address)
            {
                case OldRegisterAddress.FirmwareVersion:

                case OldRegisterAddress.CurrentTemperature1:
                case OldRegisterAddress.CurrentTemperature2:
                case OldRegisterAddress.CurrentTemperature3:
                case OldRegisterAddress.CurrentTemperature4:

                case OldRegisterAddress.FanCurrentRPM1:
                case OldRegisterAddress.FanCurrentRPM2:
                case OldRegisterAddress.FanCurrentRPM3:
                case OldRegisterAddress.FanCurrentRPM4:
                case OldRegisterAddress.FanCurrentRPM5:

                case OldRegisterAddress.FanManualTemperature1:
                case OldRegisterAddress.FanManualTemperature2:
                case OldRegisterAddress.FanManualTemperature3:
                case OldRegisterAddress.FanManualTemperature4:
                case OldRegisterAddress.FanManualTemperature5:

                case OldRegisterAddress.FanMode1:
                case OldRegisterAddress.FanMode2:
                case OldRegisterAddress.FanMode3:
                case OldRegisterAddress.FanMode4:
                case OldRegisterAddress.FanMode5:

                case OldRegisterAddress.FanTargetPWM1:
                case OldRegisterAddress.FanTargetPWM2:
                case OldRegisterAddress.FanTargetPWM3:
                case OldRegisterAddress.FanTargetPWM4:
                case OldRegisterAddress.FanTargetPWM5:

                case OldRegisterAddress.FanTargetRPM1:
                case OldRegisterAddress.FanTargetRPM2:
                case OldRegisterAddress.FanTargetRPM3:
                case OldRegisterAddress.FanTargetRPM4:
                case OldRegisterAddress.FanTargetRPM5:
                    return 2;

                case OldRegisterAddress.FanRPMTable1:
                case OldRegisterAddress.FanRPMTable2:
                case OldRegisterAddress.FanRPMTable3:
                case OldRegisterAddress.FanRPMTable4:
                case OldRegisterAddress.FanRPMTable5:

                case OldRegisterAddress.FanTemperatureTable1:
                case OldRegisterAddress.FanTemperatureTable2:
                case OldRegisterAddress.FanTemperatureTable3:
                case OldRegisterAddress.FanTemperatureTable4:
                case OldRegisterAddress.FanTemperatureTable5:
                default:
                    return 10;
            }
        }

        public static RegisterAccess ToAccess(this OldLightingNodeRegisterAddress address)
        {
            switch (address)
            {
                case OldLightingNodeRegisterAddress.FirmwareVersion:
                    return RegisterAccess.ReadOnly;

                case OldLightingNodeRegisterAddress.ManualTemperature1:
                case OldLightingNodeRegisterAddress.ManualTemperature2:

                case OldLightingNodeRegisterAddress.CycleMode1:
                case OldLightingNodeRegisterAddress.CycleMode2:

                case OldLightingNodeRegisterAddress.CycleColors1:
                case OldLightingNodeRegisterAddress.CycleColors2:

                case OldLightingNodeRegisterAddress.TemperaturesAndColors1:
                case OldLightingNodeRegisterAddress.TemperaturesAndColors2:
                default:
                    return RegisterAccess.ReadWrite;
            }
        }

        public static int ToLength(this OldLightingNodeRegisterAddress address)
        {
            switch (address)
            {
                case OldLightingNodeRegisterAddress.FirmwareVersion:
                case OldLightingNodeRegisterAddress.ManualTemperature1:
                case OldLightingNodeRegisterAddress.ManualTemperature2:
                    return 2;

                case OldLightingNodeRegisterAddress.CycleMode1:
                case OldLightingNodeRegisterAddress.CycleMode2:
                    return 1;

                case OldLightingNodeRegisterAddress.CycleColors1:
                case OldLightingNodeRegisterAddress.CycleColors2:
                    return 16;

                case OldLightingNodeRegisterAddress.TemperaturesAndColors1:
                case OldLightingNodeRegisterAddress.TemperaturesAndColors2:
                default:
                    return 15;
            }
        }
    }
}
