using HidDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommanderMiniTest
{
    class Program
    {
        static void Main(string[] args)
        {
            HidDeviceComponent component = new HidDeviceComponent();

            foreach(var bridge in component.EnumerateCoolitBridgeDevices())
            {

            }

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
