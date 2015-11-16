using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace Scheduler
{
    public class ExceptionWindowsLog : ILog
    {
        #region Fields
        private string source = Assembly.GetEntryAssembly().GetName().Name;
        public const string DEFAULT_LOG = "Application";
        private string log = DEFAULT_LOG;
        private int evntId = 1000;
        private short evntCategory = 100;
        #endregion

        #region Constructors
        public ExceptionWindowsLog()
        {
            this.CreateSource();
        }


        public ExceptionWindowsLog(string source, string log = DEFAULT_LOG)
        {
            this.source = source;
            this.log = log;
            this.CreateSource();
        }
        #endregion

        private void CreateSource()
        {
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, log);
            }
        }

        /// <summary>
        /// Logs exception to Windows event log
        /// </summary>
        /// <param name="ex">Exception to be logged</param>
        public void Log(Exception ex)
        {
            Task.Run(() =>
            {
                EventLog.WriteEntry(this.source,
                                    ex.StackTrace,
                                    EventLogEntryType.Error,
                                    this.evntId,
                                    this.evntCategory);
            });
        }
    }
}
