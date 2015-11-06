using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

//Inspiration from NlogViewer by erizet (https://github.com/erizet/NlogViewer)

namespace HelpScoutMetrics.NLogViewer
{
    public class LogEvent : INotifyPropertyChanged
    {
        public LogEvent(NLog.LogEventInfo logEventInfo)
        {
            LoggerName = logEventInfo.LoggerName;
            TimeStamp = logEventInfo.TimeStamp;
            Level = logEventInfo.Level.ToString();
            LogMessage = logEventInfo.FormattedMessage;
            ToolTip = logEventInfo.FormattedMessage;
            Exception = logEventInfo.Exception;
            SetColors(logEventInfo);
        }

        public NLog.LogEventInfo LogEventInfo {get; private set;}

        private string m_LoggerName;
        public string LoggerName
        {
            get { return m_LoggerName; }
            set { m_LoggerName = value; RaisePropertyChanged("LoggerName"); }
        }

        private DateTime m_TimeStamp;
        public DateTime TimeStamp
        {
            get { return m_TimeStamp; }
            set { m_TimeStamp = value; RaisePropertyChanged("TimeStamp"); }
        }

        private string m_Level;
        public string Level
        {
            get { return m_Level; }
            set { m_Level = value; RaisePropertyChanged("Level"); }
        }

        private string m_LogMessage;
        public string LogMessage
        {
            get { return m_LogMessage; }
            set { m_LogMessage = value; RaisePropertyChanged("LogMessage"); }
        }

        private string m_ToolTip;
        public string ToolTip
        {
            get { return m_ToolTip; }
            set { m_ToolTip = value; RaisePropertyChanged("ToolTip"); }
        }

        private Exception m_Exception;
        public Exception Exception
        {
            get { return m_Exception; }
            set { m_Exception = value; RaisePropertyChanged("Exception"); }
        }

        private SolidColorBrush m_Background;
        public SolidColorBrush Background
        {
            get { return m_Background; }
            set { m_Background = value; RaisePropertyChanged("Background"); }
        }

        private SolidColorBrush m_Foreground;
        public SolidColorBrush Foreground
        {
            get { return m_Foreground; }
            set { m_Foreground = value; RaisePropertyChanged("Foreground"); }
        }

        private SolidColorBrush m_BackgroundMouseOver;
        public SolidColorBrush BackgroundMouseOver
        {
            get { return m_BackgroundMouseOver; }
            set { m_BackgroundMouseOver = value; RaisePropertyChanged("BackgroundMouseOver"); }
        }

        private SolidColorBrush m_ForegroundMouseOverd;
        public SolidColorBrush ForegroundMouseOver
        {
            get { return m_ForegroundMouseOverd; }
            set { m_ForegroundMouseOverd = value; RaisePropertyChanged("ForegroundMouseOver"); }
        }

        private void SetColors(NLog.LogEventInfo logEventInfo)
        {
            if (logEventInfo.Level == LogLevel.Warn)
            {
                Background = Brushes.Yellow;
                BackgroundMouseOver = Brushes.GreenYellow;
            }
            else if (logEventInfo.Level == LogLevel.Error)
            {
                Background = Brushes.Tomato;
                BackgroundMouseOver = Brushes.IndianRed;
            }
            else
            {
                Background = Brushes.White;
                BackgroundMouseOver = Brushes.LightGray;
            }
            Foreground = Brushes.Black;
            ForegroundMouseOver = Brushes.Black;
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
