using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    public static class CoolitRegisterDataUtils
    {
        public static CoolitRegisterData ToRegisterData(this IConvertible adress)
        {
            // TODO: [checked] pure evil here, don't know how to implement it correctly [Bushoc: I don't explore what does it mean, TODO]
            if (adress is HidDevices.CoolitRegisterData.BridgeRegisterAddress)
            {
                return CreateRegisterData((HidDevices.CoolitRegisterData.BridgeRegisterAddress)adress);
            }
            else if (adress is HidDevices.CoolitRegisterData.ModernRegisterAddress)
            {
                return CreateRegisterData((HidDevices.CoolitRegisterData.ModernRegisterAddress)adress);
            }
            else if (adress is HidDevices.CoolitRegisterData.OldRegisterAddress)
            {
                return CreateRegisterData((HidDevices.CoolitRegisterData.OldRegisterAddress)adress);
            }
            else if (adress is HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress)
            {
                return CreateRegisterData((HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress)adress);
            }
            else
            {
                throw new InvalidEnumArgumentException();
            }
        }

        public static CoolitRegisterData CreateRegisterData(HidDevices.CoolitRegisterData.ModernRegisterAddress address)
        {
            return new CoolitRegisterData(address.ToAccess(), address.ToLength(), (byte)address);
        }
        public static CoolitRegisterData CreateRegisterData(HidDevices.CoolitRegisterData.BridgeRegisterAddress address)
        {
            return new CoolitRegisterData(address.ToAccess(), address.ToLength(), (byte)address);
        }
        public static CoolitRegisterData CreateRegisterData(HidDevices.CoolitRegisterData.OldRegisterAddress address)
        {
            return new CoolitRegisterData(address.ToAccess(), address.ToLength(), (byte)address);
        }
        public static CoolitRegisterData CreateRegisterData(HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress address)
        {
            return new CoolitRegisterData(address.ToAccess(), address.ToLength(), (byte)address);
        }

        public static RegisterAccess ToAccess(this HidDevices.CoolitRegisterData.ModernRegisterAddress address)
        {
            switch (address)
            {
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.CurrentLED:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.LEDMode:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.LEDManualTemperature:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.LEDTMTemperatures:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.LEDTMColors:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.LEDCycleColors:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.CurrentTemperatureSensor:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.CurrentFan:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanMode:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanTargetPWM:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanTargetRPM:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanManualTemperature:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanRPMTable:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanTemperatureTable:
                    return RegisterAccess.ReadWrite;
                #region defaultcases
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.DeviceID:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FirmwareVersion:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.ProductName:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.Status:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.NumberOfLEDChannels:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.LEDCurrentColor:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.NumberOfTemperatureSensors:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.TSCurrentTemperature:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.NumberOfFans:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanCurrentRPM:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanMaxRPM:
                #endregion defaultcases
                default: return RegisterAccess.ReadOnly;
            }
        }

        public static int ToLength(this HidDevices.CoolitRegisterData.ModernRegisterAddress address)
        {
            switch (address)
            {

                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FirmwareVersion:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.TSCurrentTemperature:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.LEDManualTemperature:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanTargetRPM:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanManualTemperature:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanCurrentRPM:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanMaxRPM:
                    return 2;
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.LEDCurrentColor:
                    return 3;
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.LEDTMTemperatures:
                    return 6;
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.ProductName:
                    return 8;
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.LEDTMColors:
                    return 9;
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanRPMTable:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanTemperatureTable:
                    return 10;
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.LEDCycleColors:
                    return 12;
                #region defaultCases
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.DeviceID:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.CurrentLED:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.LEDMode:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.CurrentTemperatureSensor:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.CurrentFan:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanMode:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.FanTargetPWM:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.Status:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.NumberOfLEDChannels:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.NumberOfTemperatureSensors:
                case HidDevices.CoolitRegisterData.ModernRegisterAddress.NumberOfFans:
                #endregion defaultCases
                default: return 1;
            }
        }

        public static RegisterAccess ToAccess(this HidDevices.CoolitRegisterData.BridgeRegisterAddress address)
        {
            return RegisterAccess.ReadOnly;
        }

        public static int ToLength(this HidDevices.CoolitRegisterData.BridgeRegisterAddress address)
        {
            switch (address)
            {
                case HidDevices.CoolitRegisterData.BridgeRegisterAddress.DeviceAddress:
                    return 8;
                case HidDevices.CoolitRegisterData.BridgeRegisterAddress.UDID:
                default:
                    return 4;
            }
        }

        public static RegisterAccess ToAccess(this HidDevices.CoolitRegisterData.OldRegisterAddress address)
        {
            switch (address)
            {
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FirmwareVersion:

                case HidDevices.CoolitRegisterData.OldRegisterAddress.CurrentTemperature1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.CurrentTemperature2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.CurrentTemperature3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.CurrentTemperature4:

                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanCurrentRPM1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanCurrentRPM2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanCurrentRPM3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanCurrentRPM4:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanCurrentRPM5:
                    return RegisterAccess.ReadOnly;
                #region defaultCases
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanManualTemperature1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanManualTemperature2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanManualTemperature3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanManualTemperature4:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanManualTemperature5:

                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanMode1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanMode2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanMode3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanMode4:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanMode5:

                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetPWM1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetPWM2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetPWM3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetPWM4:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetPWM5:

                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetRPM1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetRPM2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetRPM3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetRPM4:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetRPM5:

                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanRPMTable1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanRPMTable2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanRPMTable3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanRPMTable4:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanRPMTable5:

                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTemperatureTable1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTemperatureTable2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTemperatureTable3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTemperatureTable4:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTemperatureTable5:
                #endregion defaultCases
                default:
                    return RegisterAccess.ReadWrite;
            }
        }

        public static int ToLength(this HidDevices.CoolitRegisterData.OldRegisterAddress address)
        {
            switch (address)
            {
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FirmwareVersion:

                case HidDevices.CoolitRegisterData.OldRegisterAddress.CurrentTemperature1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.CurrentTemperature2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.CurrentTemperature3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.CurrentTemperature4:

                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanCurrentRPM1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanCurrentRPM2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanCurrentRPM3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanCurrentRPM4:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanCurrentRPM5:

                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanManualTemperature1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanManualTemperature2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanManualTemperature3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanManualTemperature4:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanManualTemperature5:

                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanMode1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanMode2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanMode3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanMode4:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanMode5:

                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetPWM1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetPWM2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetPWM3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetPWM4:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetPWM5:

                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetRPM1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetRPM2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetRPM3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetRPM4:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTargetRPM5:
                    return 2;

                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanRPMTable1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanRPMTable2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanRPMTable3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanRPMTable4:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanRPMTable5:

                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTemperatureTable1:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTemperatureTable2:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTemperatureTable3:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTemperatureTable4:
                case HidDevices.CoolitRegisterData.OldRegisterAddress.FanTemperatureTable5:
                default:
                    return 10;
            }
        }

        public static RegisterAccess ToAccess(this HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress address)
        {
            switch (address)
            {
                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.FirmwareVersion:
                    return RegisterAccess.ReadOnly;

                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.ManualTemperature1:
                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.ManualTemperature2:

                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.CycleMode1:
                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.CycleMode2:

                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.CycleColors1:
                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.CycleColors2:

                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.TemperaturesAndColors1:
                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.TemperaturesAndColors2:
                default:
                    return RegisterAccess.ReadWrite;
            }
        }

        public static int ToLength(this HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress address)
        {
            switch (address)
            {
                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.FirmwareVersion:
                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.ManualTemperature1:
                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.ManualTemperature2:
                    return 2;

                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.CycleMode1:
                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.CycleMode2:
                    return 1;

                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.CycleColors1:
                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.CycleColors2:
                    return 16;

                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.TemperaturesAndColors1:
                case HidDevices.CoolitRegisterData.OldLightingNodeRegisterAddress.TemperaturesAndColors2:
                default:
                    return 15;
            }
        }
    }
}
