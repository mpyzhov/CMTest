using CorsairLink4.Common.Shared.Utils;
using CorsairLink4.Devices.Flextronics.Core;
using CorsairLink4.Devices.Flextronics.Interface;
using CorsairLink4.Module.HidDevices.Coolit.ReportFactories;
using CorsairLink4.Module.HidDevices.Coolit.ResponseParsing;
using CorsairLink4.Module.HidDevices.Coolit.ResponseValidation;
using CorsairLink4.Module.HidDevices.Core;
using HumanInterfaceDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit.FlextronicsBridge
{
    internal class FlextronicsBridgeCommunicationHandle : ICommunicationHandle
    {
        private readonly HidDevice hidDevice;
        private readonly byte channel;

        public FlextronicsBridgeCommunicationHandle(HidDevice device, byte address)
        {
            hidDevice = device;
            channel = address;
        }

        public async Task<byte[]> ReadData(FlextronicsCommandCode command, int length)
        {
            var data = await RunCommand(BridgeProxyOutputReportFactory.CreateReadBlockFactory(hidDevice, channel, length), command);
            return BlockParser.ParseResponse(data);
        }

        public async Task WriteData(FlextronicsCommandCode command, byte[] data)
        {
            await RunCommand(BridgeProxyOutputReportFactory.CreateWriteBlockFactory(hidDevice, channel, data), command);
        }

        public Task<string> ReadVersion()
        {
            return Task.FromResult("");
        }

        public void Dispose()
        {
            hidDevice.Dispose();
        }

        private async Task<byte[]> RunCommand(IReportFactory factory, FlextronicsCommandCode command)
        {
            var cmd = new CommandStateMachine(hidDevice, factory, new CoolitBridgeResponseValidator());
            await cmd.Run((byte)command);
            return cmd.Result;
        }
    } 
}
