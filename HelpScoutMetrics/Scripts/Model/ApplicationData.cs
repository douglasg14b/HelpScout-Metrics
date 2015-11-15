using HelpScoutMetrics.Model.DataTypes;
using HelpScoutMetrics.NLogViewer;
using HelpScoutMetrics.Scripts.Model;
using HelpScoutMetrics.Scripts.Model.DataTypes;
using HelpScoutMetrics.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Scripts.Model
{
    public static class ApplicationData
    {
        private static Settings m_ApplicationSettings;
        public static Settings ApplicationSettings
        {
            get { return m_ApplicationSettings; }
            set { m_ApplicationSettings = value; }
        }

        private static UserList m_Users = new UserList();
        public static UserList Users
        {
            get { return m_Users; }
            set { m_Users = value; }
        }

        private static LogEntries m_MainLogEntries = new LogEntries();
        public static LogEntries MainLogEntries
        {
            get { return m_MainLogEntries; }
            set { m_MainLogEntries = value; }
        }

        private static QuickStats m_QuickStatistics = new QuickStats();
        public static QuickStats QuickStatistics
        {
            get { return m_QuickStatistics; }
            set { m_QuickStatistics = value; }
        }

        private static List<ColumnListItem> m_QuickStatsColums = new List<ColumnListItem>();
        public static List<ColumnListItem> QuickStatsColums
        {
            get { return m_QuickStatsColums; }
            set { m_QuickStatsColums = value; }
        }

        private static ColumnPreferences m_ApplicationColumns = new ColumnPreferences(true);
        public static ColumnPreferences ApplicationColumns
        {
            get { return m_ApplicationColumns; }
            set { m_ApplicationColumns = value; }
        }

        private static APICallRecords m_APICallHistory = new APICallRecords();
        public static APICallRecords APICallHistory
        {
            get { return m_APICallHistory; }
            set { m_APICallHistory = value; }
        }

        private static Dictionary<int, List<UserRating>> m_UserRatings = new Dictionary<int, List<UserRating>>();
        public static Dictionary<int, List<UserRating>> UserRatings
        {
            get { return m_UserRatings; }
            set { m_UserRatings = value; }
        }

        private static List<UserRating> m_UserRatingsList = new List<UserRating>();
        public static List<UserRating> UserRatingsList
        {
            get { return m_UserRatingsList; }
            set { m_UserRatingsList = value; }
        }

        public static string VersionNumber = "0.0.5.4";

        public static WindowAndUserControlReferances WindowReferances = new WindowAndUserControlReferances();

        public static MainScreenViewModel MainViewModel { get; set; }
    }
}
