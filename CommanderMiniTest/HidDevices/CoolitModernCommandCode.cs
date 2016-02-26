using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    public enum CoolitModernCommandCode : byte
    {
        DEVICE_ID,
        FW_VERSION,

        // Fan
        FAN_CURRENT_RPM,
        FAN_MODE_GET,
        FAN_MODE_SET,
        FAN_PWM,
        FAN_RPM,
        FAN_RPM_TABLE,
        FAN_TEMPERATURE_TABLE,
        FAN_MANUAL_TEMPERATURE,

        // Led
        LED_CURRENT_COLOR,
        LED_MODE_GET,
        LED_MODE_SET,
        LED_MANUAL_TEMPERATURE,
        LED_CYCLE_COLORS,
        LED_TEMPERATURE_MODE_COLORS,
        LED_TEMPERATURE_MODE_TEMPERATURES,

        // Temperature
        TEMPERATURE_CURRENT_TEMPERATURE,

        CURRENT_FAN,
        CURRENT_TEMPERATURE,
        CURRENT_LED,
        NUMBER_OF_TEMPERATURES,
        NUMBER_OF_FANS,
        NUMBER_OF_LEDS,
    }
}
