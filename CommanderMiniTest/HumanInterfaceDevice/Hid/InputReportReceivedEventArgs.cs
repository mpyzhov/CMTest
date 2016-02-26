using System;

namespace HumanInterfaceDevice
{
    /// <summary>
    /// Represents the arguments which the HID API sends as part of an input-report event.
    /// </summary>
    public sealed class InputReportReceivedEventArgs
    {
        /// <summary>
        /// An InputReport object
        /// </summary>
        public InputReport Report { get; set; }
    }
}
