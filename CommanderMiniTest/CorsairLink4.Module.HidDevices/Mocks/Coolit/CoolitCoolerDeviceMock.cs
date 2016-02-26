using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorsairLink4.Common.DevicesDefinitions.Common;
using CorsairLink4.Common.DevicesDefinitions.Coolit;
using CorsairLink4.Common.Shared.Communication;
using CorsairLink4.Common.Shared.DataSerialization.Coolit;
using CorsairLink4.Common.Shared.DevicesData;
using CorsairLink4.Common.Shared.Utils;
using CorsairLink4.Module.HidDevices.Coolit;

namespace CorsairLink4.Module.HidDevices.Mocks.Coolit
{
    internal class CoolitCoolerDeviceMock : IDeviceComponent
    {
        private readonly DeviceCommunicationDataFactory factory;
        
        public DeviceInstance Instance { get; private set; }

        public CoolitModel Model { get; private set; }

        public CoolitCoolerDeviceMock(DeviceInstance instance, CoolitModel model)
        {
            Model = model;
            Instance = instance;
            this.factory = new DeviceCommunicationDataFactory(Instance, CoolitPathTokenParser.Instance);
        }

        public Task Accept(ISensorVisitor visitor)
        {
            foreach (var data in EnumerateSensors())
            {
                visitor.Visit(data);
            }

            return CompletedTask.Empty;
        }

        public static DeviceDistinguishInfo GetInfo(CoolitModel model)
        {
            return new DeviceDistinguishInfo(CoolitUtils.GetPid(model), DeviceIdentity.Coolit.Vid);
        }

        private IEnumerable<DeviceCommunicationData> EnumerateSensors()
        {
            return CoolitSensorsEnumerators.EnumerateSensors(Model, factory, Instance);
        }

        private static class CoolitSensorsEnumerators 
        {
            private const double temperature = 42.0F;
            private const int pump = 1444;
            private const int fan = 1000;

            public static IEnumerable<DeviceCommunicationData> EnumerateSensors(CoolitModel model,
                DeviceCommunicationDataFactory factory,
                DeviceInstance instance)
            {
                yield return factory.CreateData("0.1.1.1", CoolitSensorType.FirmwareVersion);
                switch (model)
                {
                    case CoolitModel.H100i:
                        yield return factory.CreateData((double)DeviationGenerator.Generate(temperature, 0.5), CoolitSensorType.Temperature);
                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port0),
                                BitConverter.GetBytes(fan));
                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port1),
                                BitConverter.GetBytes(fan));
                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port2),
                                BitConverter.GetBytes(fan));
                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port3),
                                BitConverter.GetBytes(fan));
                        yield return new DeviceCommunicationData(instance, 
                            CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.PumpRpm), BitConverter.GetBytes(pump));
                        break;
                    case CoolitModel.H110iGT:
                    case CoolitModel.H110i:
                    case CoolitModel.H80i:
                        yield return factory.CreateData((double)DeviationGenerator.Generate(temperature, 0.5), CoolitSensorType.Temperature);
                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port0),
                                BitConverter.GetBytes(fan));
                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port1),
                                BitConverter.GetBytes(fan));
                        yield return new DeviceCommunicationData(instance,
                            CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.PumpRpm), BitConverter.GetBytes(pump));
                        break;
                    case CoolitModel.Whiptail:
                        // 1-6 fans, 1-4 temps
                        
                        yield return factory.CreateData((double)DeviationGenerator.Generate(25f, 0.2),
                            CoolitSensorType.Temperature, CoolitSensorAddress.Port0);
                        
                        yield return factory.CreateData((double)DeviationGenerator.Generate(temperature, 0.5),
                            CoolitSensorType.Temperature, CoolitSensorAddress.Port1);
                        yield return factory.CreateData((double)DeviationGenerator.Generate(temperature, 0.5),
                            CoolitSensorType.Temperature, CoolitSensorAddress.Port2);
                        yield return factory.CreateData((double)DeviationGenerator.Generate(temperature, 0.5),
                            CoolitSensorType.Temperature, CoolitSensorAddress.Port3);

                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port0),
                                BitConverter.GetBytes(fan));
                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port1),
                                BitConverter.GetBytes(fan));
                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port2),
                                BitConverter.GetBytes(fan));
                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port3),
                                BitConverter.GetBytes(fan));

                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port4),
                                BitConverter.GetBytes(fan));

                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port5),
                                BitConverter.GetBytes(fan));
                        break;
                    case CoolitModel.LightingNode:
                        yield return factory.CreateData("0.1.1.1", CoolitSensorType.FirmwareVersion);
                        //yield return factory.CreateData(0, CoolitSensorType.Led, CoolitSensorAddress.Port0);
                        //yield return factory.CreateData(0, CoolitSensorType.Led, CoolitSensorAddress.Port1);
                        break;
                    case CoolitModel.CoolingNode:
                        // 1-5 fans, 1-4 temps

                        yield return factory.CreateData((double)DeviationGenerator.Generate(temperature, 0.5),
                            CoolitSensorType.Temperature, CoolitSensorAddress.Port0);

                        yield return factory.CreateData((double)DeviationGenerator.Generate(temperature, 0.5),
                            CoolitSensorType.Temperature, CoolitSensorAddress.Port1);
                        yield return factory.CreateData((double)DeviationGenerator.Generate(temperature, 0.5),
                            CoolitSensorType.Temperature, CoolitSensorAddress.Port2);
                        yield return factory.CreateData((double)DeviationGenerator.Generate(temperature, 0.5),
                            CoolitSensorType.Temperature, CoolitSensorAddress.Port3);

                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port0),
                                BitConverter.GetBytes(fan));
                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port1),
                                BitConverter.GetBytes(fan));
                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port2),
                                BitConverter.GetBytes(fan));
                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port3),
                                BitConverter.GetBytes(fan));

                        yield return
                            new DeviceCommunicationData(instance,
                                CoolitPathTokenParser.Instance.CreateToken(CoolitSensorType.FanRpm, CoolitSensorAddress.Port4),
                                BitConverter.GetBytes(fan));
                        break;
                }
            }
        }
    }
}
