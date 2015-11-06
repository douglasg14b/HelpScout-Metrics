using HelpScoutMetrics.Logging;
using HelpScoutMetrics.Model.DataTypes;
using HelpScoutMetrics.Scripts.Model;
using HelpScoutNet;
using HelpScoutNet.Request.Report;
using HelpScoutNet.Request.Report.User;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HelpScoutNet.Model.Report.Team;

namespace HelpScoutMetrics.Model.WindowLogic
{
    /// <summary>
    ///  Class holding the logic surrounding the Main Window
    /// </summary>
    public static class MainWindowLogic
    {
        private static Logger logger = LogManager.GetLogger("MainWindowLogic");

        #region API Calls
        //Creates new API requsts for the requests manager
        public static void RetrieveQuickStats(User user, DateTime start, DateTime end)
        {
            UserAPIRequest<HelpScoutNet.Model.Report.User.UserReports.UserReport, UserRequest> overallRequest = new UserAPIRequest<HelpScoutNet.Model.Report.User.UserReports.UserReport, UserRequest>(new UserRequest(user.ID, start, end), APICallType.UserReport, user.ID, user.Name);
            UserAPIRequest<HelpScoutNet.Model.Report.User.UserHappiness, UserRequest> happinessRequest = new UserAPIRequest<HelpScoutNet.Model.Report.User.UserHappiness, UserRequest>(new UserRequest(user.ID, start, end), APICallType.UserHappiness, user.ID, user.Name);

            overallRequest.UserResultReady += RecieveUserOverallStat;
            happinessRequest.UserResultReady += RecieveHappinessQuickStat;

            overallRequest.resultsFailedHandler = new BaseAPIRequest.ResultsFailed(ResultsFailed);
            happinessRequest.resultsFailedHandler = new BaseAPIRequest.ResultsFailed(ResultsFailed);

            HelpScoutRequestManager.NewQueueItem(overallRequest);
            HelpScoutRequestManager.NewQueueItem(happinessRequest);

        }

        public static void RetrieveTeamOverall(DateTime start, DateTime end)
        {
            ParameterAPIRequest<HelpScoutNet.Model.Report.Team.TeamReport, CompareRequest> teamOverallRequest = new ParameterAPIRequest<HelpScoutNet.Model.Report.Team.TeamReport, CompareRequest>(new CompareRequest(start, end), APICallType.TeamOverall);

            teamOverallRequest.ResultReady += RecieveTeamOverallStat;
        }

        private static void RecieveTeamOverallStat(object sender, BaseApiRequest<TeamReport>.ResultReadyEventArgs<TeamReport> e)
        {
            throw new NotImplementedException();
        }

        //Recieves callback to add data to quickstats
        private static void RecieveUserOverallStat(object sender, UserAPIRequest<HelpScoutNet.Model.Report.User.UserReports.UserReport, UserRequest>.UserResultReadyEventArgs<HelpScoutNet.Model.Report.User.UserReports.UserReport> e)
        {
            AddUserReportToQuickStats(e.Result, e.UserID, e.Name);
        }

        //Recieves callback to add data to quickstats
        private static void RecieveHappinessQuickStat(object sender, UserAPIRequest<HelpScoutNet.Model.Report.User.UserHappiness, UserRequest>.UserResultReadyEventArgs<HelpScoutNet.Model.Report.User.UserHappiness> e)
        {
            AddHappinessReportToQuickStats(e.Result, e.UserID, e.Name);
        }


        //Recieves callback when results have failed
        private static void ResultsFailed(string message)
        {
            logger.Log(LogLevel.Error, message);
        }

        #endregion


        //Adds user report data to an already existing quickstat item
        private static void AddUserReportToQuickStats(HelpScoutNet.Model.Report.User.UserReports.UserReport userReport, int ID, string name)
        {
            int index = FindQuickStatItem(ID);
            if (index == -1)
            {
                //ApplicationData.QuickStatistics.UserQuickStats.Add(quickStat);
                //This should not happen since the quickstat will not be attempted if the user never had an ID in the first place. And havign an ID means a quickstat item was created.
            }
            else
            {
                ApplicationData.QuickStatistics.UserQuickStats[index].UpdateQuickStat(userReport.Current.TotalReplies, userReport.Current.TotalConversations, userReport.Current.HappinessScore, userReport.Current.HandleTime, userReport.Current.CustomersHelped, userReport.Current.ConversationsCreated);
            }
        }

        //Adds happiness data to an already existing quickstat item
        private static void AddHappinessReportToQuickStats(HelpScoutNet.Model.Report.User.UserHappiness userHappiness, int ID, string name)
        {
            int index = FindQuickStatItem(ID);
            if (index == -1)
            {
                //ApplicationData.QuickStatistics.UserQuickStats.Add(quickStat);
                //This should not happen since the quickstat will not be attempted if the user never had an ID in the first place. And havign an ID means a quickstat item was created.
            }
            else
            {
                ApplicationData.QuickStatistics.UserQuickStats[index].UpdateQuickStat(userHappiness.Current.RatingsCount, userHappiness.Current.GreatCount);
            }
        }

        //Writes quickstat items to the master list if it does not already exist
        private static void AddToQuickStats(UserQuickStat quickStat)
        {
            int index = FindQuickStatItem(quickStat);
            if (index == -1)
            {
                ApplicationData.QuickStatistics.UserQuickStats.Add(quickStat);
            }
            else
            {
                ApplicationData.QuickStatistics.UserQuickStats[index].UpdateQuickStat(quickStat);
            }
        }

        //Finds and returns the idex of a quickstat item in the master list
        private static int FindQuickStatItem(int ID)
        {
            for (int i = 0; i <= ApplicationData.QuickStatistics.UserQuickStats.Count; i++)
            {
                if (ApplicationData.QuickStatistics.UserQuickStats[i].UserID == ID)
                {
                    return i;
                }
            }
            return -1;
        }

        //Finds and returns the idex of a quickstat item in teh master list
        private static int FindQuickStatItem(UserQuickStat quickStat)
        {
            for(int i = 0; i <= ApplicationData.QuickStatistics.UserQuickStats.Count; i++)
            {
                if(ApplicationData.QuickStatistics.UserQuickStats[i].Name == quickStat.Name)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
