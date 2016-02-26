using CorsairLink4.Module.HidDevices.Core;
using HumanInterfaceDevice;
using HumanInterfaceDevice.Types;
using System.Diagnostics.Contracts;

namespace CorsairLink4.Module.HidDevices.PowerSupplyUnits
{
    public enum HidCommandMode
    {
        Read,
        Write
    }
    
    public class PsuOutputReportFactory : IReportFactory
    {
        private readonly byte outputReportId;
        private IBufferVisitor busWrite, busRead, handshake;
        private ArgWriteSingleByteVisitor fanCommandVisitor = new ArgWriteSingleByteVisitor();
        private ArgWriteSingleByteVisitor fanIndexVisitor = new ArgWriteSingleByteVisitor();
        private ArgWriteSingleByteVisitor pageVisitor = new ArgWriteSingleByteVisitor();
        private ArgWriteSingleByteVisitor ocpModeVisitor = new ArgWriteSingleByteVisitor();
        private CommandCodeWriterVisitor commandCode;
        private HidDevice device;

        public PsuOutputReportFactory(byte busAddress, HidDevice hidDevice, byte reportId = 0)
        {
            Contract.Requires(hidDevice != null);
            device = hidDevice;
            outputReportId = reportId;
            handshake = new HandshakeVisitor() { Address = busAddress };
            commandCode = new CommandCodeWriterVisitor();
            busWrite = new BusWriteVisitor() { Address = busAddress };
            busRead = new BusReadVisitor() { Address = busAddress };
        }

        public HidCommandMode PageMode { get; set; }

        public HidCommandMode FanCommandMode { get; set; }

        public HidCommandMode OCPModeMode { get; set; }

        public byte FanCommandArgument
        {
            get { return fanCommandVisitor.Arg; }
            set { fanCommandVisitor.Arg = value; }
        }

        public byte PageArgument
        {
            get { return pageVisitor.Arg; }
            set { pageVisitor.Arg = value; }
        }

        public byte OCPModeArgument
        {
            get { return ocpModeVisitor.Arg; }
            set { ocpModeVisitor.Arg = value; }
        }

        public byte FanIndexAgrument
        {
            get { return fanIndexVisitor.Arg; }
            set { fanIndexVisitor.Arg = value; }
        }

        OutputReport IReportFactory.Create(byte id)
        {
            return Create((PsuCommandCode)id);
        }

        public OutputReport Create(PsuCommandCode code)
        {
            var report = device.CreateOutputReport(outputReportId);
            commandCode.Code = code;
            report.Data.Accept(commandCode);
            switch (code)
            {
                case PsuCommandCode.PAGE:
                    if (PageMode == HidCommandMode.Write)
                    {
                        report.Data.Accept(busWrite);
                        report.Data.Accept(pageVisitor);
                    }

                    break;
                case PsuCommandCode.FAN_CONFIG_1_2:
                    break;
                case PsuCommandCode.FAN_COMMAND_1:
                    if (FanCommandMode == HidCommandMode.Read)
                    {
                        // for reading fan power
                        report.Data.Accept(busRead);
                    }
                    else
                    {
                        // for setting fan power
                        report.Data.Accept(busWrite);
                        report.Data.Accept(fanCommandVisitor);
                    }

                    break;
                case PsuCommandCode.VOUT_OV_FAULT_LIMIT:
                    report.Data.Accept(busRead);
                    break;
                case PsuCommandCode.VOUT_UV_FAULT_LIMIT:
                    report.Data.Accept(busRead);
                    break;
                case PsuCommandCode.IOUT_OC_FAULT_LIMIT:
                    report.Data.Accept(busRead);
                    break;
                case PsuCommandCode.OT_FAULT_LIMIT:
                    report.Data.Accept(busRead);
                    break;
                case PsuCommandCode.STATUS_VOUT:
                    break;
                case PsuCommandCode.STATUS_IOUT:
                    break;
                case PsuCommandCode.STATUS_TEMPERATURE:
                    break;
                case PsuCommandCode.STATUS_CML:
                    break;
                case PsuCommandCode.STATUS_FANS_1_2:
                    break;
                case PsuCommandCode.READ_VIN:
                    report.Data.Accept(busRead);
                    break;
                case PsuCommandCode.READ_VOUT:
                    report.Data.Accept(busRead);
                    break;
                case PsuCommandCode.READ_IOUT:
                    report.Data.Accept(busRead);
                    break;
                case PsuCommandCode.READ_TEMPERATURE_1:
                    report.Data.Accept(busRead);
                    break;
                case PsuCommandCode.READ_TEMPERATURE_2:
                    report.Data.Accept(busRead);
                    break;
                case PsuCommandCode.READ_FAN_SPEED_1:
                    report.Data.Accept(busRead);
                    break;
                case PsuCommandCode.READ_POUT:
                    report.Data.Accept(busRead);
                    break;
                case PsuCommandCode.READ_MFR_ID:
                    report.Data.Accept(busRead);
                    break;
                case PsuCommandCode.READ_MFR_MODEL:
                    report.Data.Accept(busRead);
                    break;
                case PsuCommandCode.READ_RUNTIME_TOTAL:
                    break;
                case PsuCommandCode.READ_RUNTIME_NOW:
                    break;
                case PsuCommandCode.READ_FIRMWARE_REVISION:
                    report.Data.Accept(busRead);
                    break;
                case PsuCommandCode.MFR_12V_OCP_MODE:
                    if (OCPModeMode == HidCommandMode.Read)
                    {
                        report.Data.Accept(busRead);
                    }
                    else
                    {
                        report.Data.Accept(busWrite);
                        report.Data.Accept(ocpModeVisitor);
                    }

                    break;
                case PsuCommandCode.SET_BLACKBOX_MODE:
                    break;
                case PsuCommandCode.MFR_RESET_USER_SETTING:
                    break;
                case PsuCommandCode.MFR_READ_TOTAL_POUT:
                    report.Data.Accept(busRead);
                    break;
                case PsuCommandCode.FAN_INDEX:
                    report.Data.Accept(busWrite);
                    report.Data.Accept(fanIndexVisitor);
                    break;
                case PsuCommandCode.HANDSHAKE:
                    report.Data.Accept(handshake);
                    break;
                default:
                    break;
            }

            ////LogOutputReport(report, code);
            return report;
        }

        //private void LogOutputReport(OutputReport report, PsuCommandCode command)
        //{
        //    if (logger.IsDebugEnabled)
        //    {
        //        StringBuilder repoBuilder = new StringBuilder();
        //        repoBuilder.Append(string.Format(
        //            "New OutputReport for command {0}; report size = {1}: ",
        //            command,
        //            report.Data.Length));
        //        for (int i = 0; i < report.Data.Length; i++)
        //        {
        //            repoBuilder.Append(string.Format("|{0}|", ByteToBinaryString(report.Data.ElementAt(i))));
        //        }

        //        logger.Debug(repoBuilder.ToString());
        //    }
        //}

        //private string ByteToBinaryString(byte bt)
        //{
        //    return Convert.ToString(bt, 2).PadLeft(8, '0');
        //}
    }
}
