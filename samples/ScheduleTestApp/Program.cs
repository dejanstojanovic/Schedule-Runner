using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //var runner = new Scheduler.ScheduleRunner(new Scheduler.ExceptionWindowsLog());
            var runner = new Scheduler.ScheduleRunner(new Scheduler.ExceptionConsoleLog());

            Console.ReadLine();
        }
    }
}
