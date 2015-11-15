using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler;

namespace Schedule.Schedule1
{
    class BluePrinter : Scheduler.ISchedule
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
            return new BluePrinter();
        }

        public void Start()
        {
            Console.WriteLine(string.Format("{0} {1}", DateTime.Now, this.GetType().Name.ToUpper()));
        }
    }
}
