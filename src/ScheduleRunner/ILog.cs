using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public interface ILog
    {
        void Log(Exception ex);
    }
}
