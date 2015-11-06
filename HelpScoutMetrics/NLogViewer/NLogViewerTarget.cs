using NLog.Common;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Inspiration from NlogViewer by erizet (https://github.com/erizet/NlogViewer)

namespace HelpScoutMetrics.NLogViewer
{
    [Target("LogEntries")]
    public class NlogViewerTarget : Target
    {
        public event Action<AsyncLogEventInfo> RecieveLog;

        protected override void Write(NLog.Common.AsyncLogEventInfo logEvent)
        {
            base.Write(logEvent);

            if (RecieveLog != null)
                RecieveLog(logEvent);
        }
    }
}
