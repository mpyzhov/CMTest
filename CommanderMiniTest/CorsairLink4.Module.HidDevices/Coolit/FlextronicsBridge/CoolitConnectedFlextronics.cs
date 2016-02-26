using CorsairLink4.Common.DevicesDefinitions.Common;
using CorsairLink4.Common.DevicesDefinitions.Flextronics;
using CorsairLink4.Common.Shared.Communication;
using CorsairLink4.Common.Shared.DevicesData;
using CorsairLink4.Common.Shared.Utils;
using CorsairLink4.Devices.Flextronics.Core;
using CorsairLink4.Devices.Flextronics.Flextronics;
using HumanInterfaceDevice;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit.FlextronicsBridge
{
    internal class CoolitConnectedFlextronics : ICoolitConnectedDevice
    {
        private const string SerialPrefix = "coolit";
        private readonly List<DeviceCommunicationData> flextronicsCommunicationData = new List<DeviceCommunicationData>();
        private readonly FlextronicsBridgeCommunicationHandle communicationHandle;
        private readonly string serial;

        public CoolitConnectedFlextronics(HidDevice hidDevice, byte channel, string parentSerial)
        {
            communicationHandle = new FlextronicsBridgeCommunicationHandle(hidDevice, channel);
            serial = string.Concat(SerialPrefix, parentSerial, channel);
        }

        public DeviceInstance InstaceKey
        {
            get
            {
                return new DeviceInstance(FlextronicsDeviceDefinition.DefaultDistinguishInfo, serial);
            }
        }

        public async Task UpdateProperties()
        {
            flextronicsCommunicationData.Clear();

            FlextronicsCommunicationDataHandler communicationDataHandler = await CreateCommunicationHandler();
            if (communicationDataHandler != null)
            {
                var collector = new FlextronicsDataCollectorVisitor();
                try
                {
                    await communicationDataHandler.Visit(collector);
                    flextronicsCommunicationData.AddRange(collector.GetCollectedData());
                }
                catch (Exception)
                {
                    // bad practice, I know
                }
            }
        }

        public Task UpdateInfo(Common.Shared.Settings.IGroupsService groupsService)
        {
            return CompletedTask.Empty;
        }

        public Task Accept(ISensorVisitor visitor)
        {
            foreach (var data in flextronicsCommunicationData)
            {
                visitor.Visit(data);
            }

            return CompletedTask.Empty;
        }

        public async Task ProcessControlData(DeviceCommunicationData data)
        {
            if (data.DeviceInfo.Equals(InstaceKey))
            {
                FlextronicsCommunicationDataHandler communicationDataHandler = await CreateCommunicationHandler();
                if (communicationDataHandler != null)
                {
                    await communicationDataHandler.ProcessControlData(data);
                }
            }
        }

        public async Task SetAllFans100Percents()
        {
            FlextronicsCommunicationDataHandler communicationDataHandler = await CreateCommunicationHandler();
            if (communicationDataHandler != null)
            {
                await communicationDataHandler.SetAllFans100Percents();
            }
        }

        public Task SetAllLeds(Common.Shared.Settings.RgbColor color)
        {
            return CompletedTask.Empty;
        }

        private async Task<FlextronicsCommunicationDataHandler> CreateCommunicationHandler()
        {
            try
            {
                var flextronicsDevice = await FlextronicsDevice.Create(communicationHandle);
                return new FlextronicsCommunicationDataHandler(flextronicsDevice, InstaceKey);
            }
            catch (FlextronicsNotSupportedDeviceException)
            {
            }

            return null;
        }

        private class FlextronicsDataCollectorVisitor : ISensorVisitor
        {
            private readonly List<DeviceCommunicationData> data = new List<DeviceCommunicationData>();

            public void Visit(DeviceCommunicationData communicationData)
            {
                data.Add(communicationData);
            }

            public IEnumerable<DeviceCommunicationData> GetCollectedData()
            {
                return data.ToArray();
            }
        }
    }
}
