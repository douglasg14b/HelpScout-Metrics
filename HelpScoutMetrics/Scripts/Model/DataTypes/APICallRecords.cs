using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Model.DataTypes
{
    public class APICallRecords : INotifyPropertyChanged
    {

        private int m_Last60SecondsAPICalls = 0;
        public int Last60SecondsAPICalls
        {
            get { return m_Last60SecondsAPICalls; }
            set { m_Last60SecondsAPICalls = value; RaisePropertyChanged("Last60SecondsAPICalls"); }
        }

        private int m_TotalAPICalls = 0;
        public int TotalAPICalls
        {
            get { return m_TotalAPICalls; }
            set { m_TotalAPICalls = value; RaisePropertyChanged("TotalAPICalls"); }
        }

        private int m_CurrentAPIQueueSize = 0;
        public int CurrentAPIQueueSize
        {
            get { return m_CurrentAPIQueueSize; }
            set { m_CurrentAPIQueueSize = value; RaisePropertyChanged("CurrentAPIQueueSize"); }
        }

        private int m_TotalQueueIterations = 0;
        public int TotalQueueIterations
        {
            get { return m_TotalQueueIterations; }
            set { m_TotalQueueIterations = value; RaisePropertyChanged("TotalQueueIterations"); }
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
