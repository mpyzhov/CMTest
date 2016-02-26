using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace HumanInterfaceDevice
{
    public class HidDeviceManager : IHidDeviceManager
    {
        private const uint GenericRead = 0x80000000;
        private const uint GenericWrite = 0x40000000;

        public Guid GetHidGuid()
        {
            Guid hidGuid = Guid.Empty;
            HidD_GetHidGuid(ref hidGuid);
            return hidGuid;
        }

        public IDeviceHandle OpenDevice(string devicePath, HidDeviceAccess access)
        {
            var desiredAccess = access == HidDeviceAccess.Read ? GenericRead :
                                access == HidDeviceAccess.Write ? GenericWrite :
                                GenericRead | GenericWrite;
            var safeHandle = CreateFile(devicePath, desiredAccess, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            return new DeviceHandle(safeHandle);
        }

        public HidDeviceCapabilities GetDeviceCapabilities(IDeviceHandle deviceHandle)
        {
            var safeHandle = (deviceHandle as DeviceHandle).SafeFileHandle;
            IntPtr preparsedData = new IntPtr();
            HidD_GetPreparsedData(safeHandle, ref preparsedData);
            HIDP_CAPS caps = new HIDP_CAPS();
            int result = HidP_GetCaps(preparsedData, ref caps);
            if (preparsedData != IntPtr.Zero)
            {
                HidD_FreePreparsedData(preparsedData);
            }

            HIDD_ATTRIBUTES attrib = new HIDD_ATTRIBUTES();
            attrib.Size = Marshal.SizeOf(attrib);
            HidD_GetAttributes(safeHandle, ref attrib);

            return new HidDeviceCapabilities
            {
                Usage = (ushort)caps.Usage,
                UsagePage = (ushort)caps.UsagePage,
                InputReportByteLength = (ushort)caps.InputReportByteLength,
                OutputReportByteLength = (ushort)caps.OutputReportByteLength,
                VersionNumber = (ushort)attrib.VersionNumber
            };
        }

        public Stream GetDeviceStream(IDeviceHandle deviceHandle)
        {
            var safeHandle = (deviceHandle as DeviceHandle).SafeFileHandle;
            return new FileStream(safeHandle, FileAccess.ReadWrite);
        }

        public byte[] GetInputReport(IDeviceHandle deviceHandle, int reportSize)
        {
            var safeHandle = (deviceHandle as DeviceHandle).SafeFileHandle;
            byte[] reportBuffer = new byte[reportSize];
            if (HidD_GetInputReport(safeHandle, reportBuffer, reportSize))
            {
                return reportBuffer;
            }

            return null;
        }

        #region Native declarations

        [DllImport("hid.dll")]
        private static extern void HidD_GetHidGuid(ref Guid hidGuid);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern int HidP_GetCaps(IntPtr preparsedData, ref HIDP_CAPS capabilities);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidD_GetPreparsedData(SafeFileHandle hidDeviceObject, ref IntPtr preparsedData);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidD_GetInputReport(SafeFileHandle hidDeviceObject, byte[] reportBuffer, int reportBufferLength);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidD_GetAttributes(SafeFileHandle hidDeviceObject, ref HIDD_ATTRIBUTES attributes);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidD_FreePreparsedData(IntPtr preparsedData);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern SafeFileHandle CreateFile(
            string fileName,
            uint desiredAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare shareMode,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias")]
        private struct HIDP_CAPS
        {
            internal Int16 Usage;
            internal Int16 UsagePage;
            internal Int16 InputReportByteLength;
            internal Int16 OutputReportByteLength;
            internal Int16 FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            internal Int16[] Reserved;
            internal Int16 NumberLinkCollectionNodes;
            internal Int16 NumberInputButtonCaps;
            internal Int16 NumberInputValueCaps;
            internal Int16 NumberInputDataIndices;
            internal Int16 NumberOutputButtonCaps;
            internal Int16 NumberOutputValueCaps;
            internal Int16 NumberOutputDataIndices;
            internal Int16 NumberFeatureButtonCaps;
            internal Int16 NumberFeatureValueCaps;
            internal Int16 NumberFeatureDataIndices;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HIDD_ATTRIBUTES
        {
            internal Int32 Size;
            internal Int16 VendorID;
            internal Int16 ProductID;
            internal Int16 VersionNumber;
        }

        #endregion // Native declarations
    }
}
