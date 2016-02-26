using System;

namespace CorsairLink4.Module.HidDevices.Coolit
{
    public enum CoolitChannelTag : byte
    {
        BridgeDevice = 0x00,
        PSU          = 0x01,
        AFP          = 0x03,
        Active       = 0x05,
        NonPresent   = 0xFF
    }
}
