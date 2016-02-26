using CorsairLink4.Common.DevicesDefinitions.Coolit;
using System;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit
{
    public interface IFanController
    {
        Task SetCustomCurve(CoolitSensorAddress fanAddress, short rpm1, short rpm2, short rpm3, short rpm4, short rpm5, double temp1, double temp2, double temp3, double temp4, double temp5);

        Task SetFanManualTemperature(CoolitSensorAddress fanAddress, double temperature);

        Task SetFanMode(CoolitSensorAddress fanAddress, CoolitFanMode fanMode, byte tempChannel = 0xFF);

        Task<byte> GetFanMode(CoolitSensorAddress fanAddress);

        Task SetFanPWM(CoolitSensorAddress fanAddress, byte pwm);

        Task SetFanRPM(CoolitSensorAddress fanAddress, short rpm);
    }
}
