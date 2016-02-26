//using CorsairLink4.Common.Models;
//using CorsairLink4.Service.Devices.Buffer;
//using CorsairLink4.Service.Devices.CLink.ResponseParsers;
//using CorsairLink4.Service.Devices.PowerSupplyUnits;
//using NLog;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Windows.Devices.HumanInterfaceDevice;

//namespace CorsairLink4.Service.Devices.CLink
//{
//    public class Link : HidDevice
//    {
//        private LinkDeviceEntity deviceEntity = null;

//        public Link(LinkDeviceEntity linkEntity, IHidDeviceManager deviceManager)
//            : base(linkEntity, deviceManager)
//        {
//            TraceMethodEnter();
//            this.deviceEntity = linkEntity;
//            TraceMethodLeave();
//        }

//        public override async Task UpdateProperties()
//        {
//            TraceMethodEnter();

//            LinkOutputReportFactory factory = new LinkOutputReportFactory(Capabilities.OutputReportByteLength);
//            var cmd = new CommandStateMachine(this, factory, new LinkResponseValidator());

//            for (byte address = 0; address < 2; address++)
//            {
//                factory.Address = address;
//                foreach (var command in CommandFlow())
//                {
//                    await cmd.Run((byte)command);
//                    if (command != LinkCommandCode.READ_STATUS && !cmd.IsFaulted)
//                    {
//                        UpdateDeviceEntity(command, address, GetParser(command).Parse(cmd.Result));
//                    }
//                }
//            }

//            LogDeviceInfo();
//            TraceMethodLeave();
//        }

//        public override async Task UpdateInfo()
//        {
//            TraceMethodEnter();
//            Logger.Debug("Updating link info - nothing to update");
//            await Task.FromResult(false);
//            TraceMethodLeave();
//        }

//        internal async Task RunCommand(LinkCommandCode linkCommandCode, byte address, byte value)
//        {
//            TraceMethodEnter();

//            IReportFactory factory = new LinkOutputReportFactory(Capabilities.OutputReportByteLength)
//            {
//                Address = address,
//                FanFrequency = value,
//            };

//            var cmd = new CommandStateMachine(this, factory, new LinkResponseValidator());
//            await cmd.Run((byte)LinkCommandCode.READ_STATUS);
//            await cmd.Run((byte)linkCommandCode);
//            TraceMethodLeave();
//        }

//        private IEnumerable<LinkCommandCode> CommandFlow()
//        {
//            Logger.Trace("CommandFlow()");
//            var commandSet = new LinkCommandCode[]
//                {
//                    LinkCommandCode.READ_TEMPERATURE,
//                    LinkCommandCode.READ_FREQUENCY,
//                    LinkCommandCode.READ_PWM,
//                };

//            foreach (var cmd in commandSet)
//            {
//                yield return LinkCommandCode.READ_STATUS;
//                yield return cmd;
//            }
//        }

//        private IResponseParser GetParser(LinkCommandCode commandCode)
//        {
//            TraceMethodEnter();
//            IResponseParser result = null;

//            if (commandCode == LinkCommandCode.READ_TEMPERATURE)
//            {
//                result = new LinkTemperatureResponseParser();
//            }
//            else if (commandCode == LinkCommandCode.READ_FREQUENCY)
//            {
//                result = new LinkFrequencyResponseParser();
//            }
//            else if (commandCode == LinkCommandCode.READ_PWM)
//            {
//                result = new LinkFanPowerResponseParser();
//            }
//            else
//            {
//                result = new NullParser();
//            }

//            TraceMethodLeave();
//            return result;
//        }

//        private void UpdateDeviceEntity(LinkCommandCode command, byte address, object value)
//        {
//            Logger.Trace("UpdateDeviceEntity(command = {0})", command);
//            switch (command)
//            {
//                case LinkCommandCode.READ_TEMPERATURE:
//                    double temperatureValue = (double)value;
//                    if (address == 0)
//                    {
//                        deviceEntity.TemperatureI2C = temperatureValue;
//                        Logger.Debug("Update device entity: set TemperatureI2C = {0}", temperatureValue);
//                    }
//                    else
//                    {
//                        deviceEntity.TemperatureWire = temperatureValue;
//                        Logger.Debug("Update device entity: set TemperatureWire = {0}", temperatureValue);
//                    }

//                    break;
//                case LinkCommandCode.READ_FREQUENCY:
//                    int frequencyValue = (int)value;
//                    deviceEntity.Fans[address].Frequency = frequencyValue;
//                    Logger.Debug("Update device entity: set Fan[{0}].Frequency = {1}", address, frequencyValue);
//                    break;
//                case LinkCommandCode.READ_PWM:
//                    int powerValue = (byte)value;
//                    deviceEntity.Fans[address].Power = powerValue;
//                    Logger.Debug("Update device entity: set Fan[{0}].Power = {1}", address, powerValue);
//                    break;

//                default:
//                    break;
//            }

//            Logger.Trace("~UpdateDeviceEntity()");
//        }

//        private void LogDeviceInfo()
//        {
//            Logger.Debug(
//                "CLink updated: TemperatureI2C = {0}; TemperatureWire = {1}; FrequencyFan0 = {2}; FrequencyFan1 = {3}; FanPower0(1) = {4}/{5}",
//                deviceEntity.TemperatureI2C,
//                deviceEntity.TemperatureWire,
//                deviceEntity.Fans[0].Frequency,
//                deviceEntity.Fans[1].Frequency,
//                deviceEntity.Fans[0].Power,
//                deviceEntity.Fans[1].Power);
//        }
//    }
//}
