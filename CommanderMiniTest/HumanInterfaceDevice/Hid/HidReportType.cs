using System;

namespace HumanInterfaceDevice
{
    /// <summary>
    /// Specifies a HID report type.
    /// </summary>
    public enum HidReportType
    {
        /// <summary>
        /// The report is an input report.
        /// </summary>
        Input = 0,

        /// <summary>
        /// The report is an output report.
        /// </summary>
        Output = 1,

        /// <summary>
        /// The report is a feature report.
        /// </summary>
        Feature = 2,
    }
}
