using CorsairLink4.Common.DevicesDefinitions.Coolit;
using CorsairLink4.Module.HidDevices.Coolit.CommandCodes;
using CorsairLink4.Module.HidDevices.Coolit.ReportFactories;
using CorsairLink4.Module.HidDevices.Coolit.ResponseParsing;
using CorsairLink4.Module.HidDevices.Coolit.ResponseValidation;
using CorsairLink4.Module.HidDevices.Core;
using CorsairLink4.Public.Synchronization;
using HumanInterfaceDevice;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit
{
    public class ModernLedController : ILedController
    {
        private CoolitModernOutputReportFactory reportFactory;
        private CommandStateMachine cmd;

        public ModernLedController(HidDevice hidDevice, byte channel)
        {
            reportFactory = new CoolitModernOutputReportFactory(hidDevice) { Channel = channel };
            cmd = new CommandStateMachine(hidDevice, reportFactory, new CoolitBridgeResponseValidator());
        }

        public async Task SetLedColorsAndTemperatures(CoolitSensorAddress ledAddress, byte r1, byte g1, byte b1, byte r2, byte g2, byte b2, byte r3, byte g3, byte b3, double t1, double t2, double t3)
        {
            byte[] t1b = CoolitDataConverter.TemperatureToWordBytes(t1);
            byte[] t2b = CoolitDataConverter.TemperatureToWordBytes(t2);
            byte[] t3b = CoolitDataConverter.TemperatureToWordBytes(t3);

            reportFactory.Payload = new byte[] { r1, g1, b1, r2, g2, b2, r3, g3, b3 };

            using (var lck = new CorsairDevicesGuardLock())
            {
                await DoSetCurrentLed(ledAddress);
                await cmd.Run((byte)CoolitModernCommandCode.LED_TEMPERATURE_MODE_COLORS);
                reportFactory.Payload = t1b.Concat(t2b).Concat(t3b).ToArray();
                await cmd.Run((byte)CoolitModernCommandCode.LED_TEMPERATURE_MODE_TEMPERATURES);
            }
        }

        public async Task SetLedCycleColors(CoolitSensorAddress ledAddress, byte r1, byte g1, byte b1, byte r2, byte g2, byte b2, byte r3, byte g3, byte b3, byte r4, byte g4, byte b4)
        {
            reportFactory.Payload = new byte[] { r1, g1, b1, r2, g2, b2, r3, g3, b3, r4, g4, b4 };
            using (var lck = new CorsairDevicesGuardLock())
            {
                await DoSetCurrentLed(ledAddress);
                await cmd.Run((byte)CoolitModernCommandCode.LED_CYCLE_COLORS);
            }
        }

        public async Task SetLedManualTemperature(CoolitSensorAddress ledAddress, double temperature)
        {
            reportFactory.LedManualTemperature = CoolitDataConverter.TemperatureToWord(temperature);
            using (var lck = new CorsairDevicesGuardLock())
            {
                await DoSetCurrentLed(ledAddress);
                await cmd.Run((byte)CoolitModernCommandCode.LED_MANUAL_TEMPERATURE);
            }
        }


        public async Task SetLedMode(CoolitSensorAddress ledAddress, CoolitLEDMode mode, byte tempChannel = 0xFF)
        {
            if (!(mode != CoolitLEDMode.TemperatureBased || tempChannel < 8))
            {
                throw new ArgumentException("Specify valid temp channel for temperature controlled mode");
            }

            byte modeAndChannel = (byte)mode;
            if (mode == CoolitLEDMode.TemperatureBased)
            {
                modeAndChannel |= tempChannel; // already checked that it's < 8
            }

            reportFactory.LedMode = (byte)modeAndChannel;
            using (var lck = new CorsairDevicesGuardLock())
            {
                await DoSetCurrentLed(ledAddress);
                await cmd.Run((byte)CoolitModernCommandCode.LED_MODE_SET);
            }
        }

        public async Task<CoolitLEDMode> GetLedMode(CoolitSensorAddress ledAddress)
        {
            using (var lck = new CorsairDevicesGuardLock())
            {
                await DoSetCurrentLed(ledAddress);
                await cmd.Run((byte)CoolitModernCommandCode.LED_MODE_GET);
            }

            if (cmd.IsFaulted)
            {
                return 0;
            }

            byte mode = ByteParser.ParseResponse(cmd.Result);
            if ((mode & 0xF0) == 0xC0)
            {
                mode &= 0xF0;
            }

            return (CoolitLEDMode)mode;
        }

        private async Task DoSetCurrentLed(CoolitSensorAddress ledAddress)
        {
            reportFactory.CurrentSensor = (byte)ledAddress;
            await cmd.Run((byte)CoolitModernCommandCode.CURRENT_LED);
        }
    }
}
