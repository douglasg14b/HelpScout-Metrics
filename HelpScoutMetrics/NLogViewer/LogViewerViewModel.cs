using HelpScoutMetrics.Model.DataTypes;
using HelpScoutMetrics.Scripts.Model;
using HelpScoutMetrics.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.NLogViewer
{
    public class LogViewerViewModel : ViewModelBase
    {
        public LogViewerViewModel()
        {
            ApplicationData.MainLogEntries.FinishedFilteringLogsEventHandler = new LogEntries.FinishedFilteringLogsEvent(FinishedFilteringLogs);
        }

        public ObservableCollection<LogEvent> LogEvents
        {
            get { return ApplicationData.MainLogEntries.FilteredLogEvents; }
        }

        public bool ViewDebugLogs
        {
            get { return ApplicationData.MainLogEntries.ViewDebugLogs; }
            set { CheckboxesEnabled = false; ApplicationData.MainLogEntries.ViewDebugLogs = value; RaisePropertyChanged("ViewDebugLogs"); }
        }

        public bool ViewErrorLogs
        {
            get { return ApplicationData.MainLogEntries.ViewErrorLogs; }
            set { CheckboxesEnabled = false; ApplicationData.MainLogEntries.ViewErrorLogs = value; RaisePropertyChanged("ViewErrorLogs"); }
        }

        public bool ViewInfoLogs
        {
            get { return ApplicationData.MainLogEntries.ViewInfoLogs; }
            set { CheckboxesEnabled = false; ApplicationData.MainLogEntries.ViewInfoLogs = value; RaisePropertyChanged("ViewInfoLogs"); }
        }

        private bool m_CheckboxesEnabled = true;
        public bool CheckboxesEnabled
        {
            get { return m_CheckboxesEnabled; }
            set { m_CheckboxesEnabled = value; RaisePropertyChanged("CheckboxesEnabled"); }
        }

        //Recieves a callback when the logs are finished filtering 
        private void FinishedFilteringLogs()
        {
            RaisePropertyChanged("LogEvents");
            CheckboxesEnabled = true;
        }

        public APICallRecords CallRecords
        {
            get { return ApplicationData.APICallHistory; }
        }
        
    }
}
