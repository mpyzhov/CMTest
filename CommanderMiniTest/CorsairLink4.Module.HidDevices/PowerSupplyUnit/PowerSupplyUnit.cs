using CorsairLink4.Common.DevicesDefinitions.Common;
using CorsairLink4.Common.DevicesDefinitions.PowerSupplyUnit;
using CorsairLink4.Common.Shared.Communication;
using CorsairLink4.Common.Shared.DataSerialization;
using CorsairLink4.Common.Shared.DataSerialization.PowerSupplyUnit;
using CorsairLink4.Common.Shared.DevicesData;
using CorsairLink4.Common.Shared.Utils;
using CorsairLink4.Module.HidDevices.Coolit;
using CorsairLink4.Module.HidDevices.Coolit.ReportFactories;
using CorsairLink4.Module.HidDevices.Coolit.ResponseValidation;
using CorsairLink4.Module.HidDevices.Core;
using CorsairLink4.Module.HidDevices.Models;
using CorsairLink4.Module.HidDevices.PowerSupplyUnits.ResponseParsing;
using CorsairLink4.Module.HidDevices.PowerSupplyUnits.ResponseValidation;
using CorsairLink4.Public.Synchronization;
using HumanInterfaceDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits
{
    public sealed class PowerSupplyUnit : IDeviceComponent, ICoolitConnectedDevice, IDisposable
    {
        private const byte BusAddress = 1;
        private readonly PowerSupplyUnitDeviceEntity deviceEntity;
        private readonly HidDevice hidDevice;
        private readonly ConnectionTypeHelper connectionTypeHelper;

        private PsuCommandCode[] singlePagePopertiesCommands = new PsuCommandCode[]
        {
            PsuCommandCode.READ_IOUT,
            PsuCommandCode.READ_VOUT,
            PsuCommandCode.VOUT_OV_FAULT_LIMIT,
            PsuCommandCode.VOUT_UV_FAULT_LIMIT,
            PsuCommandCode.IOUT_OC_FAULT_LIMIT,
            PsuCommandCode.READ_POUT,
        };

        private PsuCommandCode[] generalPropertiesCommands = new PsuCommandCode[]
        {
            PsuCommandCode.READ_VIN,
            PsuCommandCode.READ_FAN_SPEED_1,
            PsuCommandCode.FAN_COMMAND_1,
            PsuCommandCode.OT_FAULT_LIMIT,
            PsuCommandCode.READ_TEMPERATURE_1,
            PsuCommandCode.READ_TEMPERATURE_2,
            PsuCommandCode.MFR_12V_OCP_MODE,
            PsuCommandCode.MFR_READ_TOTAL_POUT,
        };

        private PsuCommandCode[] generalInfoCommands = new PsuCommandCode[]
        {
            PsuCommandCode.READ_MFR_ID,
            PsuCommandCode.READ_MFR_MODEL,
            PsuCommandCode.READ_FIRMWARE_REVISION,
        };

        private PowerSupplyUnit(HidDevice device, PowerSupplyUnitDeviceEntity psuEntity, ConnectionMode deviceConnectionMode, byte channel)
        {
            deviceEntity = psuEntity;
            hidDevice = device;
            connectionTypeHelper = new ConnectionTypeHelper(device, deviceConnectionMode, channel);
        }

        public static PowerSupplyUnit CreatePowerSupplyUnit(HidDevice device, PowerSupplyUnitDeviceEntity psuEntity)
        {
            return new PowerSupplyUnit(device, psuEntity, ConnectionMode.Direct, 0);
        }

        public static PowerSupplyUnit CreateCoolitConnectedPowerSupplyUnit(HidDevice device, byte channel)
        {
            return new PowerSupplyUnit(device, new PowerSupplyUnitDeviceEntity(), ConnectionMode.Coolit, channel);
        }

        private enum ConnectionMode
        {
            Direct,
            Coolit
        }

        public DeviceInstance InstanceKey
        {
            get
            {
                return connectionTypeHelper.DeviceInstanceKey;
            }
        }
        public async Task ProcessControlData(DeviceCommunicationData data)
        {
            if (data.DeviceInfo.Equals(InstanceKey))
            {
                var factory = PowerSupplyUnitPathTokenParser.Instance;
                var controller = factory.GetEnumValue<PowerSupplyUnitControllerType>(data.DevicePathToken);
                switch (controller)
                {
                    case PowerSupplyUnitControllerType.OCPMode:
                        var mode = ByteDataDeserializer.ConvertBooleanFromBytes(data.Data) ? OverCurrentProtectionMode.MultiRail : OverCurrentProtectionMode.SingleRail;
                        await RunCommand(PsuCommandCode.MFR_12V_OCP_MODE, (byte)mode);
                        break;
                    case PowerSupplyUnitControllerType.FanMode:
                        var fanMode = ByteDataDeserializer.ConvertEnumFromBytes<PowerSupplyUnitFanModeType>(data.Data);
                        await SetFanMode(fanMode);
                        break;
                    case PowerSupplyUnitControllerType.FanPower:
                        var power = ByteDataDeserializer.ConvertSingleByteFromBytes(data.Data);
                        await SetFanPower(power);
                        break;
                    default:
                        break;
                }
            }
        }

        public async Task SetFanPower(byte power)
        {
            await RunCommand(PsuCommandCode.FAN_COMMAND_1, power);
        }

        public async Task SetFanMode(PowerSupplyUnitFanModeType fanMode)
        {
            byte fanModeByte = (byte)(fanMode == PowerSupplyUnitFanModeType.Normal ? 0 : 1);
            await RunCommand(PsuCommandCode.FAN_INDEX, fanModeByte);
        }

        public async Task SetAllFans100Percents()
        {
            await SetFanMode(PowerSupplyUnitFanModeType.Manual);
            await SetFanPower(100);
        }

        public Task SetAllLeds(Common.Shared.Settings.RgbColor color)
        {
            return CompletedTask.Empty;
        }

        public Task Accept(ISensorVisitor visitor)
        {
            var instance = InstanceKey;
            
            var dataFactory = new DeviceCommunicationDataFactory(instance, PowerSupplyUnitPathTokenParser.Instance);

            visitor.Visit(dataFactory.CreateData(deviceEntity.ManufacturersModel, PowerSupplyUnitSensorType.ManufacturersModel));

            if (CheckValue(deviceEntity.Temperature2))
            {
                visitor.Visit(dataFactory.CreateData(deviceEntity.Temperature2, PowerSupplyUnitSensorType.Temperature));
            }

            if (CheckValue(deviceEntity.OverTemperatureFaultLimit))
            {
                visitor.Visit(dataFactory.CreateData(deviceEntity.OverTemperatureFaultLimit, PowerSupplyUnitSensorType.OverTemperatureFaultLimit));
            }


            if (CheckValue(deviceEntity.FanSpeed))
            {
                visitor.Visit(dataFactory.CreateData(deviceEntity.FanSpeed, PowerSupplyUnitSensorType.FanSpeed));
            }
            visitor.Visit(dataFactory.CreateData(deviceEntity.FanPower, PowerSupplyUnitSensorType.FanPower));

            visitor.Visit(dataFactory.CreateData(deviceEntity.InputVoltage, PowerSupplyUnitSensorType.InputVoltage));
            visitor.Visit(dataFactory.CreateData(deviceEntity.PowerOutTotal, PowerSupplyUnitSensorType.PowerOutTotal));
            visitor.Visit(dataFactory.CreateData(deviceEntity.OCPMode, PowerSupplyUnitSensorType.OCPMode));

            foreach (var page in EnumsUtil.GetAllValuesEnum<PowerSupplyUnitPageAddress>())
            {
                PowerSupplyUnitPage devicePage = deviceEntity.Pages[(int)page];

                visitor.Visit(dataFactory.CreateData(devicePage.OutputCurrent, PowerSupplyUnitSensorType.OutputCurrent, page));
                visitor.Visit(dataFactory.CreateData(devicePage.OutputCurrentOverCurrentFaultLimit, PowerSupplyUnitSensorType.OutputCurrentOverCurrentFaultLimit, page));
                visitor.Visit(dataFactory.CreateData(devicePage.OutputVoltage, PowerSupplyUnitSensorType.OutputVoltage, page));
                visitor.Visit(dataFactory.CreateData(devicePage.OutputPower, PowerSupplyUnitSensorType.OutputPower, page));
            }

            return CompletedTask.Empty;
        }

        public void Dispose()
        {
            hidDevice.Dispose();
        }

        public async Task UpdateProperties()
        {
            PsuOutputReportFactory factory = new PsuOutputReportFactory(BusAddress, hidDevice)
            {
                PageMode = HidCommandMode.Write,
                FanCommandMode = HidCommandMode.Read,
                OCPModeMode = HidCommandMode.Read,
            };
            var cmd = connectionTypeHelper.CreateCmd(factory);

            foreach (var command in generalPropertiesCommands)
            {
                using (var lck = new CorsairDevicesGuardLock())
                {
                    await DoRunCommand(cmd, command);
                }
                UpdateDeviceEntityGeneralProperty(command, GetCommandStateMachineResult(cmd, command));
            }

            for (byte pageAddress = 0; pageAddress <= 2; pageAddress++)
            {
                factory.PageArgument = pageAddress;
                foreach (var command in singlePagePopertiesCommands)
                {
                    using (var lck = new CorsairDevicesGuardLock())
                    {
                        await DoRunCommand(cmd, PsuCommandCode.PAGE);
                        await DoRunCommand(cmd, command);
                    }
                    UpdateDeviceEntityPageProperty(command, (PSUPage)pageAddress, GetCommandStateMachineResult(cmd, command));
                }
            }
        }

        public async Task UpdateInfo(Common.Shared.Settings.IGroupsService groupsService)
        {
            PsuOutputReportFactory factory = new PsuOutputReportFactory(BusAddress, hidDevice)
            {
                PageMode = HidCommandMode.Write,
                FanCommandMode = HidCommandMode.Read,
            };
            var cmd = connectionTypeHelper.CreateCmd(factory);

            foreach (var command in generalInfoCommands)
            {
                using (var lck = new CorsairDevicesGuardLock())
                {
                    await DoRunCommand(cmd, command);
                }
                UpdateDeviceEntityGeneralProperty(command, GetCommandStateMachineResult(cmd, command));
            }
        }

        private async Task DoRunCommand(CommandStateMachine cmd, PsuCommandCode command)
        {
            await connectionTypeHelper.RunCmd(cmd, PsuCommandCode.HANDSHAKE);
            await connectionTypeHelper.RunCmd(cmd, command);
        }

        internal async Task RunCommand(PsuCommandCode psuCommandCode, byte arg)
        {
            var factory = new PsuOutputReportFactory(BusAddress, hidDevice)
            {
                FanCommandMode = HidCommandMode.Write,
                FanCommandArgument = arg,
                OCPModeMode = HidCommandMode.Write,
                OCPModeArgument = arg,
                FanIndexAgrument = arg,
            };

            var cmd = connectionTypeHelper.CreateCmd(factory);
            using (var lck = new CorsairDevicesGuardLock())
            {
                await DoRunCommand(cmd, psuCommandCode);
            }
        }

        private IResponseParser GetParser(PsuCommandCode commandCode)
        {
            IResponseParser result = null;
            PsuCommandCode[] commandsBytesAsDouble = new PsuCommandCode[]
            {
                PsuCommandCode.READ_IOUT,
                PsuCommandCode.READ_VOUT,
                PsuCommandCode.READ_VIN,
                PsuCommandCode.READ_FAN_SPEED_1,
                PsuCommandCode.VOUT_OV_FAULT_LIMIT,
                PsuCommandCode.VOUT_UV_FAULT_LIMIT,
                PsuCommandCode.IOUT_OC_FAULT_LIMIT,
                PsuCommandCode.OT_FAULT_LIMIT,
                PsuCommandCode.READ_TEMPERATURE_1,
                PsuCommandCode.READ_TEMPERATURE_2,
                PsuCommandCode.READ_POUT,
                PsuCommandCode.MFR_READ_TOTAL_POUT,
            };

            if (commandsBytesAsDouble.Contains(commandCode))
            {
                result = new BytesAsDoubleParser(3);
            }

            if (commandCode == PsuCommandCode.FAN_COMMAND_1)
            {
                result = new SingleByteParser(1);
            }

            if (commandCode == PsuCommandCode.READ_MFR_ID || commandCode == PsuCommandCode.READ_MFR_MODEL)
            {
                result = new BlockParser(7);
            }

            if (commandCode == PsuCommandCode.READ_FIRMWARE_REVISION)
            {
                result = new FirmwareVersionParser();
            }

            if (commandCode == PsuCommandCode.MFR_12V_OCP_MODE)
            {
                result = new SingleByteParser(6);
            }

            if (result == null)
            {
                result = new NullParser();
            }

            return result;
        }

        private void UpdateDeviceEntityGeneralProperty(PsuCommandCode command, object value)
        {
            if (value == null)
            {
                return;
            }

            switch (command)
            {
                case PsuCommandCode.READ_VIN:
                    double inputVoltageValue = (double)value;
                    deviceEntity.InputVoltage = inputVoltageValue;
                    break;
                case PsuCommandCode.MFR_READ_TOTAL_POUT:
                    double outputPowerValue = (double)value;
                    deviceEntity.PowerOutTotal = outputPowerValue;
                    break;
                case PsuCommandCode.READ_FAN_SPEED_1:
                    double fanSpeed = (double)value;
                    deviceEntity.FanSpeed = fanSpeed;
                    break;
                case PsuCommandCode.FAN_COMMAND_1:
                    int fanPower = (byte)value;
                    deviceEntity.FanPower = fanPower;
                    break;
                case PsuCommandCode.OT_FAULT_LIMIT:
                    double overTemperatureFaultValue = (double)value;
                    deviceEntity.OverTemperatureFaultLimit = overTemperatureFaultValue;
                    break;
                case PsuCommandCode.READ_TEMPERATURE_1:
                    double temperature1Value = (double)value;
                    deviceEntity.Temperature1 = temperature1Value;
                    break;
                case PsuCommandCode.READ_TEMPERATURE_2:
                    double temperature2Value = (double)value;
                    deviceEntity.Temperature2 = temperature2Value;
                    break;
                case PsuCommandCode.READ_MFR_ID:
                    string mfrID = (string)value;
                    deviceEntity.ManufacturersID = mfrID;
                    break;
                case PsuCommandCode.READ_MFR_MODEL:
                    string mfrModel = (string)value;
                    deviceEntity.ManufacturersModel = mfrModel;
                    break;
                case PsuCommandCode.READ_FIRMWARE_REVISION:
                    string version = (string)value;
                    deviceEntity.FirmwareVersion = version;
                    break;
                case PsuCommandCode.MFR_12V_OCP_MODE:
                    OverCurrentProtectionMode ocpMode = (OverCurrentProtectionMode)(byte)value;
                    deviceEntity.OCPMode = Enum.IsDefined(typeof(OverCurrentProtectionMode), ocpMode) ? ocpMode : OverCurrentProtectionMode.SingleRail;
                    break;
                default:
                    break;
            }
        }

        private void UpdateDeviceEntityPageProperty(PsuCommandCode command, PSUPage page, object value)
        {
            if (value == null)
            {
                return;
            }

            int pageIndex = (byte)page;
            if (deviceEntity.Pages[pageIndex] == null)
            {
                deviceEntity.Pages[pageIndex] = new PowerSupplyUnitPage();
            }

            PowerSupplyUnitPage devicePage = deviceEntity.Pages[pageIndex];

            switch (command)
            {
                case PsuCommandCode.READ_IOUT:
                    double outputCurrentValue = (double)value;
                    devicePage.OutputCurrent = outputCurrentValue;
                    break;
                case PsuCommandCode.READ_VOUT:
                    double outputVoltageValue = (double)value;
                    devicePage.OutputVoltage = outputVoltageValue;
                    break;
                case PsuCommandCode.VOUT_OV_FAULT_LIMIT:
                    double voutOVFaloutValue = (double)value;
                    devicePage.OutputVoltageOverVoltageFaultLimit = voutOVFaloutValue;
                    break;
                case PsuCommandCode.VOUT_UV_FAULT_LIMIT:
                    double voutUVFaloutValue = (double)value;
                    devicePage.OutputVoltageUnderVoltageFaultLimit = voutUVFaloutValue;
                    break;
                case PsuCommandCode.IOUT_OC_FAULT_LIMIT:
                    double ioutOCFaloutValue = (double)value;
                    devicePage.OutputCurrentOverCurrentFaultLimit = ioutOCFaloutValue;
                    break;
                case PsuCommandCode.READ_POUT:
                    double poutValue = (double)value;
                    devicePage.OutputPower = poutValue;
                    break;
                default:
                    break;
            }
        }

        // null if HANDSHAKE or failed
        private object GetCommandStateMachineResult(CommandStateMachine cmd, PsuCommandCode code)
        {
            if (code == PsuCommandCode.HANDSHAKE || cmd.IsFaulted)
            {
                return null;
            }

            return connectionTypeHelper.CreateResponseParser(GetParser(code)).Parse(cmd.Result);
        }

        private bool CheckValue(double value)
        {
            const int maxAdequateValue = 5000;
            // sometimes HW getting crazy returning huge values
            // this hack prevents such situations
            return value < maxAdequateValue;
        }

        private class ConnectionTypeHelper
        {
            private readonly ConnectionMode connection;
            private readonly byte channel;
            private readonly HidDevice device;

            public ConnectionTypeHelper(HidDevice hidDevice, ConnectionMode connectionMode, byte address)
            {
                connection = connectionMode;
                channel = address;
                device = hidDevice;
            }

            public DeviceInstance DeviceInstanceKey
            {
                get
                {
                    string hidDeviceSerial = device.Version.ToString("x4");
                    if (IsDirect)
                    {
                        return new DeviceInstance(device.ProductId.ToString("x4"), device.VendorId.ToString("x4"), hidDeviceSerial);
                    }

                    return new DeviceInstance(DeviceIdentity.Corsair.Pid.CorsairPSU_HX1200i, DeviceIdentity.Corsair.Vid, hidDeviceSerial + channel);
                }
            }

            private bool IsDirect
            {
                get
                {
                    return connection == ConnectionMode.Direct;
                }
            }
    
            private bool IsCoolit
            {
                get
                {
                    return connection == ConnectionMode.Coolit;
                }
            }

            public CommandStateMachine CreateCmd(PsuOutputReportFactory psuOutputReportFactory)
            {
                IReportFactory factory = null;
                IResponseValidator responseValidator = null;

                if (IsDirect)
                {
                    factory = psuOutputReportFactory;
                    responseValidator = new PsuResponseValidator();
                }
                else
                {
                    factory = new JoycePSUOutputReportFactory(device, channel, psuOutputReportFactory);
                    responseValidator = new CoolitBridgeResponseValidator();
                }

                return new CommandStateMachine(device, factory, responseValidator);
            }

            public async Task RunCmd(CommandStateMachine cmd, PsuCommandCode commandCode)
            {
                // in case of coolit connected device we don't need to run handshake
                if (IsCoolit && commandCode == PsuCommandCode.HANDSHAKE)
                {
                    return;
                }

                await cmd.Run((byte)commandCode);
            }

            public IResponseParser CreateResponseParser(IResponseParser resultParser)
            {
                if (IsDirect)
                {
                    return resultParser;
                }

                return new CoolitDataParser(resultParser);
            }
        }
    }
}
