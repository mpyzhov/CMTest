using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit.CommandCodes
{
    public enum LedNodeCommandOpCode
    {
        DEVICE_ID,
        FW_VERSION,
        SET_COLORS,
        SET_MANUAL_TEMPERATURE,
        SET_COLORS_AND_TEMPERATURES,
        SET_MODE,
        GET_MODE,
    }
}
