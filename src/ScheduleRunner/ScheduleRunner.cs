﻿using System;
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
        private string schedulesFolder;
        private string schedulesNameStartWith;
        private const string SCHEDULE_NAME_START = "Schedule.";
        private List<ScheduleUnit> schedules;
        private object locker = new object();

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



        public ScheduleRunner()
        {
            this.schedulesFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Schedules");
            this.schedulesNameStartWith = SCHEDULE_NAME_START;
            this.Initialize();
        }

        public ScheduleRunner(string schedulesFolder, string schedulesNameStartWith = SCHEDULE_NAME_START)
        {
            this.schedulesFolder = schedulesFolder;
            this.schedulesNameStartWith = schedulesNameStartWith;
            this.Initialize();
        }

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
                                        Console.WriteLine(String.Concat(Enumerable.Repeat("*", 14)));
                                        Console.WriteLine("Schedule run time: {0}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"));
                                        Console.WriteLine("Running schedule: {0}", schedule.GetType().Name);
                                        schedule.Schedule.GetInstance().Start();
                                    });
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
            
            foreach (string file in Directory.GetFiles(this.SchedulesFolder).Where(f => Path.GetFileName( f).StartsWith(this.schedulesNameStartWith, StringComparison.InvariantCultureIgnoreCase) && Path.GetFileName(f).EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)))
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