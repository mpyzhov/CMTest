using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorsairLink4.Common.DevicesDefinitions.Common;
using CorsairLink4.Common.DevicesDefinitions.Robbins;
using CorsairLink4.Common.Shared.Communication;
using CorsairLink4.Common.Shared.DataSerialization.Robbins;
using CorsairLink4.Common.Shared.DevicesData;
using CorsairLink4.Common.Shared.Utils;

namespace CorsairLink4.Module.HidDevices.Mocks
{
    internal class RobbinsPsuDeviceMock : IDeviceComponent
    {
        private readonly DeviceCommunicationDataFactory dataCreator;
        private readonly RobbinsPsuPathTokenParser tokenParser = RobbinsPsuPathTokenParser.Instance;

        private double current = 10;
        private double rpm = 2243;

        public RobbinsPsuDeviceMock(DeviceInstance instance)
        {
            InstanceKey = instance;
            dataCreator = new DeviceCommunicationDataFactory(instance, tokenParser);
        }

        public DeviceInstance InstanceKey { get; private set; }
        public Task Accept(ISensorVisitor visitor)
        {
            DoVisit(visitor);
            return CompletedTask.Empty;
        }

        private void DoVisit(ISensorVisitor visitor)
        {
            foreach (var sens in EnumerateSensors())
                visitor.Visit(sens);
        }

        private IEnumerable<DeviceCommunicationData> EnumerateSensors()
        {
            yield return dataCreator.CreateData(DeviationGenerator.Generate(current), RobbinsPsuSensorType.Current);
            yield return dataCreator.CreateData(DeviationGenerator.Generate(rpm), RobbinsPsuSensorType.FanRpm);
        }
    }
}
