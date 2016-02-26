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

            CommanderMiniTester.Test().Wait();

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
