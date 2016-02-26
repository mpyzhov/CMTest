using CorsairLink4.Common.DevicesDefinitions.Common;
using CorsairLink4.Common.DevicesDefinitions.Coolit;
using CorsairLink4.Common.DevicesDefinitions.Flextronics;
using CorsairLink4.Common.Shared.Communication;
using CorsairLink4.Common.Shared.DataSerialization;
using CorsairLink4.Common.Shared.DataSerialization.Coolit;
using CorsairLink4.Common.Shared.DevicesData;
using CorsairLink4.Common.Shared.Settings;
using CorsairLink4.Devices.Flextronics.Core;
using CorsairLink4.Module.HidDevices.Coolit.CommandCodes;
using CorsairLink4.Module.HidDevices.Coolit.FlextronicsBridge;
using CorsairLink4.Module.HidDevices.Coolit.ReportFactories;
using CorsairLink4.Module.HidDevices.Coolit.ResponseParsing;
using CorsairLink4.Module.HidDevices.Coolit.ResponseValidation;
using CorsairLink4.Module.HidDevices.Coolit.Sensors;
using CorsairLink4.Module.HidDevices.Core;
using CorsairLink4.Public.Synchronization;
using HumanInterfaceDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit
{
    public class CoolitDevice : ICoolitConnectedDevice, IDisposable
    {
        private HidDevice deviceEntity;
        private List<ICoolitConnectedDevice> connectedDevices = new List<ICoolitConnectedDevice>();
        private List<CoolitSensor> fanSensors = new List<CoolitSensor>();
        private List<CoolitSensor> temperatureSensors = new List<CoolitSensor>();
        private List<CoolitSensor> ledSensors = new List<CoolitSensor>();

        private CoolitModernOutputReportFactory modernReportFactory;
        private CoolitOldOutputReportFactory oldReportFactory;
        private OldLightingNodeOutputReportFactory lightingNodeReportFactory;
        private CoolitBridgeOutputReportFactory bridgeReportFactory;
        private CoolitDeviceInfo deviceInfo;
        private string fwVersion;
        private IFanController fanController;
        private ILedController ledController;

        private static readonly Dictionary<CoolitModel, CoolitDeviceInfo> DeviceInfoTable = new Dictionary<CoolitModel, CoolitDeviceInfo>()
        {
            { CoolitModel.Unknown, new CoolitDeviceInfo(CoolitModel.Unknown, string.Empty, ProtocolType.Unsupported, false, 0) },
            { CoolitModel.H80, new CoolitDeviceInfo(CoolitModel.H80, DeviceIdentity.Coolit.Pid.H80, ProtocolType.OldWorldCoolingNode, true, 0) },
            { CoolitModel.CoolingNode, new CoolitDeviceInfo(CoolitModel.CoolingNode, DeviceIdentity.Coolit.Pid.CoolingNode, ProtocolType.OldWorldCoolingNode, true, 0) },
            { CoolitModel.LightingNode, new CoolitDeviceInfo(CoolitModel.LightingNode, DeviceIdentity.Coolit.Pid.LightingNode, ProtocolType.OldWorldLightingNode, true, 0) },
            { CoolitModel.H100, new CoolitDeviceInfo(CoolitModel.H100, DeviceIdentity.Coolit.Pid.H100, ProtocolType.OldWorldCoolingNode, true, 0) },
            { CoolitModel.H80i, new CoolitDeviceInfo(CoolitModel.H80i, DeviceIdentity.Coolit.Pid.H80i, ProtocolType.Modern, true, 2) },
            { CoolitModel.H100i, new CoolitDeviceInfo(CoolitModel.H100i, DeviceIdentity.Coolit.Pid.H100i, ProtocolType.Modern, true, 2) },
            { CoolitModel.Whiptail, new CoolitDeviceInfo(CoolitModel.Whiptail, DeviceIdentity.Coolit.Pid.Whiptail, ProtocolType.Modern, true, 5) },
            { CoolitModel.H100iGT, new CoolitDeviceInfo(CoolitModel.H100iGT, DeviceIdentity.Coolit.Pid.H100iGT, ProtocolType.Modern, true, 1) },
            { CoolitModel.H110iGT, new CoolitDeviceInfo(CoolitModel.H110iGT, DeviceIdentity.Coolit.Pid.H110iGT, ProtocolType.Modern, true, 1) },
            { CoolitModel.H110i, new CoolitDeviceInfo(CoolitModel.H110i, DeviceIdentity.Coolit.Pid.H110i, ProtocolType.Modern, true, 1) }
        };

        public CoolitDevice(HidDevice hidDevice, byte channel = 0)
        {
            deviceEntity = hidDevice;
            Channel = channel;
            bridgeReportFactory = new CoolitBridgeOutputReportFactory(deviceEntity) { Channel = channel };
            modernReportFactory = new CoolitModernOutputReportFactory(deviceEntity) { Channel = channel };
            oldReportFactory = new CoolitOldOutputReportFactory(deviceEntity) { Channel = channel };
            lightingNodeReportFactory = new OldLightingNodeOutputReportFactory(deviceEntity) { Channel = channel };
        }

        public string UDID { get; private set; }

        public byte Channel { get; private set; }

        public DeviceInstance InstanceKey
        {
            get
            {
                return new DeviceInstance(deviceInfo.Pid, DeviceIdentity.Coolit.Vid, UDID);
            }
        }

        public HidDevice DeviceEntity
        {
            get
            {
                return deviceEntity;
            }
        }

        private bool IsBridge
        {
            get
            {
                return Channel == 0 && deviceInfo.Bridgeable;
            }
        }

        public async Task<bool> Initialize()
        {
            var cmd = new CommandStateMachine(deviceEntity, bridgeReportFactory, new CoolitBridgeResponseValidator());
            await RunCmdSynchronized(cmd, (byte)CoolitBridgeCommandCode.MODEL);
            var model = (CoolitModel)(GetCommandStateMachineResult(cmd, GetParser(CoolitBridgeCommandCode.MODEL)) ?? CoolitModel.Unknown);
            deviceInfo = DeviceInfoTable[model];

            if (deviceInfo.ProtocolType == ProtocolType.Unsupported)
            {
                return false;
            }

            fanController = (deviceInfo.ProtocolType == ProtocolType.OldWorldCoolingNode) ? 
                (IFanController)new OldWorldFanController(deviceEntity, Channel) :
                                new ModernFanController(deviceEntity, Channel);

            ledController = (deviceInfo.ProtocolType == ProtocolType.OldWorldLightingNode) ?
                (ILedController)new OldWorldLedController(deviceEntity, Channel) :
                                new ModernLedController(deviceEntity, Channel);

            // Retrieving Unique Device ID. Need to identify device with Channel to work correctly with nested Coolit devices
            UDID = deviceEntity.DeviceInstancePath + Channel.ToString();

            if (IsBridge)
            {
                await RunCmdSynchronized(cmd, (byte)CoolitBridgeCommandCode.DEVICE_ADDRESS);
                if (cmd.IsFaulted)
                {
                    return false;
                }

                connectedDevices.Clear();
                byte[] channelState = cmd.Result.Skip(3).ToArray();
                // Skip channel 0, as it is device itself
                for (int address = 1; address <= deviceInfo.ChildChannels; address++) 
                {
                    if (channelState[address] == (byte)CoolitChannelTag.Active)
                    {
                        connectedDevices.Add(new CoolitDevice(deviceEntity, (byte)address));
                    }
                    else if (channelState[address] == (byte)CoolitChannelTag.PSU)
                    {
                        var psu = await CreatePSUCoolitConnectedDevice((byte)address);
                        if (psu != null)
                        {
                            connectedDevices.Add(psu);
                        }
                    }
                }
            }

            return true;
        }

        public async Task UpdateProperties()
        {
            if (await Initialize())
            {
                await ReadFwVersion();

                foreach (var child in connectedDevices)
                {
                    await child.UpdateProperties();
                }

                await FillFanSensorsCollection();
                await FillTemperatureSensorsCollection();
                await FillLedSensorsCollection();
            }
        }

        private async Task ReadFwVersion()
        {
            CommandStateMachine cmd;
            var parser = new BCDFWVersionParser();
            byte commandCode;
            IReportFactory factory;

            switch (deviceInfo.ProtocolType)
            {
                case ProtocolType.OldWorldCoolingNode:
                    commandCode = (byte)CoolitOldCommandCode.FW_VERSION;
                    factory = oldReportFactory;
                    break;

                case ProtocolType.OldWorldLightingNode:
                    commandCode = (byte)LedNodeCommandOpCode.FW_VERSION;
                    factory = lightingNodeReportFactory;
                    break;

                case ProtocolType.Modern:
                default:
                    commandCode = (byte)CoolitModernCommandCode.FW_VERSION;
                    factory = modernReportFactory;
                    break;
            }

            cmd = new CommandStateMachine(deviceEntity, factory, new CoolitBridgeResponseValidator());
            await RunCmdSynchronized(cmd, commandCode);
            fwVersion = (GetCommandStateMachineResult(cmd, parser) ?? string.Empty).ToString();
        }

        private async Task FillFanSensorsCollection()
        {
            fanSensors.Clear();
            if (deviceInfo.ProtocolType == ProtocolType.Modern)
            {
                var cmd = new CommandStateMachine(deviceEntity, modernReportFactory, new CoolitBridgeResponseValidator());
                await RunCmdSynchronized(cmd, (byte)CoolitModernCommandCode.NUMBER_OF_FANS);
                int count = (byte)(GetCommandStateMachineResult(cmd, GetParser(CoolitModernCommandCode.NUMBER_OF_FANS)) ?? 0);

                for (byte i = 0; i < count; i++)
                {
                    byte mode = await fanController.GetFanMode((CoolitSensorAddress)i);
                    if ((mode & 0x80) != 0) // highest bit specifies if fan is detected
                    {
                        fanSensors.Add(new CoolitSensor()
                        {
                            Channel = (CoolitSensorAddress)i,
                            SensorType = CoolitSensorType.FanRpm
                        });
                    }
                }

                if (fanSensors.Count > 0 && deviceInfo.Model != CoolitModel.Whiptail) // whiptail does not contain pump
                {
                    var pump = fanSensors.Last();
                    pump.SensorType = CoolitSensorType.PumpRpm;
                }
            }
            else if (deviceInfo.ProtocolType == ProtocolType.OldWorldCoolingNode)
            {
                int count = 5; // H80, H100 and Cooling node (all old cooling nodes) have up to 5 fan channels
                for (byte i = 0; i < count; i++)
                {
                    byte mode = await fanController.GetFanMode((CoolitSensorAddress)i);
                    if ((mode & 0x80) != 0) // highest bit specifies if fan is detected
                    {
                        fanSensors.Add(new CoolitSensor()
                        {
                            Channel = (CoolitSensorAddress)i,
                            SensorType = CoolitSensorType.FanRpm
                        });
                    }
                }
            }

            NameSensors(fanSensors, deviceInfo.Model);
        }

        private async Task FillTemperatureSensorsCollection()
        {
            temperatureSensors.Clear();

            if (deviceInfo.ProtocolType == ProtocolType.Modern)
            {
                var cmd = new CommandStateMachine(deviceEntity, modernReportFactory, new CoolitBridgeResponseValidator());
                await RunCmdSynchronized(cmd, (byte)CoolitModernCommandCode.NUMBER_OF_TEMPERATURES);
                int count = (byte)(GetCommandStateMachineResult(cmd, GetParser(CoolitModernCommandCode.NUMBER_OF_TEMPERATURES)) ?? 0);

                for (int i = 0; i < count; i++)
                {
                    temperatureSensors.Add(new CoolitSensor()
                    {
                        Channel = (CoolitSensorAddress)i,
                        SensorType = CoolitSensorType.Temperature
                    });
                }
            }
            else if (deviceInfo.ProtocolType == ProtocolType.OldWorldCoolingNode)
            {
                int count = 4; // H80, H100 and Cooling node (all old cooling nodes) have up to 4 temp sensors
                for (byte i = 0; i < count; i++)
                {
                    temperatureSensors.Add(new CoolitSensor()
                    {
                        Channel = (CoolitSensorAddress)i,
                        SensorType = CoolitSensorType.Temperature
                    });
                }
            }

            NameSensors(temperatureSensors, deviceInfo.Model);
        }

        private async Task FillLedSensorsCollection()
        {
            ledSensors.Clear();
            if (deviceInfo.ProtocolType == ProtocolType.Modern)
            {
                var cmd = new CommandStateMachine(deviceEntity, modernReportFactory, new CoolitBridgeResponseValidator());
                await RunCmdSynchronized(cmd, (byte)CoolitModernCommandCode.NUMBER_OF_LEDS);
                int count = (byte)(GetCommandStateMachineResult(cmd, GetParser(CoolitModernCommandCode.NUMBER_OF_LEDS)) ?? 0);

                for (int i = 0; i < count; i++)
                {
                    ledSensors.Add(new CoolitSensor()
                    {
                        Channel = (CoolitSensorAddress)i,
                        SensorType = CoolitSensorType.Led
                    });
                }
            }
            else if (deviceInfo.ProtocolType == ProtocolType.OldWorldLightingNode)
            {
                int count = 2;
                for (int i = 0; i < count; i++)
                {
                    ledSensors.Add(new CoolitSensor()
                    {
                        Channel = (CoolitSensorAddress)i,
                        SensorType = CoolitSensorType.Led
                    });
                }
            }

            NameSensors(ledSensors, deviceInfo.Model);
        }

        private static void NameSensors(List<CoolitSensor> collection, CoolitModel model)
        {
            if (collection.Count == 1 && collection[0].SensorType == CoolitSensorType.Led)
            {
                collection[0].Name = CoolitUtils.GetSensorBaseName(CoolitSensorType.Led);
                return;
            }

            foreach (var sensor in collection)
            {
                sensor.Name = CoolitUtils.GetCoolitSensorName(sensor.SensorType, sensor.Channel, model);
            }
        }

        private byte GetTemperatureSensorChannel(SensorInstance temp)
        {
            byte tempChannel = 0xFF;
            SensorInstance sensorInstance = new SensorInstance();
            sensorInstance.ParentDevice = InstanceKey;
            foreach (var sensor in temperatureSensors)
            {
                sensorInstance.OriginalName = sensor.Name;
                if (sensorInstance.Equals(temp))
                {
                    tempChannel = (byte)sensor.Channel;
                    break;
                }
            }

            return tempChannel;
        }

        public async Task UpdateInfo(IGroupsService groupsService)
        {
            SensorInstance sensorInstance = new SensorInstance() { ParentDevice = InstanceKey };

            if (deviceInfo.ProtocolType == ProtocolType.Modern)
            {
                var cmd = new CommandStateMachine(deviceEntity, modernReportFactory, new CoolitBridgeResponseValidator());
                foreach (var sensor in fanSensors)
                {
                    sensorInstance.OriginalName = sensor.Name;
                    var groupSettings = groupsService.GetGroupSetting(sensorInstance);
                    if (groupSettings != null)
                    {
                        var mode = await fanController.GetFanMode(sensor.Channel);
                        mode &= 0x0F; // clear 'detected' bit
                        if (sensorInstance.ParentDevice.Equals(groupSettings.Temp.ParentDevice) && deviceInfo.Model != CoolitModel.Whiptail)
                        {
                            byte tempChannel = GetTemperatureSensorChannel(groupSettings.Temp);
                            await fanController.SetFanMode(sensor.Channel, (CoolitFanMode)mode, tempChannel);
                        }
                        else
                        {
                            double temperature = groupsService.GetGroupTemperature(groupSettings.Temp);
                            if (temperature > 0)
                            {
                                await fanController.SetFanMode(sensor.Channel, (CoolitFanMode)mode, 7); // 7 - manual
                                await fanController.SetFanManualTemperature(sensor.Channel, temperature);
                            }
                        }
                    }

                    modernReportFactory.CurrentSensor = (byte)sensor.Channel;
                    using (var lck = new CorsairDevicesGuardLock())
                    {
                        await cmd.Run((byte)CoolitModernCommandCode.CURRENT_FAN);
                        await cmd.Run((byte)CoolitModernCommandCode.FAN_CURRENT_RPM);
                    }
                    sensor.Value = (short)(GetCommandStateMachineResult(cmd, GetParser(CoolitModernCommandCode.FAN_CURRENT_RPM)) ?? 0);
                }

                foreach (var sensor in temperatureSensors)
                {
                    modernReportFactory.CurrentSensor = (byte)sensor.Channel;
                    using (var lck = new CorsairDevicesGuardLock())
                    {
                        await cmd.Run((byte)CoolitModernCommandCode.CURRENT_TEMPERATURE);
                        await cmd.Run((byte)CoolitModernCommandCode.TEMPERATURE_CURRENT_TEMPERATURE);
                    }
                    sensor.Value = (short)(GetCommandStateMachineResult(cmd, GetParser(CoolitModernCommandCode.TEMPERATURE_CURRENT_TEMPERATURE)) ?? 0);
                }

                // There is no requirement to read LED data. we should only write it and store on client
            }
            else if (deviceInfo.ProtocolType == ProtocolType.OldWorldCoolingNode)
            {
                var cmd = new CommandStateMachine(deviceEntity, oldReportFactory, new CoolitBridgeResponseValidator());
                foreach (var sensor in fanSensors)
                {
                    sensorInstance.OriginalName = sensor.Name;
                    var groupSettings = groupsService.GetGroupSetting(sensorInstance);

                    if (groupSettings != null)
                    {
                        var mode = await fanController.GetFanMode(sensor.Channel);
                        mode &= 0x0F; // clear 'detected' bit
                        if (sensorInstance.ParentDevice.Equals(groupSettings.Temp.ParentDevice))
                        {
                            byte tempChannel = GetTemperatureSensorChannel(groupSettings.Temp);
                            await fanController.SetFanMode(sensor.Channel, (CoolitFanMode)mode, tempChannel);
                        }
                        else
                        {
                            double temperature = groupsService.GetGroupTemperature(groupSettings.Temp);
                            if (temperature > 0)
                            {
                                await fanController.SetFanMode(sensor.Channel, (CoolitFanMode)mode, 7); // 7 - manual
                                await fanController.SetFanManualTemperature(sensor.Channel, temperature);
                            }
                        }
                    }

                    oldReportFactory.CurrentSensor = (byte)sensor.Channel;
                    await RunCmdSynchronized(cmd, (byte)CoolitOldCommandCode.FAN_CURRENT_RPM);
                    sensor.Value = (short)(GetCommandStateMachineResult(cmd, GetParser(CoolitOldCommandCode.FAN_CURRENT_RPM)) ?? 0);
                }

                foreach (var sensor in temperatureSensors)
                {
                    oldReportFactory.CurrentSensor = (byte)sensor.Channel;
                    await RunCmdSynchronized(cmd, (byte)CoolitOldCommandCode.TEMPERATURE_CURRENT_TEMPERATURE);
                    sensor.Value = (short)(GetCommandStateMachineResult(cmd, GetParser(CoolitOldCommandCode.TEMPERATURE_CURRENT_TEMPERATURE)) ?? 0);
                }
            }

            foreach (var sensor in ledSensors)
            {
                var mode = await ledController.GetLedMode(sensor.Channel);
                if (mode != CoolitLEDMode.TemperatureBased)
                {
                    continue;
                }

                sensorInstance.OriginalName = sensor.Name;
                var groupSettings = groupsService.GetGroupSetting(sensorInstance);
                if (groupSettings != null)
                {
                    if (sensorInstance.ParentDevice.Equals(groupSettings.Temp.ParentDevice))
                    {
                        byte tempChannel = GetTemperatureSensorChannel(groupSettings.Temp);
                        await ledController.SetLedMode(sensor.Channel, mode, tempChannel);
                    }
                    else
                    {
                        double temperature = groupsService.GetGroupTemperature(groupSettings.Temp);
                        if (temperature > 0)
                        {
                            await ledController.SetLedMode(sensor.Channel, mode, 7); // 7 - manual
                            await ledController.SetLedManualTemperature(sensor.Channel, temperature);
                        }
                    }
                }
            }

            foreach (var child in connectedDevices)
            {
                await child.UpdateInfo(groupsService);
            }
        }

        public async Task Accept(ISensorVisitor visitor)
        {
            if (deviceInfo.ProtocolType == ProtocolType.Unsupported)
            {
                return;
            }

            var dataFactory = new DeviceCommunicationDataFactory(InstanceKey, CoolitPathTokenParser.Instance);

            visitor.Visit(dataFactory.CreateData(deviceInfo.Model.ToString(), CoolitSensorType.DeviceName));
            visitor.Visit(dataFactory.CreateData(fwVersion ?? string.Empty, CoolitSensorType.FirmwareVersion));

            foreach (var sensor in fanSensors)
            {
                visitor.Visit(dataFactory.CreateData(sensor.Value, sensor.SensorType, sensor.Channel));
            }

            // sensors are active only if they report temperature greater than zero
            foreach (var sensor in temperatureSensors.Where(t => t.Value > 0))// || deviceInfo.Model == CoolitModel.Whiptail))
            {
                visitor.Visit(dataFactory.CreateData(CoolitDataConverter.WordToTemperature((short)sensor.Value), sensor.SensorType, sensor.Channel));
            }

            foreach (var child in connectedDevices)
            {
                await child.Accept(visitor);
            }
        }

        public async Task ProcessControlData(DeviceCommunicationData data)
        {
            if (await Initialize())
            {
                if (InstanceKey.Equals(data.DeviceInfo))
                {
                    await DoProcessControlData(data);
                }
                else
                {
                    foreach (var child in connectedDevices)
                    {
                        await child.ProcessControlData(data);
                    }
                }
            }
        }

        public async Task SetAllFans100Percents()
        {
            if (await Initialize())
            {
                await FillFanSensorsCollection();
                foreach (var sensor in fanSensors)
                {
                    var sensorAddress = (CoolitSensorAddress)sensor.Channel;
                    if (sensor.SensorType == CoolitSensorType.PumpRpm)
                    {
                        if (deviceInfo.Model == CoolitModel.H110iGT || deviceInfo.Model == CoolitModel.H110i) // only H110iGT(H110i) has controllable pump
                        {
                            await fanController.SetFanMode(sensorAddress, CoolitFanMode.FixedRPM);
                            await fanController.SetFanRPM(sensorAddress, 2950); // 2950 is max value for pump
                        }
                    }
                    else
                    {
                        await fanController.SetFanMode(sensorAddress, CoolitFanMode.FixedPWM);
                        await fanController.SetFanPWM(sensorAddress, 100);
                    }
                }

                foreach (var child in connectedDevices)
                {
                    await child.SetAllFans100Percents();
                }
            }
        }

        public async Task SetAllLeds(RgbColor color)
        {
            byte r = color.Red, g = color.Green, b = color.Blue;
            if (await Initialize())
            {
                await FillLedSensorsCollection();
                foreach (var sensor in ledSensors)
                {
                    var sensorAddress = (CoolitSensorAddress)sensor.Channel;

                    await ledController.SetLedCycleColors(sensorAddress, r, g, b, r, g, b, r, g, b, r, g, b);
                    await ledController.SetLedMode(sensorAddress, CoolitLEDMode.Static);
                }

                foreach (var child in connectedDevices)
                {
                    await child.SetAllLeds(color);
                }
            }
        }

        private async Task DoProcessControlData(DeviceCommunicationData data)
        {
            var factory = CoolitPathTokenParser.Instance;
            var controller = factory.GetEnumValue<CoolitControllerType>(data.DevicePathToken);
            CoolitSensorAddress sensorAddress;
            switch (controller)
            {
                case CoolitControllerType.FanMode:
                    {
                        byte tempChannel = data.Data[0];
                        int offset = 1;
                        var fanMode = ByteDataDeserializer.ConvertEnumFromBytes<CoolitFanMode>(data.Data.Skip(offset).Take(sizeof(int)).ToArray());
                        offset += sizeof(int);
                        sensorAddress = factory.GetEnumValue<CoolitSensorAddress>(data.DevicePathToken);

                        if (deviceInfo.Model == CoolitModel.H80i || deviceInfo.Model == CoolitModel.H100i)
                        {
                            if (sensorAddress == CoolitSensorAddress.Port4)
                            {
                                return;
                            }
                        }

                        // spec says: H110iGT(H110i) has two fans, channel 0 (first fan), channel 1 (second fan), channel 2 (pump)
                        if ((deviceInfo.Model == CoolitModel.H110iGT || deviceInfo.Model == CoolitModel.H110i) && sensorAddress == CoolitSensorAddress.Port2)
                        {
                            // Performance - pump is working on 2950 rpm
                            // Quiet - pump is working on 2350 rpm. This is the default option
                            short rpmValue = (short)(fanMode == CoolitFanMode.Performance ? 2950 : 2350);
                            await fanController.SetFanMode(sensorAddress, CoolitFanMode.FixedRPM);
                            await fanController.SetFanRPM(sensorAddress, rpmValue);
                            return;
                        }

                        await fanController.SetFanMode(sensorAddress, fanMode, tempChannel);
                        if (fanMode == CoolitFanMode.Custom)
                        {
                            const int PointsCount = 5;
                            var temps = data.Data.Skip(offset).Take(PointsCount).Select(t => (double)t).ToArray();
                            offset += PointsCount;
                            var rpmBytes = data.Data.Skip(offset).Take(PointsCount * sizeof(short)).ToArray();
                            short[] rpms = new short[PointsCount];
                            for (int i = 0; i < rpmBytes.Length; i += sizeof(short))
                            {
                                rpms[i >> 1] = BitConverter.ToInt16(rpmBytes, i);
                            }

                            await fanController.SetCustomCurve(sensorAddress, rpms[0], rpms[1], rpms[2], rpms[3], rpms[4], temps[0], temps[1], temps[2], temps[3], temps[4]);
                        }
                    }

                    break;
                case CoolitControllerType.FanRPM:
                    short rpm = (short)ByteDataDeserializer.ConvertIntFromBytes(data.Data);
                    sensorAddress = factory.GetEnumValue<CoolitSensorAddress>(data.DevicePathToken);
                    await fanController.SetFanMode(sensorAddress, CoolitFanMode.FixedRPM);
                    await fanController.SetFanRPM(sensorAddress, rpm);
                    break;
                case CoolitControllerType.FanPWM:
                    byte pwm = ByteDataDeserializer.ConvertSingleByteFromBytes(data.Data);
                    sensorAddress = factory.GetEnumValue<CoolitSensorAddress>(data.DevicePathToken);
                    await fanController.SetFanMode(sensorAddress, CoolitFanMode.FixedPWM);
                    await fanController.SetFanPWM(sensorAddress, pwm);
                    break;
                case CoolitControllerType.FanManualTemperature:
                    {
                        double temperature = ByteDataDeserializer.ConvertDoubleFromBytes(data.Data);
                        sensorAddress = factory.GetEnumValue<CoolitSensorAddress>(data.DevicePathToken);
                        await fanController.SetFanManualTemperature(sensorAddress, temperature);
                    }

                    break;
                case CoolitControllerType.LedMode:
                    {
                        int offset = 0;
                        byte tempChannel = data.Data[offset++];
                        var ledMode = (CoolitLedMode)data.Data[offset++];
                        sensorAddress = factory.GetEnumValue<CoolitSensorAddress>(data.DevicePathToken);
                        var c1 = data.Data.Skip(offset).Take(3).ToArray();
                        offset += 3;
                        var c2 = data.Data.Skip(offset).Take(3).ToArray();
                        offset += 3;
                        var c3 = data.Data.Skip(offset).Take(3).ToArray();
                        offset += 3;
                        var c4 = data.Data.Skip(offset).Take(3).ToArray();
                        offset += 3;
                        var t1 = data.Data[offset++];
                        var t2 = data.Data[offset++];
                        var t3 = data.Data[offset++];

                        switch (ledMode)
                        {
                            case CoolitLedMode.StaticColor:
                                await ledController.SetLedCycleColors(sensorAddress, c1[0], c1[1], c1[2], c2[0], c2[1], c2[2], c3[0], c3[1], c3[2], c4[0], c4[1], c4[2]);
                                await ledController.SetLedMode(sensorAddress, CoolitLEDMode.Static, tempChannel);
                                break;
                            case CoolitLedMode.CycleThroughTwoColors:
                                await ledController.SetLedCycleColors(sensorAddress, c1[0], c1[1], c1[2], c2[0], c2[1], c2[2], c3[0], c3[1], c3[2], c4[0], c4[1], c4[2]);
                                await ledController.SetLedMode(sensorAddress, CoolitLEDMode.TwoColor, tempChannel);
                                break;
                            case CoolitLedMode.CycleThroughFourColors:
                                await ledController.SetLedCycleColors(sensorAddress, c1[0], c1[1], c1[2], c2[0], c2[1], c2[2], c3[0], c3[1], c3[2], c4[0], c4[1], c4[2]);
                                await ledController.SetLedMode(sensorAddress, CoolitLEDMode.FourColor, tempChannel);
                                break;
                            case CoolitLedMode.TemperatureBased:
                                await ledController.SetLedColorsAndTemperatures(sensorAddress, c1[0], c1[1], c1[2], c2[0], c2[1], c2[2], c3[0], c3[1], c3[2], t1, t2, t3);
                                await ledController.SetLedMode(sensorAddress, CoolitLEDMode.TemperatureBased, tempChannel);
                                break;
                            default:
                                break;
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        private async Task<ICoolitConnectedDevice> CreatePSUCoolitConnectedDevice(byte address)
        {
            byte command = (byte)FlextronicsCommandCode.READ_MFR_MODEL; // the same for HXi-RMi devices
            int length = 7;
            var factory = BridgeProxyOutputReportFactory.CreateReadBlockFactory(deviceEntity, address, length);
            var cmd = new CommandStateMachine(deviceEntity, factory, new CoolitBridgeResponseValidator());
            await RunCmdSynchronized(cmd, command);
            if (cmd.IsFaulted)
            {
                return null;
            }

            var data = BlockParser.ParseResponse(cmd.Result);
            string model = System.Text.Encoding.ASCII.GetString(data).TrimEnd('\0');
            // check if Flextronics
            if (FlextronicsDeviceDefinition.SupportedDevices.Contains(model))
            {
                return new CoolitConnectedFlextronics(deviceEntity, address, InstanceKey.SerialNumber);
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(model, @"^(HX|RM)\d+i$"))
            {
                return CorsairLink4.Module.HidDevices.PowerSupplyUnits.PowerSupplyUnit.CreateCoolitConnectedPowerSupplyUnit(deviceEntity, address);
            }

            // unsupported
            return null;
        }

        public void Dispose()
        {
            deviceEntity.Dispose();
        }

        private object GetCommandStateMachineResult(CommandStateMachine cmd, IResponseParser parser)
        {
            if (cmd.IsFaulted)
            {
                return null;
            }

            return parser.Parse(cmd.Result);
        }

        private IResponseParser GetParser(CoolitBridgeCommandCode commandCode)
        {
            IResponseParser result = null;

            if (commandCode == CoolitBridgeCommandCode.MODEL)
            {
                result = new ModelParser();
            }

            // TODO: [tm] add more parsers here

            if (result == null)
            {
                result = new NullParser();
            }

            return result;
        }

        private IResponseParser GetParser(CoolitModernCommandCode commandCode)
        {
            IResponseParser result = null;

            if (commandCode == CoolitModernCommandCode.FAN_CURRENT_RPM ||
                commandCode == CoolitModernCommandCode.TEMPERATURE_CURRENT_TEMPERATURE)
            {
                result = new WordParser();
            }

            if (commandCode == CoolitModernCommandCode.NUMBER_OF_FANS ||
                commandCode == CoolitModernCommandCode.NUMBER_OF_TEMPERATURES ||
                commandCode == CoolitModernCommandCode.NUMBER_OF_LEDS)
            {
                result = new ByteParser();
            }

            if (commandCode == CoolitModernCommandCode.FAN_MODE_GET)
            {
                result = new ByteParser();
            }

            if (commandCode == CoolitModernCommandCode.FW_VERSION)
            {
                result = new BCDFWVersionParser();
            }

            // TODO: [tm] add more parsers here

            if (result == null)
            {
                result = new NullParser();
            }

            return result;
        }

        private IResponseParser GetParser(CoolitOldCommandCode commandCode)
        {
            IResponseParser result = null;

            if (commandCode == CoolitOldCommandCode.FAN_CURRENT_RPM ||
                commandCode == CoolitOldCommandCode.TEMPERATURE_CURRENT_TEMPERATURE)
            {
                result = new WordParser();
            }

            if (commandCode == CoolitOldCommandCode.FAN_MODE_GET)
            {
                result = new ByteParser();
            }

            if (commandCode == CoolitOldCommandCode.FW_VERSION)
            {
                result = new BCDFWVersionParser();
            }

            // TODO: [tm] add more parsers here

            if (result == null)
            {
                result = new NullParser();
            }

            return result;
        }

        private async Task RunCmdSynchronized(CommandStateMachine cmd, byte data)
        {
            using (var lck = new CorsairDevicesGuardLock())
            {
                await cmd.Run(data);
            }
        }

        private enum ProtocolType
        {
            Unsupported,
            OldWorldCoolingNode,
            OldWorldLightingNode,
            Modern,
        }

        private class CoolitDeviceInfo
        {
            public CoolitDeviceInfo(CoolitModel model, string pid, ProtocolType protocolType, bool bridgeable, int childChannels = 0)
            {
                Model = model;
                Pid = pid;
                Bridgeable = bridgeable;
                ChildChannels = childChannels;
                ProtocolType = protocolType;
            }

            public CoolitModel Model { get; private set; }

            public string Pid { get; private set; }

            public bool Bridgeable { get; private set; }

            public int ChildChannels { get; private set; }

            public ProtocolType ProtocolType { get; private set; }
        }
    }
}
