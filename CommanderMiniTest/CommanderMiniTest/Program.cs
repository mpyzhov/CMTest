using Extensions;
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
            Console.WriteLine("Succeeded: " + TaskExt.AttemptsSucceeded);
            Console.WriteLine("Failed: " + TaskExt.AttemptsFailed);
            Console.WriteLine("Percentage: {0}% success", TaskExt.AttemptsSucceeded * 100 / (TaskExt.AttemptsSucceeded + TaskExt.AttemptsFailed));
            Console.ReadLine();
        }
    }
}
