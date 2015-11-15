using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
   public class ScheduleUnit
    {
        private ISchedule schedule;

        public ISchedule Schedule
        {
            get
            {
                return this.schedule;
            }
        }

        public DateTime StartAt
        {
            get; set;
        }

        public ScheduleUnit(ISchedule schedule, DateTime startAt)
        {
            this.schedule = schedule;
            this.StartAt = startAt;
        }
    }
}
