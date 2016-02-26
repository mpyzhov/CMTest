using CorsairLink4.Module.HidDevices.PowerSupplyUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits
{
    public static class CommandLengthHelper
    {
        private static readonly HashSet<Tuple<HashSet<PsuCommandCode>, int>> commandLengthData = new HashSet<Tuple<HashSet<PsuCommandCode>, int>>() {
            // single byte
            Tuple.Create(
                new HashSet<PsuCommandCode>() {
                    PsuCommandCode.PAGE,
                    PsuCommandCode.FAN_COMMAND_1,
                    PsuCommandCode.MFR_12V_OCP_MODE,
                    PsuCommandCode.FAN_INDEX,
                }, 1),
            // half precision double, 2 bytes
            Tuple.Create(
                new HashSet<PsuCommandCode>() {
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
                }, 2),
            // strings
            Tuple.Create(
                new HashSet<PsuCommandCode>() {
                    PsuCommandCode.READ_MFR_MODEL,
                    PsuCommandCode.READ_MFR_ID,
                }, 7),
        };

        public static byte GetCommandArgLength(byte id)
        {
            var code = (PsuCommandCode)id;
            return (byte)commandLengthData.Where(pr => pr.Item1.Contains(code)).First().Item2;
        }
    }
}
