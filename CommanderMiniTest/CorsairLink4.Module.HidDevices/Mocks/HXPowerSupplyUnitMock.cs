using CorsairLink4.Common.DevicesDefinitions.Common;
using CorsairLink4.Common.DevicesDefinitions.PowerSupplyUnit;
using CorsairLink4.Common.Shared.Communication;
using CorsairLink4.Common.Shared.DataSerialization;
using CorsairLink4.Common.Shared.DataSerialization.PowerSupplyUnit;
using CorsairLink4.Common.Shared.DevicesData;
using CorsairLink4.Common.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Mocks
{
    internal class HXOutputPage
    {
        private DeviceCommunicationDataFactory factory;
        public PowerSupplyUnitPageAddress addr { get; set; }

        public HXOutputPage(DeviceCommunicationDataFactory fact, PowerSupplyUnitPageAddress address)
        {
            this.factory = fact;
            this.addr = address;
        }

        private double Voltage
        {
            get
            {
                return DoGenerate(new[] { 12.0, 5.0, 3.3 });
            }
        }

        private double OverVoltage
        {
            get
            {
                return DoGenerate(new[] { 22.0, 15.0, 9.3 });
            }
        }

        private double UnderVoltage
        {
            get
            {
                return DoGenerate(new[] { 2.0, 1.0, 0.3 });
            }
        }

        private double Current
        {
            get
            {
                return DoGenerate(new[] { 8.0, 3.0, 1.0 });
            }
        }


        public IEnumerable<DeviceCommunicationData> EnumerateSensors()
        {
            yield return factory.CreateData(Voltage, PowerSupplyUnitSensorType.OutputVoltage, addr);
            yield return factory.CreateData(UnderVoltage, PowerSupplyUnitSensorType.OutputVoltageUnderVoltageFaultLimit, addr);
            yield return factory.CreateData(OverVoltage, PowerSupplyUnitSensorType.OutputVoltageOverVoltageFaultLimit, addr);
            yield return factory.CreateData(Current, PowerSupplyUnitSensorType.OutputCurrent, addr);
            yield return factory.CreateData(Voltage * Current, PowerSupplyUnitSensorType.OutputPower, addr);
        }


        private double DoGenerate(double[] vals)
        {
            double val = 0;
            switch (addr)
            {
                case PowerSupplyUnitPageAddress.Output12V:
                    val = vals[0];
                    break;
                case PowerSupplyUnitPageAddress.Output5V:
                    val = vals[1];
                    break;
                case PowerSupplyUnitPageAddress.Output3V:
                    val = vals[2];
                    break;
                default:
                    break;
            }
            return DeviationGenerator.Generate(val);

        }
    }

    internal class HXPowerSupplyUnitMock : IDeviceComponent
    {
        private readonly DeviceCommunicationDataFactory dataCreator;
        private readonly PowerSupplyUnitPathTokenParser tokenParser = PowerSupplyUnitPathTokenParser.Instance;
        private double temperature = 26.0;
        private byte fanPower = 40;
        private double rpmPerPerPowerUnit = 18.0;
        private int fanMode = 0;
        private double voltageIn = 220.0;
        private double powerOut = 130.0;
        private string model;
        private List<HXOutputPage> pages;

        public HXPowerSupplyUnitMock(DeviceInstance inst, string model)
        {
            InstanceKey = inst;
            dataCreator = new DeviceCommunicationDataFactory(inst, tokenParser);
            this.model = model;
            pages = new List<HXOutputPage>()
            {
                new HXOutputPage(dataCreator, PowerSupplyUnitPageAddress.Output12V),
                new HXOutputPage(dataCreator, PowerSupplyUnitPageAddress.Output5V),
                new HXOutputPage(dataCreator, PowerSupplyUnitPageAddress.Output3V),
            };
        }

        public DeviceInstance InstanceKey { get; private set; }

        public Task Accept(ISensorVisitor visitor)
        {
            DoVisit(visitor);
            return CompletedTask.Empty;
        }

        public void ProcessRequest(DeviceCommunicationData data)
        {
            var controller = tokenParser.GetEnumValue<PowerSupplyUnitControllerType>(data.DevicePathToken);
            switch (controller)
            {
                case PowerSupplyUnitControllerType.OCPMode:
                    break;
                case PowerSupplyUnitControllerType.FanMode:
                    var mode = ByteDataDeserializer.ConvertEnumFromBytes<PowerSupplyUnitFanModeType>(data.Data);
                    fanMode = mode == PowerSupplyUnitFanModeType.Normal ? 0 : 1;
                    break;
                case PowerSupplyUnitControllerType.FanPower:
                    var power = ByteDataDeserializer.ConvertSingleByteFromBytes(data.Data);
                    fanPower = power;
                    break;
                default:
                    break;
            }
        }

        public void SetFan100Percent()
        {
            fanMode = 1;
            fanPower = 100;
        }

        private double FanSpeed { get { return fanPower * rpmPerPerPowerUnit * fanMode; } }

        private void DoVisit(ISensorVisitor visitor)
        {
            foreach (var sens in EnumerateSensors())
                visitor.Visit(sens);
        }

        private IEnumerable<DeviceCommunicationData> EnumerateSensors()
        {
            yield return dataCreator.CreateData(model, PowerSupplyUnitSensorType.ManufacturersModel);
            yield return dataCreator.CreateData(temperature, PowerSupplyUnitSensorType.Temperature);
            yield return dataCreator.CreateData(FanSpeed, PowerSupplyUnitSensorType.FanSpeed);
            yield return dataCreator.CreateData(voltageIn, PowerSupplyUnitSensorType.InputVoltage);
            yield return dataCreator.CreateData(powerOut, PowerSupplyUnitSensorType.PowerOutTotal);
            foreach (var pg in pages)
                foreach (var data in pg.EnumerateSensors())
                    yield return data;
        }
    }

}
