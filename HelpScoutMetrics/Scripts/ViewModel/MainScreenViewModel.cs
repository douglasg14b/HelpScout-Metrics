using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpScoutNet.Model.Report.User.UserReports;
using HelpScoutMetrics.Model.DataTypes;
using System.Collections.ObjectModel;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using HelpScoutMetrics.Views;
using MahApps.Metro.Controls.Dialogs;
using HelpScoutMetrics.Model.WindowLogic;
using HelpScoutMetrics.NLogViewer;
using NLog;
using HelpScoutMetrics.Scripts.Model;
using HelpScoutMetrics.Scripts.Model.WindowLogic;
using System.Windows.Data;

namespace HelpScoutMetrics.ViewModel
{
    public class MainScreenViewModel : ViewModelBase
    {
        public MainScreenViewModel()
        {
            QuickStatistics = new QuickStats();
            QuickStatistics.UserQuickStats = new ObservableCollection<UserQuickStat>();
            ApplicationData.MainViewModel = this; // Temp for debug of issues
            logger.Log(LogLevel.Info, "Instantiated MainScreenViewModel");
        }

        public static Logger logger = LogManager.GetLogger("MainScreenViewModel");

        public MainWindow Window { get; set; }

        public string ProgramTitle
        {
            get { return "HelpScout Metrics v" + ApplicationData.VersionNumber ; }
        }

        private UserReport m_UserOverall;
        public UserReport UserOverall
        {
            get { return m_UserOverall; }
            set { m_UserOverall = value; RaisePropertyChanged("UserOverall"); }
        }

        private QuickStats m_QuickStatistics = ApplicationData.QuickStatistics;
        public QuickStats QuickStatistics
        {
            get { return ApplicationData.QuickStatistics; }
            set { ApplicationData.QuickStatistics = value; RaisePropertyChanged("QuickStatistics"); }
        }

        private DateTime m_SelectedStartDate = DateTime.Now;
        public DateTime SelectedStartDate
        {
            get { return m_SelectedStartDate; }
            set 
            { 
                m_SelectedStartDate = value;
                QuickStatistics.SetStartDate(value);
                RaisePropertyChanged("SelectedStartDate");
            }
        }

        private DateTime m_SelectedEndDate = DateTime.Now;
        public DateTime SelectedEndDate
        {
            get { return m_SelectedEndDate; }
            set
            {
                m_SelectedEndDate = value;
                QuickStatistics.SetEndDate(value);
                RaisePropertyChanged("SelectedEndDate");
            }
        }

        private ObservableCollection<DateTime> m_SelectedDates;
        public ObservableCollection<DateTime> SelectedDates
        {
            get { return m_SelectedDates; }
            set { m_SelectedDates = value; RaisePropertyChanged("SelectedDates"); }
        }

        #region LoadQuickStats Button

        private bool m_EnableLoadQuickStatsButton;
        public bool EnableLoadQuickStatsButton
        {
            get { return m_EnableLoadQuickStatsButton; }
            set { m_EnableLoadQuickStatsButton = value; RaisePropertyChanged("EnableLoadQuickStatsButton"); }
        }

        private bool m_CurrentlyLoadingQuickStats = false;
        public bool CurrentlyLoadingQuickStats
        {
            get { return m_CurrentlyLoadingQuickStats; }
            set 
            { 
                m_CurrentlyLoadingQuickStats = value; 
                if(value)
                {
                    LoadQuickStatsButtonText = "Loading Quickstats...";
                    EnableLoadQuickStatsButton = false;
                }
                else
                {
                    LoadQuickStatsButtonText = "Load Quickstats";
                    EnableLoadQuickStatsButton = true;
                    CalculateSelectedUsersTotals();
                }
                RaisePropertyChanged("CurrentlyLoadingQuickStats"); 
            }
        }

        private string m_LoadQuickStatsButtonText = "Load Quickstats";
        public string LoadQuickStatsButtonText
        {
            get { return m_LoadQuickStatsButtonText; }
            set { m_LoadQuickStatsButtonText = value; RaisePropertyChanged("LoadQuickStatsButtonText"); }
        }

        #endregion

        #region Columns

        /*===================================
         *        Column CheckBoxes
         * ================================*/

        private Dictionary<string, ColumnObject> m_EnabledColumns = new Dictionary<string, ColumnObject>();
        public Dictionary<string, ColumnObject> EnabledColumns
        {
            get { return m_EnabledColumns; }
            set
            {
                m_EnabledColumns = value;
                RaisePropertyChanged("EnabledColums");
            }
        }



        public bool NameColumnEnabled
        {
            get { return GetColumnEnabledStatus("Name"); }
            set { SetColumnEnabledStatus("Name", value); RaisePropertyChanged("NameColumnEnabled"); }
        }

        private int m_NameColumnIndex;
        public int NameColumnIndex
        {
            get { return m_NameColumnIndex; }
            set { m_NameColumnIndex = value; ChangeColumnIndex("Name", value); RaisePropertyChanged("NameColumnIndex"); }
        }



        public bool TotalConversationsColumnEnabled
        {
            get { return GetColumnEnabledStatus("Total Conversations"); }
            set { SetColumnEnabledStatus("Total Conversations", value); RaisePropertyChanged("TotalConversationsColumnEnabled"); }
        }

        private int m_TotalConversationsColumnIndex;
        public int TotalConversationsColumnIndex
        {
            get { return m_TotalConversationsColumnIndex; }
            set { m_TotalConversationsColumnIndex = value; ChangeColumnIndex("Total Conversations", value); RaisePropertyChanged("TotalConversationsColumnIndex"); }
        }



        public bool TotalRepliesColumnEnabled
        {
            get { return GetColumnEnabledStatus("Total Replies"); }
            set { SetColumnEnabledStatus("Total Replies", value); RaisePropertyChanged("TotalRepliesColumnEnabled"); }
        }

        private int m_TotalRepliesColumnIndex;
        public int TotalRepliesColumnIndex
        {
            get { return m_TotalRepliesColumnIndex; }
            set { m_TotalRepliesColumnIndex = value; ChangeColumnIndex("Total Replies", value); RaisePropertyChanged("TotalRepliesColumnIndex"); }
        }



        public bool GreatRatingsColumnEnabled
        {
            get { return GetColumnEnabledStatus("Great Ratings"); }
            set { SetColumnEnabledStatus("Great Ratings", value); RaisePropertyChanged("GreatRatingsColumnEnabled"); }
        }

        private int m_GreatRatingsColumnIndex;
        public int GreatRatingsColumnIndex
        {
            get { return m_GreatRatingsColumnIndex; }
            set { m_GreatRatingsColumnIndex = value; ChangeColumnIndex("Great Ratings", value); RaisePropertyChanged("GreatRatingsColumnIndex"); }
        }



        public bool TotalRatingsColumnEnabled
        {
            get { return GetColumnEnabledStatus("Total Ratings"); }
            set { SetColumnEnabledStatus("Total Ratings", value); RaisePropertyChanged("TotalRatingsColumnEnabled"); }
        }

        private int m_TotalRatingsColumnIndex;
        public int TotalRatingsColumnIndex
        {
            get { return m_TotalRatingsColumnIndex; }
            set { m_TotalRatingsColumnIndex = value; ChangeColumnIndex("Total Ratings", value); RaisePropertyChanged("TotalRatingsColumnIndex"); }
        }



        public bool HappinessScoreColumnEnabled
        {
            get { return GetColumnEnabledStatus("Happiness Score"); }
            set { SetColumnEnabledStatus("Happiness Score", value); RaisePropertyChanged("HappinessScoreColumnEnabled"); }
        }

        private int m_HappinessScoreColumnIndex;
        public int HappinessScoreColumnIndex
        {
            get { return m_HappinessScoreColumnIndex; }
            set { m_HappinessScoreColumnIndex = value; ChangeColumnIndex("Happiness Score", value); RaisePropertyChanged("HappinessScoreColumnIndex"); }
        }



        public bool HandleTimeColumnEnabled
        {
            get { return GetColumnEnabledStatus("Handle Time"); ; }
            set { SetColumnEnabledStatus("Handle Time", value); RaisePropertyChanged("HandleTimeColumnEnabled"); }
        }

        private int m_HandleTimeColumnIndex;
        public int HandleTimeColumnIndex
        {
            get { return m_HandleTimeColumnIndex; }
            set { m_HandleTimeColumnIndex = value; ChangeColumnIndex("Handle Time", value); RaisePropertyChanged("HandleTimeColumnIndex"); }
        }



        public bool CustomersHelpedColumnEnabled
        {
            get { return GetColumnEnabledStatus("Customers Helped"); }
            set { SetColumnEnabledStatus("Customers Helped", value); RaisePropertyChanged("CustomersHelpedColumnEnabled"); }
        }

        private int m_CustomersHelpedColumnIndex;
        public int CustomersHelpedColumnIndex
        {
            get { return m_CustomersHelpedColumnIndex; }
            set { m_CustomersHelpedColumnIndex = value; ChangeColumnIndex("Customers Helped", value); RaisePropertyChanged("CustomersHelpedColumnIndex"); }
        }



        public bool ConversationsCreatedColumnEnabled
        {
            get { return GetColumnEnabledStatus("Conversations Created"); }
            set { SetColumnEnabledStatus("Conversations Created", value); RaisePropertyChanged("ConversationsCreatedColumnEnabled"); }
        }

        private int m_ConversationsCreatedColumnIndex;
        public int ConversationsCreatedColumnIndex
        {
            get { return m_ConversationsCreatedColumnIndex; }
            set { m_ConversationsCreatedColumnIndex = value; ChangeColumnIndex("Conversations Created", value); RaisePropertyChanged("ConversationsCreatedColumnIndex"); }
        }

        //Return the column enabled status for the UI
        private bool GetColumnEnabledStatus(string name)
        {
            try
            {
                return EnabledColumns[name].Enabled;
            }
            catch(Exception exception)
            {
                logger.Fatal("Unable to find column " + name + " in HashTable", exception);
                return false;
            }
        }

        //Sets the column enabled status when the checkbox is checked
        private void SetColumnEnabledStatus(string name, bool value)
        {
            try
            {
                EnabledColumns[name].Enabled = value;
            }
            catch (Exception exception)
            {
                logger.Fatal("Unable to find column " + name + " in HashTable", exception);
            }
        }

        [Obsolete("Method has been replaced with SetColumnEnabledStatus()", true)]
        private void EnableDisableColumn(string name, bool value)
        {
            ColumnObject column;
            try
            {
                column = EnabledColumns[name];
            }
            catch (InvalidOperationException exception)
            {
                logger.Error("Input Sequence Contained Zero, Or More Than One Item. Unable to enable/disable Column: " + name, exception);
                return;
            }
            column.Enabled = value;
        }

        //Saves column index changes to the column objects
        private void ChangeColumnIndex(string name, int value)
        {
            ColumnObject column;
            ColumnListItem columnListItem;
            try
            {
                column = EnabledColumns[name];
                columnListItem = ApplicationData.ApplicationColumns.QuickStatColumns.Single(i => i.Name == name);
            }
            catch (InvalidOperationException exception)
            {
                logger.Error("Input Sequence Contained Zero, Or More Than One Item. Unable to save index for column: " + name, exception);
                return;
            }
            column.Index = value;
            columnListItem.Index = value;

        }

        public void SetUpColumns()
        {
            List<ColumnListItem> columnItems = ApplicationData.ApplicationColumns.QuickStatColumns.OrderBy(v => v.Index).ToList(); //Necessary to instantiate items in the correct order when indexes don't start at 0
            int index = 0;
            for (int i = 0; i < columnItems.Count; i++ )
            {
                //If it's en enabled column, then iterate the index
                if(columnItems[i].Enabled)
                {
                    columnItems[i].Index = index;
                    index++;
                }
                EnabledColumns.Add(columnItems[i].Name, new ColumnObject(columnItems[i], Window.MainScreenView.QuickStatsDataGrid, this));
            }
        }

        #endregion

        #region Selected Users Totals

        private int m_SelectedUsersTotalConversations = 0;
        public int SelectedUsersTotalConversations
        {
            get { return m_SelectedUsersTotalConversations; }
            set { m_SelectedUsersTotalConversations = value; RaisePropertyChanged("SelectedUsersTotalConversations"); }
        }

        private int m_SelectedUsersTotalReplies = 0;
        public int SelectedUsersTotalReplies
        {
            get { return m_SelectedUsersTotalReplies; }
            set { m_SelectedUsersTotalReplies = value; RaisePropertyChanged("SelectedUsersTotalReplies"); }
        }

        private double m_SelectedUsersAverageHappiness = 0;
        public double SelectedUsersAverageHappiness
        {
            get { return m_SelectedUsersAverageHappiness; }
            set { m_SelectedUsersAverageHappiness = (Math.Truncate(value * 100) / 100); RaisePropertyChanged("SelectedUsersAverageHappiness"); }
        }

        private double selectedUsersHappinessTotal;
        private int selectedUsersHappinessUserCount;

        //Calculate the total stats for the quickstat users
        private void CalculateSelectedUsersTotals()
        {
            if (QuickStatistics.UserQuickStats.Count > 0)
            {
                ClearSelectedUsersTotals();
                int totalRatings = 0;
                int greatRatings = 0;
                foreach (UserQuickStat quickStat in QuickStatistics.UserQuickStats)
                {
                    SelectedUsersTotalConversations += quickStat.TotalConversations;
                    SelectedUsersTotalReplies += quickStat.TotalReplies;
                    selectedUsersHappinessTotal += quickStat.HappinessScore;

                    if (quickStat.TotalRatingsCount != 0)
                        selectedUsersHappinessUserCount++;
                }

                if (selectedUsersHappinessUserCount > 0)
                {
                    SelectedUsersAverageHappiness = selectedUsersHappinessTotal / selectedUsersHappinessUserCount;
                }
            }
        }

        private void ClearSelectedUsersTotals()
        {
            SelectedUsersTotalConversations = 0;
            SelectedUsersTotalReplies = 0;
            selectedUsersHappinessTotal = 0;
            selectedUsersHappinessUserCount = 0;
            SelectedUsersAverageHappiness = 0;
        }

        #endregion

        #region Team Totals

        private bool m_PullTeamTotalsBool = false;
        public bool  PullTeamTotalsBool
        {
            get { return m_PullTeamTotalsBool; }
            set { m_PullTeamTotalsBool = value; RaisePropertyChanged("PullTeamTotalsBool"); }
        }

        private string m_TeamTotalConversations = "0";
        public string TeamTotalConversations
        {
            get { return m_TeamTotalConversations; }
            set { m_TeamTotalConversations = value; RaisePropertyChanged("TeamTotalConversations"); }
        }

        private string m_TeamTotalReplies = "0";
        public string TeamTotalReplies
        {
            get { return m_TeamTotalReplies; }
            set { m_TeamTotalReplies = value; RaisePropertyChanged("TeamTotalReplies"); }
        }

        private string m_TeamCustomersHelped = "0";
        public string TeamCustomersHelped
        {
            get { return m_TeamCustomersHelped; }
            set { m_TeamCustomersHelped = value; RaisePropertyChanged("TeamCustomersHelped"); }
        }

        private string m_TeamTicketsClosed = "0";
        public string TeamTicketsClosed
        {
            get { return m_TeamTicketsClosed; }
            set { m_TeamTicketsClosed = value; RaisePropertyChanged("TeamTicketsClosed"); }
        }

        private string m_TeamTotalUsers = "0";
        public string TeamTotalUsers
        {
            get { return m_TeamTotalUsers; }
            set { m_TeamTotalUsers = value; RaisePropertyChanged("TeamTotalUsers"); }
        }

        #endregion

        #region Get Quick Stats

        // The number of requests contained in the current quickstats request
        private int m_QuickStatsRequestSize = 0;
        public int QuickStatsRequestSize
        {
            get { return m_QuickStatsRequestSize; }
            set { m_QuickStatsRequestSize = value; RaisePropertyChanged("QuickStatsRequestSize"); }
        }

        public async void GetQuickStats()
        {
            CurrentlyLoadingQuickStats = true;
            foreach(HelpScoutMetrics.Model.DataTypes.User userObject in ApplicationData.Users.Users)
            {
                if(userObject.Valid)
                {
                    await Task.Run(() => MainWindowLogic.RetrieveQuickStats(userObject, QuickStatistics.StartDate.Value.ToUniversalTime(), QuickStatistics.EndDate.Value.ToUniversalTime()));
                }
            }
            CurrentlyLoadingQuickStats = false;
        }

        private void SetQuickStatsRequestSize()
        {
            QuickStatsRequestSize = (ApplicationData.Users.Users.Count * 2);
        }

        public void RecieveQuickStatOverall()
        {

        }

        public void RecieveQuickStatHappiness()
        {

        }

        #endregion

        #region WindowOpeening

        public void OpenSettings()
        {
            if(Window.SettingsFlyout.IsOpen)
            {
                Window.SettingsFlyout.IsOpen = false;
                logger.Log(LogLevel.Info, "Closed Settings Flyout");
            }
            else
            {
                SettingsViewModel viewModel = Window.SettingsFlyout.DataContext as SettingsViewModel;
                viewModel.MainWindow = Window;
                viewModel.LoadSettings();
                Window.SettingsFlyout.IsOpen = true;
                logger.Log(LogLevel.Info, "Opened Settings Flyout");
            }
        }

        public void OpenUsersList()
        {
            if(Window.UserListFlyout.IsOpen)
            {
                Window.UserListFlyout.IsOpen = false;
                logger.Log(LogLevel.Info, "Closed Users List Flyout");
            }
            else
            {
                UserListViewModel viewModel = Window.UserListFlyout.DataContext as UserListViewModel;
                UserListView userListView = Window.UserListFlyoutView;
                viewModel.UserListView = userListView;
                viewModel.MainWindow = Window;
                viewModel.SetupFreshViewModel();

                Window.UserListFlyout.IsOpen = true;
                logger.Log(LogLevel.Info, "Opened Users List Flyout");

            }
        }

        public void OpenLogWindow()
        {
            int count = ApplicationData.MainLogEntries.LogEvents.Count;
            NLogViewerView window = new NLogViewerView();
            window.Show();
            logger.Log(LogLevel.Debug, "Opened Log Window");
        }

        public void RefreshView()// Temp for debug of issues
        {
            foreach (string name in MiscMethods.GetPropertyNames(this))
            {
                RaisePropertyChanged(name);
            }
        }

        #endregion


        public APICallRecords APICallCount
        {
            get { return ApplicationData.APICallHistory; }
        }
    }
}
