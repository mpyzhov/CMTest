using HumanInterfaceDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    internal class CommandStateMachine
    {
        private const int ErrorState = -1;
        //private static Logger logger = LogManager.GetCurrentClassLogger();
        private int state;
        private string errorMessage;
        private HidDevice device;
        private OutputReport outReport;
        private InputReport inReport;
        private byte[] result;
        private IReportFactory factory;
        private byte command;
        private IResponseValidator validator;
        private ReportDataExtractorVisitor extractor;

        public CommandStateMachine(HidDevice hidDevice, IReportFactory reportFactory, IResponseValidator responseValidator)
        {
            //logger.Trace("new CommandStateMachine()");

            device = hidDevice;
            factory = reportFactory;
            validator = responseValidator;
            extractor = new ReportDataExtractorVisitor();
        }

        public bool IsFaulted
        {
            get { return state == ErrorState; }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
        }

        public byte[] Result
        {
            get { return result; }
        }

        public async Task<bool> Run(byte commandCode)
        {
            //logger.Trace("Run(commandCode = {0})", commandCode);

            state = 0;
            command = commandCode;
            errorMessage = string.Empty;
            await MoveNext();

            //logger.Trace("~Run()");
            return state != ErrorState;
        }

        private void GoToErrorState(string error)
        {
            errorMessage = error;
            //System.Diagnostics.Debug.WriteLine("GoToErrorState: Command = {0}; Error = {1}", command, error);
            state = ErrorState;
        }

        private async Task MoveNext()
        {
            try
            {
                switch (state)
                {
                    case 0:
                        //logger.Trace("Open state");
                        device.Open(factory == null ? HidDeviceAccess.Read : HidDeviceAccess.ReadWrite);
                        if (device.IsOpen)
                        {
                            state++;
                        }
                        else
                        {
                            GoToErrorState("Cannot open device");
                        }

                        break;
                    case 1:
                        //logger.Trace("Send output report state");
                        if (factory != null)
                        {
                            outReport = factory.Create(command);
                            await device.SendOutputReport(outReport);
                        }

                        state++;
                        break;
                    case 2:
                        //logger.Trace("Get input report state");
                        inReport = await device.GetInputReport(0);
                        inReport.Data.Accept(validator);

                        if (validator.IsInvalid)
                        {
                            GoToErrorState("validation failure");
                            break;
                        }
                        else
                        {
                            inReport.Data.Accept(extractor);
                            result = extractor.Data;
                        }

                        state++;
                        break;
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                //logger.Error("Error MoveNext", ex);
                GoToErrorState(ex.ToString());
            }

            await MoveNext();
        }
    }
}
