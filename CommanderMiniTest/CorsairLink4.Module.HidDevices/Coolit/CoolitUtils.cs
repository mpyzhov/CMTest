using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorsairLink4.Common.DevicesDefinitions.Common;
using CorsairLink4.Common.DevicesDefinitions.Coolit;
using CorsairLink4.Module.HidDevices.Coolit.Sensors;

namespace CorsairLink4.Module.HidDevices.Coolit
{
    public static class CoolitUtils
    {
        private const int MaxTempCountWhiptailCoolingnode = 3;

        public static CoolitModel GetModelFromPid(string pid)
        {
            switch (pid)
            {
                case DeviceIdentity.Coolit.Pid.H80:
                    return CoolitModel.H80;
                case DeviceIdentity.Coolit.Pid.CoolingNode:
                    return CoolitModel.CoolingNode;
                case DeviceIdentity.Coolit.Pid.LightingNode:
                    return CoolitModel.LightingNode;
                case DeviceIdentity.Coolit.Pid.H100:
                    return CoolitModel.H100;
                case DeviceIdentity.Coolit.Pid.H80i:
                    return CoolitModel.H80i;
                case DeviceIdentity.Coolit.Pid.H100i:
                    return CoolitModel.H100i;
                case DeviceIdentity.Coolit.Pid.Whiptail:
                    return CoolitModel.Whiptail;
                case DeviceIdentity.Coolit.Pid.H100iGT:
                    return CoolitModel.H100iGT;
                case DeviceIdentity.Coolit.Pid.H110iGT:
                    return CoolitModel.H110iGT;
                case DeviceIdentity.Coolit.Pid.H110i:
                    return CoolitModel.H110i;
                default:
                    throw new Exception("Invalid model type");
            }
        }
        public static string GetPid(CoolitModel model)
        {
            switch (model)
            {
                case CoolitModel.H80:
                    return DeviceIdentity.Coolit.Pid.H80;
                case CoolitModel.CoolingNode:
                    return DeviceIdentity.Coolit.Pid.CoolingNode;
                case CoolitModel.LightingNode:
                    return DeviceIdentity.Coolit.Pid.LightingNode;
                case CoolitModel.H100:
                    return DeviceIdentity.Coolit.Pid.H100;
                case CoolitModel.H80i:
                    return DeviceIdentity.Coolit.Pid.H80i;
                case CoolitModel.H100i:
                    return DeviceIdentity.Coolit.Pid.H100i;
                case CoolitModel.Whiptail:
                    return DeviceIdentity.Coolit.Pid.Whiptail;
                case CoolitModel.H100iGT:
                    return DeviceIdentity.Coolit.Pid.H100iGT;
                case CoolitModel.H110iGT:
                    return DeviceIdentity.Coolit.Pid.H110iGT;
                case CoolitModel.H110i:
                    return DeviceIdentity.Coolit.Pid.H110i;
                default:
                    throw new Exception("Invalid model type");
            }
        }

        private static string GetName(string name, int currentPortNumber, bool doInvert = false, int maxPorts = 0)
        {
            return String.Format("{0} #{1}", name, (doInvert ? maxPorts - currentPortNumber : currentPortNumber) + 1);
        }

        public static string GetCoolitSensorName(CoolitSensorType sensorType, CoolitSensorAddress sensorChannel, CoolitModel model)
        {
            if (sensorType == CoolitSensorType.PumpRpm)
            {
                return GetSensorBaseName(sensorType);
            }
            else if (sensorType == CoolitSensorType.Led && model != CoolitModel.LightingNode)
            {
                // do not enumerating single led
                return GetSensorBaseName(sensorType);
            }
            else if (sensorType == CoolitSensorType.Temperature && (model == CoolitModel.H100i || model == CoolitModel.H80i || model == CoolitModel.H110iGT || model == CoolitModel.H110i))
            {
                // do not enumerating single temp
                return GetSensorBaseName(sensorType);
            }
            else
            {
                bool doInvert = sensorType == CoolitSensorType.Temperature && (model == CoolitModel.CoolingNode || model == CoolitModel.Whiptail);
                return GetName(GetSensorBaseName(sensorType), (int)sensorChannel, doInvert, MaxTempCountWhiptailCoolingnode);
            }
        }

        public static int GetMaxSensorsCount(CoolitModel model, CoolitSensorType type)
        {
            switch (model)
            {
                case CoolitModel.H80i: //temp - 1, fan - 4, pump - 1, led - 1
                case CoolitModel.H100i:
                    switch (type)
                    {
                        case CoolitSensorType.Temperature:
                            return 1;
                        case CoolitSensorType.PumpRpm:
                            return 1;
                        case CoolitSensorType.Led:
                            return 1;
                        case CoolitSensorType.FanRpm:
                            return 4;
                        default:
                            return 0;
                    }
                case CoolitModel.H110iGT: //temp - 1, fan - 2 , pump - 1, led - 1
                case CoolitModel.H110i:
                    switch (type)
                    {
                        case CoolitSensorType.Temperature:
                            return 1;
                        case CoolitSensorType.PumpRpm:
                            return 1;
                        case CoolitSensorType.Led:
                            return 1;
                        case CoolitSensorType.FanRpm:
                            return 2;
                        default:
                            return 0;
                    }
                case CoolitModel.LightingNode: // led - 2
                    switch (type)
                    {
                        case CoolitSensorType.Led:
                            return 2;
                        default:
                            return 0;
                    }
                case CoolitModel.Whiptail: // fan - 6 max, temp 4 max, “LED strip” always show one tile that represents LED strip 
                    //(even if it is not connected because there is no way to detect it)
                    switch (type)
                    {
                        case CoolitSensorType.Temperature:
                            return 4;
                        case CoolitSensorType.Led:
                            return 1;
                        case CoolitSensorType.FanRpm:
                            return 6;
                        default:
                            return 0;
                    }
                case CoolitModel.H80:
                case CoolitModel.H100:
                case CoolitModel.CoolingNode: //fan - 5max, temp - 4max,
                    switch (type)
                    {
                        case CoolitSensorType.Temperature:
                            return 4;
                        case CoolitSensorType.FanRpm:
                            return 5;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }

        public static string GetSensorBaseName(CoolitSensorType sensorType)
        {
            switch (sensorType)
            {
                case CoolitSensorType.FanRpm:
                    return SensorDefaultNames.FanSpeed;
                case CoolitSensorType.PumpRpm:
                    return SensorDefaultNames.Pump;
                case CoolitSensorType.Temperature:
                    return SensorDefaultNames.Temperature;
                case CoolitSensorType.Led:
                    return SensorDefaultNames.Led;
                default:
                    return string.Empty;
            }
        }
    }
}
