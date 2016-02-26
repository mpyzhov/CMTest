using CorsairLink4.Common.DevicesDefinitions.Common;
using CorsairLink4.Common.Shared.Communication;
using CorsairLink4.Common.Shared.DevicesData;
using CorsairLink4.Common.Shared.Settings;
using CorsairLink4.Common.Shared.Utils;
using CorsairLink4.Module.HidDevices.Barbuda.Core;
using CorsairLink4.Module.HidDevices.Barbuda.Mocks;
using CorsairLink4.Module.HidDevices.Coolit;
using CorsairLink4.Module.HidDevices.Mocks.Coolit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Mocks
{
    public class HidDeviceComponentMock : IHidDeviceComponent
    {
        private static readonly List<HXPowerSupplyUnitMock> psuDevices = new List<HXPowerSupplyUnitMock>();
        private static readonly List<CoolitCoolerDeviceMock> coolitDevices = new List<CoolitCoolerDeviceMock>();
        private static readonly List<RobbinsPsuDeviceMock> robbinsDevices = new List<RobbinsPsuDeviceMock>();

        private readonly BarbudaDevicesEnumeratorMock barbudaEnumerator = new BarbudaDevicesEnumeratorMock();
        private readonly BarbudaDeviceComponent barbudaComponent;

        private IEnumerator<Tuple<string, DeviceInstance>> deviceHXInfo = EnumerateDeviceHXInfos().GetEnumerator();

        public HidDeviceComponentMock()
        {
            barbudaComponent = new BarbudaDeviceComponent(barbudaEnumerator);
        }

        public void AddFakeHXPowerSupplyUnit(int count)
        {
            Enumerable.Repeat(0, count).Select(x => AddFakePSUDevice()).ToArray();
        }

        public void AddFakeBarbudaDevices(int count)
        {
            for (int i = 0; i < count; i++)
            {
                barbudaEnumerator.AddMockDevice("Barbuda serial " + i.ToString(), true);
            }
        }

        public DeviceInstance AddFakePSUDevice()
        {
            deviceHXInfo.MoveNext();
            var device = deviceHXInfo.Current;
            psuDevices.Add(new HXPowerSupplyUnitMock(device.Item2, device.Item1));
            return device.Item2;
        }

        public DeviceInstance AddFakeRobbinsDevice()
        {
            string serial = "FakeRmPsu" + robbinsDevices.Count.ToString();
            DeviceInstance instance = new DeviceInstance(new DeviceDistinguishInfo(DeviceIdentity.Corsair.Pid.RobbinsPSU_RM, DeviceIdentity.Corsair.Vid), serial);
            robbinsDevices.Add(new RobbinsPsuDeviceMock(instance));
            return instance;
        }

        public void AddFakeCoolitDevices(IEnumerable<CoolitModel> models)
        {
            foreach (var coolitModel in models)
            {
                AddFakeCoolitDevice(coolitModel);
            }
        }

        public DeviceInstance AddFakeCoolitDevice(CoolitModel model)
        {
            string serial = "FakeCoolit" + coolitDevices.Count.ToString();
            DeviceInstance instance = new DeviceInstance(CoolitCoolerDeviceMock.GetInfo(model), serial);
            coolitDevices.Add(new CoolitCoolerDeviceMock(instance, model));
            return instance;
        }

        public Task Accept(ISensorVisitor visitor)
        {
            Action<IDeviceComponent> accept = d => d.Accept(visitor);
            psuDevices.ForEach(accept);
            coolitDevices.ForEach(accept);
            robbinsDevices.ForEach(accept);
            barbudaComponent.Accept(visitor);

            return CompletedTask.Empty;
        }

        public Task ProcessControlData(DeviceCommunicationData data)
        {
            var device = psuDevices.FirstOrDefault(d => d.InstanceKey.Equals(data.DeviceInfo));
            if (device != null)
            {
                device.ProcessRequest(data);
            }
            barbudaComponent.ProcessControlData(data);

            return CompletedTask.Empty;
        }

        public Task SetAllFans100Percents()
        {
            foreach (var dev in psuDevices)
            {
                dev.SetFan100Percent();
            }
            barbudaComponent.SetAllFans100Percents();

            return CompletedTask.Empty;
        }

        public Task SetAllLeds(RgbColor color)
        {
            barbudaComponent.SetAllLeds(color);
            return CompletedTask.Empty;
        }

        public void RegisterGroups(IGroupsService groups)
        {
            barbudaComponent.RegisterGroups(groups);
        }

        private static IEnumerable<Tuple<string,DeviceInstance>> EnumerateDeviceHXInfos()
        {
            while (true)
            {
                yield return new Tuple<string,DeviceInstance>("HX1200i", new DeviceInstance(DeviceIdentity.Corsair.Pid.CorsairPSU_HX1200i, DeviceIdentity.Corsair.Vid, "fake_serial" + psuDevices.Count.ToString()));
                yield return new Tuple<string, DeviceInstance>("HX1000i", new DeviceInstance(DeviceIdentity.Corsair.Pid.CorsairPSU_HX1000i, DeviceIdentity.Corsair.Vid, "fake_serial" + psuDevices.Count.ToString()));
                yield return new Tuple<string, DeviceInstance>("HX850i", new DeviceInstance(DeviceIdentity.Corsair.Pid.CorsairPSU_HX850i, DeviceIdentity.Corsair.Vid, "fake_serial" + psuDevices.Count.ToString()));
                yield return new Tuple<string, DeviceInstance>("HX750i", new DeviceInstance(DeviceIdentity.Corsair.Pid.CorsairPSU_HX750i, DeviceIdentity.Corsair.Vid, "fake_serial" + psuDevices.Count.ToString()));
                yield return new Tuple<string, DeviceInstance>("HX650i", new DeviceInstance(DeviceIdentity.Corsair.Pid.CorsairPSU_HX650i, DeviceIdentity.Corsair.Vid, "fake_serial" + psuDevices.Count.ToString()));
            }
        }
    }
}
