using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public class ExceptionConsoleLog : ILog
    {
        private object lockObject;

        public ExceptionConsoleLog()
        {
            lockObject = new object();
        }

        public void Log(Exception ex)
        {

            lock (lockObject)
            {
                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), " ", ex.StackTrace));
                Console.ForegroundColor = defaultColor;
            }
        }
    }
}
