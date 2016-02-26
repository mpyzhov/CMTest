using System;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits
{
    public enum PsuCommandCode : byte
    {
        PAGE                    = 0x00,
        FAN_CONFIG_1_2          = 0x3A,
        FAN_COMMAND_1           = 0x3B,
        VOUT_OV_FAULT_LIMIT     = 0x40,
        VOUT_UV_FAULT_LIMIT     = 0x44,
        IOUT_OC_FAULT_LIMIT     = 0x46,
        OT_FAULT_LIMIT          = 0x4F,
        STATUS_VOUT             = 0x7A,
        STATUS_IOUT             = 0x7B,
        STATUS_TEMPERATURE      = 0x7D,
        STATUS_CML              = 0x7E,
        STATUS_FANS_1_2         = 0x81,
        READ_VIN                = 0x88,
        READ_VOUT               = 0x8B,
        READ_IOUT               = 0x8C,
        READ_TEMPERATURE_1      = 0x8D,
        READ_TEMPERATURE_2      = 0x8E,
        READ_FAN_SPEED_1        = 0x90,
        READ_POUT               = 0x96,
        READ_MFR_ID             = 0x99,
        READ_MFR_MODEL          = 0x9A,
        READ_RUNTIME_TOTAL      = 0xD1,
        READ_RUNTIME_NOW        = 0xD2,
        READ_FIRMWARE_REVISION  = 0xD4,
        MFR_12V_OCP_MODE        = 0xD8,
        SET_BLACKBOX_MODE       = 0xD9,
        MFR_RESET_USER_SETTING  = 0xDD,
        MFR_READ_TOTAL_POUT     = 0xEE,
        FAN_INDEX               = 0xF0,
        HANDSHAKE               = 0xFE,
    }
}
