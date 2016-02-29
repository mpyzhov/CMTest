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
        public static async Task Test(uint internalAttempts  = 1000)
        {
            HidDeviceComponent component = new HidDeviceComponent();

            for (int i = 0; i < internalAttempts; i++)
            {
                Console.WriteLine("----Test #{0}", i + 1);
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
                await Task.Delay(10);
                Console.WriteLine();
            }
        }
    } 
}
