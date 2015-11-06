using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

//Inspiration from NlogViewer by erizet (https://github.com/erizet/NlogViewer)

namespace HelpScoutMetrics.NLogViewer
{
    public class LogEntries : INotifyPropertyChanged
    {
        public LogEntries()
        {
            foreach(NlogViewerTarget target in NLog.LogManager.Configuration.AllTargets.Where(t=>t is NlogViewerTarget).Cast<NlogViewerTarget>())
            {
                target.RecieveLog += RecieveLog;
            }

            FilteredLogEvents = new ObservableCollection<LogEvent>();
            BindingOperations.EnableCollectionSynchronization(FilteredLogEvents, filteredLogEventsLock);
        }

        Logger logger = LogManager.GetLogger("LogEntries Log");

        //Callback for the viewmodel to notify when filtering is done
        public delegate void FinishedFilteringLogsEvent();
        public FinishedFilteringLogsEvent FinishedFilteringLogsEventHandler;

        //Lock object for the observable collection
        private object filteredLogEventsLock = new object();

        public ObservableCollection<LogEvent> FilteredLogEvents { get; set; }

        //Holds temp logs when filtering is active and new logs get added
        private ConcurrentQueue<LogEvent> tempLogEvents = new ConcurrentQueue<LogEvent>();

        private ConcurrentQueue<LogEvent> m_LogEvents = new ConcurrentQueue<LogEvent>();
        public ConcurrentQueue<LogEvent> LogEvents
        {
            get { return m_LogEvents; }
            set { m_LogEvents = value; RaisePropertyChanged("LogEvents"); }
        }

        private bool m_ViewDebugLogs = false;
        public bool ViewDebugLogs
        {
            get { return m_ViewDebugLogs; }
            set { m_ViewDebugLogs = value; RaisePropertyChanged("ViewDebugLogs"); Task.Run(new Action(() => FilterList())); }
        }

        private bool m_ViewErrorLogs = true;
        public bool ViewErrorLogs
        {
            get { return m_ViewErrorLogs; }
            set { m_ViewErrorLogs = value; RaisePropertyChanged("ViewErrorLogs"); Task.Run(new Action(() => FilterList())); }
        }

        private bool m_ViewInfoLogs = true;
        public bool ViewInfoLogs
        {
            get { return m_ViewInfoLogs; }
            set { m_ViewInfoLogs = value; RaisePropertyChanged("ViewInfoLogs"); Task.Run(new Action(() => FilterList())); }
        }

        private bool currentlyFiltering = false;
        //Refilters the current log events list to the filtered list when one of the options change
        private void FilterList()
        {
            currentlyFiltering = true;

            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                FilteredLogEvents.Clear();
            }));
            SpinWait sw = new SpinWait();
            foreach (LogEvent log in FilterLogEvents(LogEvents.ToArray()))
            {
                FilteredLogEvents.Add(log);
                Thread.Sleep(1);
            }
            AddQueuedLogsToFilteredList();

            RaisePropertyChanged("FilteredLogEvents");
            FinishedFilteringLogs();

            currentlyFiltering = false;
        }

        //Filters and returns a list of filtered log events
        private IEnumerable<LogEvent> FilterLogEvents(LogEvent[] logs)
        {
            return (from x in logs
                    where ((ViewDebugLogs == true) ? x.Level == "Debug" : false)
                    || ((ViewErrorLogs == true) ? x.Level == "Error" : false)
                    || ((ViewInfoLogs == true) ? x.Level == "Info" : false)
                    select x);
        }

        private void FinishedFilteringLogs()
        {
            if(FinishedFilteringLogsEventHandler != null)
            {
                FinishedFilteringLogsEventHandler();
            }
        }

        private void AddQueuedLogsToFilteredList()
        {
            int preInsertionCount = tempLogEvents.Count;
            int insertedLogsCount = 0;

            if (!tempLogEvents.IsEmpty)
                logger.Log(LogLevel.Debug, preInsertionCount + " Log Events Occured During Filtering, Attempting To Insert.");

            while (!tempLogEvents.IsEmpty)
            {
                LogEvent log;
                if (tempLogEvents.TryDequeue(out log))
                {
                    if (ViewDebugLogs && log.Level == "Debug")
                    {
                        InsertIntoFilteredList(log);
                    }
                    else if (ViewErrorLogs && log.Level == "Error")
                    {
                        InsertIntoFilteredList(log);
                    }
                    else if (ViewInfoLogs && log.Level == "Info")
                    {
                        InsertIntoFilteredList(log);
                    }
                    insertedLogsCount++;
                }
            }

            if(insertedLogsCount - preInsertionCount != 0)
                logger.Log(LogLevel.Debug, (insertedLogsCount - preInsertionCount) + " Matching Log Events Occured While Inserting");
            if(insertedLogsCount != 0)
                logger.Log(LogLevel.Debug, "Sucessfully Inserted " + insertedLogsCount + " Logs Into Collection");
        }

        private void InsertIntoFilteredList(LogEvent log)
        {
            for(int i = FilteredLogEvents.Count -1; i >= 0; i--)
            {
                if(FilteredLogEvents[i].TimeStamp.Ticks <= log.TimeStamp.Ticks)
                {
                    if(i == FilteredLogEvents.Count - 1) //If the log is added to the top of the array
                    {
                        FilteredLogEvents.Add(log);
                        break;
                    }
                    else if(FilteredLogEvents[i].TimeStamp.Ticks == log.TimeStamp.Ticks) //If the timestamp is the same, insert at i
                    {
                        FilteredLogEvents.Insert(i, log);
                        break;
                    }
                    else if(FilteredLogEvents[i + 1].TimeStamp.Ticks >= log.TimeStamp.Ticks) //If the index above is a later than or the same time as timestamp, insert there
                    {
                        FilteredLogEvents.Insert(i + 1, log);
                        break;
                    }
                    else //If somehow it's all fucked up, break?
                    {
                        int z = 7;
                    }
                }
            }
        }

        //Adds the log event to the filtered events list if it matches the criteria
        private void AddToFilteredList(LogEvent logEvent)
        {
            if (ViewDebugLogs && logEvent.Level == "Debug")
            {
                FilteredLogEvents.Add(logEvent);
            }
            else if (ViewErrorLogs && logEvent.Level == "Error")
            {
                FilteredLogEvents.Add(logEvent);
            }
            else if (ViewInfoLogs && logEvent.Level == "Info")
            {
                FilteredLogEvents.Add(logEvent);
            }
        }

        public void RecieveLog(NLog.Common.AsyncLogEventInfo log)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                LogEvents.Enqueue(new LogEvent(log.LogEvent));
                if (!currentlyFiltering)
                {
                    AddToFilteredList(new LogEvent(log.LogEvent));
                }
                else
                {
                    tempLogEvents.Enqueue(new LogEvent(log.LogEvent));
                }
            }));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler temp = PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
