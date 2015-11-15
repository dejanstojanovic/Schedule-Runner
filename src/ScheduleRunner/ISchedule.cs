using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public interface ISchedule
    {
        /// <summary>
        /// Time when the task will start
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// Time when task will stop to be executed by the ScheuleRunner class
        /// </summary>
        DateTime? EndTime {get;}

        /// <summary>
        /// Interval for repeating the task
        /// </summary>
        int PeriodSeconds { get; }

        /// <summary>
        /// If Enabled is true task will be executed otherwise, it will be skipped
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Creates new instance intsed of usinf Activator or Reflection
        /// </summary>
        /// <returns>New instance of the clas</returns>
        ISchedule GetInstance();

        /// <summary>
        /// Performs scheduled action
        /// </summary>
        void Start();
    }
}
