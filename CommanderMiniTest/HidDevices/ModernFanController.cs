using HumanInterfaceDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    public class ModernFanController
    {
        private CoolitModernOutputReportFactory reportFactory;
        private CommandStateMachine cmd;

        public ModernFanController(HidDevice hidDevice, byte channel)
        {
            reportFactory = new CoolitModernOutputReportFactory(hidDevice) { Channel = channel };
            cmd = new CommandStateMachine(hidDevice, reportFactory, new CoolitBridgeResponseValidator());
        }

        public async Task<byte> GetFanMode(CoolitSensorAddress fanAddress)
        {
            await DoSetCurrentFan(fanAddress);
            await cmd.Run((byte)CoolitModernCommandCode.FAN_MODE_GET);

            if (cmd.IsFaulted)
            {
                return 0;
            }

            byte mode = ByteParser.ParseResponse(cmd.Result);
            return (byte)(mode & 0x8E); // high bit is set if fan detected, bits 3..1 contain fan mode. Low bit is set when the fan is 4-pin, ignore it.
        }

        private async Task DoSetCurrentFan(CoolitSensorAddress fanAddress)
        {
            reportFactory.CurrentSensor = (byte)fanAddress;
            await cmd.Run((byte)CoolitModernCommandCode.CURRENT_FAN);
        }
    }
}
