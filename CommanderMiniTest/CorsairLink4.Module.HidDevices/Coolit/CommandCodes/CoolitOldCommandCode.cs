using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit.CommandCodes
{
    public enum CoolitOldCommandCode
    {
        DEVICE_ID,
        FW_VERSION,

        FAN_CURRENT_RPM,
        FAN_RPM,
        FAN_PWM,
        FAN_RPM_TABLE,
        FAN_TEMPERATURE_TABLE,
        FAN_MANUAL_TEMPERATURE,
        FAN_MODE_GET,
        FAN_MODE_SET,

        TEMPERATURE_CURRENT_TEMPERATURE,
    }
}
