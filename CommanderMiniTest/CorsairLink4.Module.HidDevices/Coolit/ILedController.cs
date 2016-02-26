using CorsairLink4.Common.DevicesDefinitions.Coolit;
using System;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit
{
    public interface ILedController
    {
        Task SetLedColorsAndTemperatures(CoolitSensorAddress ledAddress, byte r1, byte g1, byte b1, byte r2, byte g2, byte b2, byte r3, byte g3, byte b3, double t1, double t2, double t3);

        Task SetLedCycleColors(CoolitSensorAddress ledAddress, byte r1, byte g1, byte b1, byte r2, byte g2, byte b2, byte r3, byte g3, byte b3, byte r4, byte g4, byte b4);

        Task SetLedManualTemperature(CoolitSensorAddress ledAddress, double temperature);

        Task SetLedMode(CoolitSensorAddress ledAddress, CoolitLEDMode mode, byte tempChannel = 0xFF);

        Task<CoolitLEDMode> GetLedMode(CoolitSensorAddress ledAddress);
    }
}
