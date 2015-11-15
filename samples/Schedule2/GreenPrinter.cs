using System;
using Scheduler;

namespace Schedule.Schedule2
{
    class GreenPrinter : Scheduler.ISchedule
    {
        public bool Enabled
        {
            get
            {
                return true;
            }
        }

        public DateTime? EndTime
        {
            get
            {
                return null;
            }
        }

        public int PeriodSeconds
        {
            get
            {
                return 3;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        public ISchedule GetInstance()
        {
            return new GreenPrinter();
        }

        public void Start()
        {
            Console.WriteLine(string.Format("{0} {1}",DateTime.Now, this.GetType().Name.ToUpper()));
        }
    }
}
