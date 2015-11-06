using HelpScoutMetrics.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Model.DataTypes
{
    public class QuickStats : ViewModelBase
    {
        public QuickStats()
        {
            m_UserQuickStats = new ObservableCollection<UserQuickStat>();
        }

        //Sets the start date when the date on the datepicker changes
        public void SetStartDate(DateTime dateTime)
        {
            DateTime startTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 1, DateTimeKind.Local);
            StartDate = startTime;
        }

        //Sets the end date when the date on the datepicker changes
        public void SetEndDate(DateTime dateTime)
        {
            DateTime endTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, DateTimeKind.Local);
            EndDate = endTime;
        }

        // The start of the stats timerange
        private DateTime? m_StartDate;
        public DateTime? StartDate
        {
            get { return m_StartDate; }
            set { m_StartDate = value; RaisePropertyChanged("StartDate"); }
        }

        //The end of the stats timerange
        private DateTime? m_EndDate;
        public DateTime? EndDate
        {
            get { return m_EndDate; }
            set { m_EndDate = value; RaisePropertyChanged("EndDate"); }
        }

        private ObservableCollection<UserQuickStat> m_UserQuickStats;
        public ObservableCollection<UserQuickStat> UserQuickStats
        {
            get { return m_UserQuickStats; }
            set { m_UserQuickStats = value; RaisePropertyChanged("UserQuickStats"); }
        }
    }
}
