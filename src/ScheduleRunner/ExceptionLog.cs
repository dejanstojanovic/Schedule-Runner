using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace Scheduler
{
    public class ExceptionLog : ILog
    {
        //https://support.microsoft.com/en-us/kb/307024


        private string source = Assembly.GetEntryAssembly().GetName().Name;
        private string log = "Application";
        private string evnt = "Exception";


        public ExceptionLog()
        {
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, log);
            }
        }

        public void Log(Exception ex)
        {
            Task.Run(() =>
            {
                EventLog.WriteEntry(this.source, this.evnt, EventLogEntryType.Error);
            });
        }
    }
}
