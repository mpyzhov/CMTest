using CorsairLink4.Common.DevicesDefinitions.Coolit;
using CorsairLink4.Common.Shared.Utils;
using CorsairLink4.Module.HidDevices.Coolit.CommandCodes;
using CorsairLink4.Module.HidDevices.Coolit.ReportFactories;
using CorsairLink4.Module.HidDevices.Coolit.ResponseParsing;
using CorsairLink4.Module.HidDevices.Coolit.ResponseValidation;
using CorsairLink4.Module.HidDevices.Core;
using CorsairLink4.Public.Synchronization;
using HumanInterfaceDevice;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit
{
    public class OldWorldLedController : ILedController
    {
        private OldLightingNodeOutputReportFactory reportFactory;
        private CommandStateMachine cmd;

        public OldWorldLedController(HidDevice hidDevice, byte channel)
        {
            reportFactory = new OldLightingNodeOutputReportFactory(hidDevice);
            reportFactory.Channel = channel;
            cmd = new CommandStateMachine(hidDevice, reportFactory, new CoolitBridgeResponseValidator());
        }

        public async Task SetLedColorsAndTemperatures(CoolitSensorAddress ledAddress, byte r1, byte g1, byte b1, byte r2, byte g2, byte b2, byte r3, byte g3, byte b3, double t1, double t2, double t3)
        {
            DoSetCurrentLed(ledAddress);

            byte[] t1b = CoolitDataConverter.TemperatureToWordBytes(t1);
            byte[] t2b = CoolitDataConverter.TemperatureToWordBytes(t2);
            byte[] t3b = CoolitDataConverter.TemperatureToWordBytes(t3);

            List<byte> payload = new List<byte>();

            payload.AddRange(t1b);
            payload.Add(r1);
            payload.Add(g1);
            payload.Add(b1);

            payload.AddRange(t2b);
            payload.Add(r2);
            payload.Add(g2);
            payload.Add(b2);

            payload.AddRange(t3b);
            payload.Add(r3);
            payload.Add(g3);
            payload.Add(b3);

            reportFactory.Payload = payload.ToArray();
            using (var lck = new CorsairDevicesGuardLock())
            {
                await cmd.Run((byte)LedNodeCommandOpCode.SET_COLORS_AND_TEMPERATURES);
            }
        }

        public async Task SetLedCycleColors(CoolitSensorAddress ledAddress, byte r1, byte g1, byte b1, byte r2, byte g2, byte b2, byte r3, byte g3, byte b3, byte r4, byte g4, byte b4)
        {
            DoSetCurrentLed(ledAddress);
            reportFactory.Payload = new byte[16] { r1, g1, b1, 0, r2, g2, b2, 0, r3, g3, b3, 0, r4, g4, b4, 0 };
            using (var lck = new CorsairDevicesGuardLock())
            {
                await cmd.Run((byte)LedNodeCommandOpCode.SET_COLORS);
            }
        }

        public async Task SetLedManualTemperature(CoolitSensorAddress ledAddress, double temperature)
        {
            DoSetCurrentLed(ledAddress);
            reportFactory.ManualTemperature = CoolitDataConverter.TemperatureToWord(temperature);
            using (var lck = new CorsairDevicesGuardLock())
            {
                await cmd.Run((byte)LedNodeCommandOpCode.SET_MANUAL_TEMPERATURE);
            }
        }

        public async Task SetLedMode(CoolitSensorAddress ledAddress, CoolitLEDMode mode, byte tempChannel = 0xFF)
        {
            DoSetCurrentLed(ledAddress);
            reportFactory.LedMode = (byte)mode;
            using (var lck = new CorsairDevicesGuardLock())
            {
                await cmd.Run((byte)LedNodeCommandOpCode.SET_MODE);
            }
        }

        public async Task<CoolitLEDMode> GetLedMode(CoolitSensorAddress ledAddress)
        {
            DoSetCurrentLed(ledAddress);
            using (var lck = new CorsairDevicesGuardLock())
            {
                await cmd.Run((byte)LedNodeCommandOpCode.GET_MODE);
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

        public void DoSetCurrentLed(CoolitSensorAddress ledAddress)
        {
            reportFactory.CurrentLed = (byte)ledAddress;
        }
    }
}
