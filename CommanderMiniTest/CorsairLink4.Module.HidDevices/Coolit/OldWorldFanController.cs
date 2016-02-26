using CorsairLink4.Common.DevicesDefinitions.Coolit;
using CorsairLink4.Common.Shared.Utils;
using CorsairLink4.Module.HidDevices.Coolit.CommandCodes;
using CorsairLink4.Module.HidDevices.Coolit.ReportFactories;
using CorsairLink4.Module.HidDevices.Coolit.ResponseParsing;
using CorsairLink4.Module.HidDevices.Coolit.ResponseValidation;
using CorsairLink4.Module.HidDevices.Core;
using CorsairLink4.Public.Synchronization;
using HumanInterfaceDevice;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit
{
    public class OldWorldFanController : IFanController
    {
        private CoolitOldOutputReportFactory reportFactory;
        private CommandStateMachine cmd;

        public OldWorldFanController(HidDevice hidDevice, byte channel)
        {
            reportFactory = new CoolitOldOutputReportFactory(hidDevice);
            reportFactory.Channel = channel;
            cmd = new CommandStateMachine(hidDevice, reportFactory, new CoolitBridgeResponseValidator());
        }

        public async Task<byte> GetFanMode(CoolitSensorAddress fanAddress)
        {
            DoSetCurrentFan(fanAddress);
            using (var lck = new CorsairDevicesGuardLock())
            {
                await cmd.Run((byte)CoolitOldCommandCode.FAN_MODE_GET);
            }

            if (cmd.IsFaulted)
            {
                return 0;
            }

            byte mode = ByteParser.ParseResponse(cmd.Result);
            return (byte)(mode & 0x8E); // high bit is set if fan detected, bits 3..1 contain fan mode. Low bit is set when the fan is 4-pin, ignore it.
        }

        public async Task SetFanMode(CoolitSensorAddress fanAddress, CoolitFanMode fanMode, byte tempChannel = 0xFF)
        {
            if ((fanMode == CoolitFanMode.Default ||
                fanMode == CoolitFanMode.Quiet ||
                fanMode == CoolitFanMode.Balanced ||
                fanMode == CoolitFanMode.Performance ||
                fanMode == CoolitFanMode.Custom) &&
                tempChannel > 7)
            {
                throw new ArgumentException("Specify valid temp channel (<=7) for temperature controlled mode");
            }

            byte modeAndChannel = (byte)fanMode;
            if (fanMode == CoolitFanMode.Default ||
                fanMode == CoolitFanMode.Quiet ||
                fanMode == CoolitFanMode.Balanced ||
                fanMode == CoolitFanMode.Performance ||
                fanMode == CoolitFanMode.Custom)
            {
                modeAndChannel |= (byte)(tempChannel << 4); // already checked that it's < 8
            }

            DoSetCurrentFan(fanAddress);
            reportFactory.FanMode = (byte)modeAndChannel;

            using (var lck = new CorsairDevicesGuardLock())
            {
                await cmd.Run((byte)CoolitOldCommandCode.FAN_MODE_SET);
            }
        }

        public async Task SetFanPWM(CoolitSensorAddress fanAddress, byte pwm)
        {
            DoSetCurrentFan(fanAddress);

            reportFactory.FanPWM = (byte)(Math.Min(pwm / 100.0, 1.0) * 255); // coolit pwm range is 0..255

            using (var lck = new CorsairDevicesGuardLock())
            {
                await cmd.Run((byte)CoolitOldCommandCode.FAN_PWM);
            }
        }

        public async Task SetFanRPM(CoolitSensorAddress fanAddress, short rpm)
        {
            DoSetCurrentFan(fanAddress);
            reportFactory.FanRPM = rpm;

            using (var lck = new CorsairDevicesGuardLock())
            {
                await cmd.Run((byte)CoolitOldCommandCode.FAN_RPM);
            }
        }

        public async Task SetFanManualTemperature(CoolitSensorAddress fanAddress, double temperature)
        {
            DoSetCurrentFan(fanAddress);
            reportFactory.FanManualTemperature = CoolitDataConverter.TemperatureToWord(temperature);

            using (var lck = new CorsairDevicesGuardLock())
            {
                await cmd.Run((byte)CoolitOldCommandCode.FAN_MANUAL_TEMPERATURE);
            }
        }

        public async Task SetCustomCurve(CoolitSensorAddress fanAddress, short rpm1, short rpm2, short rpm3, short rpm4, short rpm5, double temp1, double temp2, double temp3, double temp4, double temp5)
        {
            DoSetCurrentFan(fanAddress);

            var payloadRpm = new List<byte>();
            payloadRpm.AddRange(BitConverter.GetBytes(rpm1));
            payloadRpm.AddRange(BitConverter.GetBytes(rpm2));
            payloadRpm.AddRange(BitConverter.GetBytes(rpm3));
            payloadRpm.AddRange(BitConverter.GetBytes(rpm4));
            payloadRpm.AddRange(BitConverter.GetBytes(rpm5));

            var payloadTemp = new List<byte>();
            payloadTemp.AddRange(CoolitDataConverter.TemperatureToWordBytes(temp1));
            payloadTemp.AddRange(CoolitDataConverter.TemperatureToWordBytes(temp2));
            payloadTemp.AddRange(CoolitDataConverter.TemperatureToWordBytes(temp3));
            payloadTemp.AddRange(CoolitDataConverter.TemperatureToWordBytes(temp4));
            payloadTemp.AddRange(CoolitDataConverter.TemperatureToWordBytes(temp5));

            using (var lck = new CorsairDevicesGuardLock())
            {
                reportFactory.Payload = payloadRpm.ToArray();
                await cmd.Run((byte)CoolitOldCommandCode.FAN_RPM_TABLE);
                reportFactory.Payload = payloadTemp.ToArray();
                await cmd.Run((byte)CoolitOldCommandCode.FAN_TEMPERATURE_TABLE);
            }
        }

        private void DoSetCurrentFan(CoolitSensorAddress fanAddress)
        {
            reportFactory.CurrentSensor = (byte)fanAddress;
        }
    }
}
