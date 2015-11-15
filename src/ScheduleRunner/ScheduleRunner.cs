using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.ComponentModel;

namespace Scheduler
{
    public class ScheduleRunner
    {
        #region Fields
        private string schedulesFolder;
        private string schedulesNameStartWith;
        private const string SCHEDULE_NAME_START = "Schedule.";
        private List<ScheduleUnit> schedules;
        private object locker = new object();
        private ILog exceptionLogger = null;
        #endregion

        #region Properties
        /// <summary>
        /// List of schedule units loaded from the task folder
        /// </summary>
        public IEnumerable<ScheduleUnit> Schedules
        {
            get
            {
                return this.schedules;
            }
        }

        /// <summary>
        /// Instance of the exception logging class implementing ILog interface
        /// </summary>
        public ILog ExceptionLogger
        {
            get
            {
                return this.exceptionLogger;
            }
        }

        /// <summary>
        /// Folder which will contain the task libraries
        /// </summary>
        public string SchedulesFolder
        {
            get
            {
                return this.schedulesFolder;
            }
        }

        /// <summary>
        /// String value with which task libraries file names start with
        /// </summary>
        public string SchedulesNameStartWith
        {
            get
            {
                return this.schedulesNameStartWith;
            }
        }

        #endregion

        #region Constructors

        public ScheduleRunner()
        {
            this.schedulesFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Schedules");
            this.schedulesNameStartWith = SCHEDULE_NAME_START;
            this.Initialize();
        }

        public ScheduleRunner(ILog exceptionLogger):this()
        {
            this.exceptionLogger = exceptionLogger;
        }

        public ScheduleRunner(ILog exceptionLogger, string schedulesFolder, string schedulesNameStartWith = SCHEDULE_NAME_START) : this(schedulesFolder, schedulesNameStartWith)
        {
            this.exceptionLogger = exceptionLogger;
        }

        public ScheduleRunner(string schedulesFolder, string schedulesNameStartWith = SCHEDULE_NAME_START)
        {
            this.schedulesFolder = schedulesFolder;
            this.schedulesNameStartWith = schedulesNameStartWith;
            this.Initialize();
        }

        #endregion

        private void Initialize()
        {
            if (Directory.Exists(this.schedulesFolder))
            {
                if (LoadSchedules() > 0)
                {
                    Task.Run(() =>
                    {
                        while (true)
                        {
                            foreach (ScheduleUnit schedule in this.Schedules)
                            {
                                if ((DateTime.Now - schedule.StartAt).Seconds == 0 && (schedule.Schedule.EndTime == null || schedule.Schedule.EndTime > DateTime.Now))
                                {
                                    lock (locker)
                                    {
                                        schedule.StartAt = schedule.StartAt.AddSeconds(schedule.Schedule.PeriodSeconds);
                                    }

                                    Task.Run(() =>
                                    {
                                        schedule.Schedule.GetInstance().Start();
                                    }).ContinueWith((t) =>
                                    {
                                        if (this.ExceptionLogger != null)
                                        {
                                            AggregateException aggregateException = t.Exception;
                                            if (aggregateException != null && aggregateException.InnerException != null)
                                            {
                                                this.ExceptionLogger.Log(aggregateException.InnerException);
                                            }
                                        }
                                    }, TaskContinuationOptions.OnlyOnFaulted);
                                }
                            }

                        }
                    });
                }
            }
            else
            {
                throw new DirectoryNotFoundException(string.Format("Folder \"{0}\" does not exists", this.SchedulesFolder));
            }
        }


        private int LoadSchedules()
        {

            foreach (string file in Directory.GetFiles(this.SchedulesFolder).Where(f => Path.GetFileName(f).StartsWith(this.schedulesNameStartWith, StringComparison.InvariantCultureIgnoreCase) && Path.GetFileName(f).EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)))
            {
                foreach (Type scheduleType in Assembly.LoadFile(file).GetTypes().Where(t => typeof(ISchedule).IsAssignableFrom(t)))
                {
                    if (this.schedules == null)
                    {
                        this.schedules = new List<ScheduleUnit>();
                    }

                    ISchedule schedule = Activator.CreateInstance(scheduleType) as ISchedule;
                    if (schedule != null && schedule.Enabled)
                    {
                        this.schedules.Add(new ScheduleUnit(schedule, schedule.StartTime));
                    }


                }
            }
            return this.schedules == null ? 0 : this.schedules.Count;
        }
    }
}
