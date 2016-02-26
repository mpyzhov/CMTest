using HidDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommanderMiniTest
{
    public class CommanderMiniTester
    {
        private static int attempts = 1000;
        public static async Task Test()
        {
            HidDeviceComponent component = new HidDeviceComponent();


            for (int i = 0; i < attempts; i++)
            {
                // should be connected ONLY Commander Mini
                var device = component.EnumerateCoolitBridgeDevices().FirstOrDefault();

                if (device != null)
                {
                    await device.UpdateProperties();

                    if (device.IsInitialized)
                    {
                        Console.WriteLine("Initialized!");

                        Console.WriteLine("FW version: " + device.FwVersion);

                        Console.WriteLine("Fan count: " + device.FanCount);
                    }
                    else
                    {
                        Console.WriteLine("Can't initialize!");
                    }
                }
                else
                {
                    Console.WriteLine("Device is absent!");
                }

                //Console.WriteLine("1000 msec delay");
                await Task.Delay(1000);
                Console.WriteLine();
            }
        }
    } 
}
