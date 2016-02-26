using HidDevices.General;
using HumanInterfaceDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    public class CoolitDevice : IDisposable
    {
        private HidDevice deviceEntity;

        private List<CoolitSensor> fanSensors = new List<CoolitSensor>();
        private List<CoolitSensor> temperatureSensors = new List<CoolitSensor>();
        private List<CoolitSensor> ledSensors = new List<CoolitSensor>();

        private CoolitModernOutputReportFactory modernReportFactory;
        private CoolitBridgeOutputReportFactory bridgeReportFactory;
        private CoolitDeviceInfo deviceInfo;
        private string fwVersion;
        private ModernFanController fanController;

        private static readonly Dictionary<CoolitModel, CoolitDeviceInfo> DeviceInfoTable = new Dictionary<CoolitModel, CoolitDeviceInfo>()
        {
            { CoolitModel.Whiptail, new CoolitDeviceInfo(CoolitModel.Whiptail, "0c043d", ProtocolType.Modern, true, 5) },
        };

        public CoolitDevice(HidDevice hidDevice, byte channel = 0)
        {
            deviceEntity = hidDevice;
            Channel = channel;
            bridgeReportFactory = new CoolitBridgeOutputReportFactory(deviceEntity) { Channel = channel };
            modernReportFactory = new CoolitModernOutputReportFactory(deviceEntity) { Channel = channel };
            //oldReportFactory = new CoolitOldOutputReportFactory(deviceEntity) { Channel = channel };
            //lightingNodeReportFactory = new OldLightingNodeOutputReportFactory(deviceEntity) { Channel = channel };
        }

        public string FwVersion
        {
            get { return fwVersion; }
        }

        public int FanCount
        {
            get { return fanSensors.Count; }
        }

        public bool IsInitialized { get; private set; }

        public string UDID { get; private set; }

        public byte Channel { get; private set; }

        public DeviceInstance InstanceKey
        {
            get
            {
                return new DeviceInstance(deviceInfo.Pid, "1b1c", UDID);
            }
        }

        public HidDevice DeviceEntity
        {
            get
            {
                return deviceEntity;
            }
        }

        //private bool IsBridge
        //{
        //    get
        //    {
        //        return Channel == 0 && deviceInfo.Bridgeable;
        //    }
        //}

        public async Task<bool> Initialize()
        {
            var cmd = new CommandStateMachine(deviceEntity, bridgeReportFactory, new CoolitBridgeResponseValidator());
            await RunCmdSynchronized(cmd, (byte)CoolitBridgeCommandCode.MODEL);
            var model = (CoolitModel)(GetCommandStateMachineResult(cmd, GetParser(CoolitBridgeCommandCode.MODEL)) ?? CoolitModel.Unknown);
            deviceInfo = DeviceInfoTable.ContainsKey(model) ? DeviceInfoTable[model] : new CoolitDeviceInfo(CoolitModel.Unknown, "0000", ProtocolType.Unsupported, true, 0);

            if (deviceInfo.ProtocolType == ProtocolType.Unsupported)
            {
                return IsInitialized = false;
            }

            fanController = new ModernFanController(deviceEntity, Channel);

            // Retrieving Unique Device ID. Need to identify device with Channel to work correctly with nested Coolit devices
            UDID = deviceEntity.DeviceInstancePath + Channel.ToString();

            await RunCmdSynchronized(cmd, (byte)CoolitBridgeCommandCode.DEVICE_ADDRESS);

            if (cmd.IsFaulted)
            {
                return IsInitialized = false;
            }

            return IsInitialized = true;
        }

        public async Task UpdateProperties()
        {
            if (await Initialize())
            {
                await ReadFwVersion();

                await FillFanSensorsCollection();
                await FillTemperatureSensorsCollection();
                await FillLedSensorsCollection();
            }
        }

        private async Task ReadFwVersion()
        {
            fwVersion = "<Empty>";
            CommandStateMachine cmd;
            var parser = new BCDFWVersionParser();
            byte commandCode;
            IReportFactory factory;

            switch (deviceInfo.ProtocolType)
            {
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

        private static void NameSensors(List<CoolitSensor> collection, CoolitModel model)
        {
            if (collection.Count == 1 && collection[0].SensorType == CoolitSensorType.Led)
            {
                collection[0].Name = "Led";
                return;
            }

            foreach (var sensor in collection)
            {
                sensor.Name = CoolitUtils.GetCoolitSensorName(sensor.SensorType, sensor.Channel, model);
            }
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

        private async Task RunCmdSynchronized(CommandStateMachine cmd, byte data)
        {
            await cmd.Run(data);
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
