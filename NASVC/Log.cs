using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NASVC
{
    public static class Log
    {
        public static EventLog NAEventLog;
        
        static Log()
        {
            NAEventLog = new EventLog();
            if (!EventLog.SourceExists("NodeAliveSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "NodeAliveSource", "NodeAlive");
            }
            NAEventLog.Source = "NodeAliveSource";
            NAEventLog.Log = "NodeAlive";
            Write("Logging initialized successfully.");
        }

        public static void Write(string message)
        {
            NAEventLog.WriteEntry(message);
        }
    }
}
