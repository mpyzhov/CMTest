using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static class TaskExt
    {
        public static async Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)))
            {
                return await task;
            }
            Console.WriteLine("Timeout!!!");
            throw new TimeoutException();
        }

        public static async Task WithTimeout(this Task task, TimeSpan timeout)
        {
            if (task != await Task.WhenAny(task, Task.Delay(timeout)))
            {
                Console.WriteLine("Timeout!!!");
                throw new TimeoutException();
            }
        }
    }
}
