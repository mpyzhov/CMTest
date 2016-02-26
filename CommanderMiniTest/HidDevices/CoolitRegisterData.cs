using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    public class CoolitRegisterData
    {
        public enum ModernRegisterAddress
        {
            DeviceID = 0x00,
            FirmwareVersion = 0x01,
            ProductName = 0x02,
            Status = 0x03,
            CurrentLED = 0x04,
            NumberOfLEDChannels = 0x05,

            LEDMode = 0x06,
            LEDCurrentColor = 0x07,
            LEDManualTemperature = 0x08,
            LEDTMTemperatures = 0x09,
            LEDTMColors = 0x0A,
            LEDCycleColors = 0x0B,

            CurrentTemperatureSensor = 0x0C,
            NumberOfTemperatureSensors = 0x0D,
            TSCurrentTemperature = 0x0E,
            //TemperatureThreshold, // DK: not used

            CurrentFan = 0x10,
            NumberOfFans = 0x11,
            FanMode = 0x12,
            FanTargetPWM = 0x13,
            FanTargetRPM = 0x14,
            FanManualTemperature = 0x15,
            FanCurrentRPM = 0x16,
            FanMaxRPM = 0x17,
            // FanThreshold, // DK: not used
            FanRPMTable = 0x19,
            FanTemperatureTable = 0x1A,
        };

        public enum BridgeRegisterAddress
        {
            // commented legacy bridge registers
            //IICSpeed = 0x00,
            //WriteTimeout = 0x10,
            //ReadTimeout = 0x20,
            //SmbTimeout = 0x30,
            DeviceAddress = 0x40,
            //AfpEnable = 0x50,
            //AdcSample = 0x60,
            //ChannelState = 0x70,
            //AlertState = 0x80,
            //SystemUptime = 0x90,
            UDID = 0xA0,
            //SmbEnable = 0xB0,
        };

        public enum OldRegisterAddress
        {
            DeviceId = 0x00,
            FirmwareVersion = 0x01,

            CurrentTemperature1 = 0x07,
            CurrentTemperature2 = 0x08,
            CurrentTemperature3 = 0x09,
            CurrentTemperature4 = 0x0A,

            FanCurrentRPM1 = 0x0B,
            FanCurrentRPM2 = 0x0C,
            FanCurrentRPM3 = 0x0D,
            FanCurrentRPM4 = 0x0E,
            FanCurrentRPM5 = 0x0F,

            FanManualTemperature1 = 0x1A,
            FanManualTemperature2 = 0x1B,
            FanManualTemperature3 = 0x1C,
            FanManualTemperature4 = 0x1D,
            FanManualTemperature5 = 0x1E,

            FanMode1 = 0x20,
            FanMode2 = 0x30,
            FanMode3 = 0x40,
            FanMode4 = 0x50,
            FanMode5 = 0x60,

            FanTargetPWM1 = 0x21,
            FanTargetPWM2 = 0x31,
            FanTargetPWM3 = 0x41,
            FanTargetPWM4 = 0x51,
            FanTargetPWM5 = 0x61,

            FanTargetRPM1 = 0x22,
            FanTargetRPM2 = 0x32,
            FanTargetRPM3 = 0x42,
            FanTargetRPM4 = 0x52,
            FanTargetRPM5 = 0x62,

            FanRPMTable1 = 0x23,
            FanRPMTable2 = 0x33,
            FanRPMTable3 = 0x43,
            FanRPMTable4 = 0x53,
            FanRPMTable5 = 0x63,

            FanTemperatureTable1 = 0x28,
            FanTemperatureTable2 = 0x38,
            FanTemperatureTable3 = 0x48,
            FanTemperatureTable4 = 0x58,
            FanTemperatureTable5 = 0x68,
        }

        public enum OldLightingNodeRegisterAddress
        {
            DeviceId = 0x00,
            FirmwareVersion = 0x01,

            RgbOut = 0x06,

            ManualTemperature1 = 0x0C,
            ManualTemperature2 = 0x0E,

            CycleMode1 = 0x10,
            CycleMode2 = 0x30,

            CycleColors1 = 0x20,
            CycleColors2 = 0x40,

            TemperaturesAndColors1 = 0x11,
            TemperaturesAndColors2 = 0x31,
        }

        public CoolitRegisterData(RegisterAccess access, int length, byte address)
        {
            Access = access;
            Length = length;
            AddressByteRepresentation = address;
        }

        public RegisterAccess Access { get; private set; }

        public int Length { get; private set; }

        public byte AddressByteRepresentation { get; private set; }
    }

    public enum RegisterAccess
    {
        ReadOnly,
        ReadWrite,
    };
}
