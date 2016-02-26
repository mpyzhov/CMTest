using HumanInterfaceDevice;
using System;
using System.Threading.Tasks;
using Extensions;

namespace HumanInterfaceDevice
{
    public abstract class HidDevice : IDisposable
    {
        private readonly string deviceInstancePath;
        private IHidDeviceManager manager;
        private string devicePath;
        private IDeviceHandle handle;

        private HidDeviceCapabilities capabilities;
        private System.IO.Stream stream;

        public HidDevice(string deviceID, IHidDeviceManager deviceManager)
        {
            manager = deviceManager;
            devicePath = new DevicePathBuilder(manager.GetHidGuid()).Build(deviceID);
            deviceInstancePath = deviceID;

            var devid = deviceID.ToLowerInvariant();
            ProductId = ParseHexId(devid, devid.IndexOf("pid"));
            VendorId = ParseHexId(devid, devid.IndexOf("vid"));
            Version = ParseHexId(devid, devid.IndexOf("rev"));
        }

        public bool IsOpen
        {
            get { return handle != null && handle.IsInvalid == false; }
        }

        public HidDeviceCapabilities Capabilities
        {
            get { return capabilities; }
        }

        public string DeviceInstancePath
        {
            get
            {
                return devicePath;
            }
        }

        /// <summary>
        /// Gets the usage identifier for the given HID device.
        /// </summary>
        public ushort UsageId
        {
            get { return capabilities.Usage; }
        }

        /// <summary>
        /// Gets the usage page of the top-level collection.
        /// </summary>
        public ushort UsagePage
        {
            get { return capabilities.UsagePage; }
        }

        /// <summary>
        /// Gets the vendor identifier for the given HID device.
        /// </summary>
        public ushort VendorId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the product identifier for the given HID device.
        /// </summary>
        public ushort ProductId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the version, or revision, number for the given HID device.
        /// </summary>
        public ushort Version
        {
            get;
            private set;
        }

        public ushort VersionNumber
        {
            get;
            private set;
        }

        public HidDeviceAccess AccessLevel
        {
            get;
            private set;
        }

        public void Open(HidDeviceAccess desiredAccess)
        {
            handle = manager.OpenDevice(devicePath, desiredAccess);
            if (handle.IsInvalid)
            {
                //Logger.Error("Cannot open device");
                return;
            }

            AccessLevel = desiredAccess;

            capabilities = manager.GetDeviceCapabilities(handle);
            VersionNumber = capabilities.VersionNumber;

            stream = manager.GetDeviceStream(handle);
        }

        public async Task<InputReport> GetInputReport(byte reportId = 0)
        {
            byte[] tmp = null;

            if (AccessLevel == HidDeviceAccess.Read)
            {
                tmp = manager.GetInputReport(handle, capabilities.InputReportByteLength);
            }
            else
            {
                tmp = new byte[capabilities.InputReportByteLength];
                await stream.ReadAsync(tmp, 0, tmp.Length).WithTimeout(TimeSpan.FromMilliseconds(500));
            }

            return new InputReport(tmp);
        }

        public OutputReport CreateOutputReport(byte reportId = 0)
        {
            OutputReport outputReport = new OutputReport(capabilities.OutputReportByteLength);
            return outputReport;
        }

        public Task SendOutputReport(OutputReport outputReport)
        {
            var visitor = new ReportDataExtractorVisitor();
            outputReport.Data.Accept(visitor);

            return stream.WriteAsync(visitor.Data, 0, visitor.Data.Length).WithTimeout(TimeSpan.FromMilliseconds(500));
        }

        public void Dispose()
        {
            try
            {
                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }

                if (handle != null)
                {
                    handle.Dispose();
                }
            }
            catch (Exception)
            {
                // 
            }
        }

        private ushort ParseHexId(string s, int nameStartIndex)
        {
            if (nameStartIndex >= 0)
            {
                return ushort.Parse(s.Substring(nameStartIndex + 4, 4), System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                return 0;
            }
        }
    }
}
