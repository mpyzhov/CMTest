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
            uint count = 0;
            bool hasArgs = args.Count() > 0;
            if (!hasArgs || !uint.TryParse(args[0], out count))
                count = 1000;

            CommanderMiniTester.Test(count).Wait();

            if (!hasArgs)
            {
                Console.WriteLine("End");
                Console.WriteLine("Succeeded: " + TaskExt.AttemptsSucceeded);
                Console.WriteLine("Failed: " + TaskExt.AttemptsFailed);
                Console.WriteLine("Percentage: {0}% success", TaskExt.AttemptsSucceeded * 100 / (TaskExt.AttemptsSucceeded + TaskExt.AttemptsFailed));

                Console.ReadLine();
            }
        }
    }
}
