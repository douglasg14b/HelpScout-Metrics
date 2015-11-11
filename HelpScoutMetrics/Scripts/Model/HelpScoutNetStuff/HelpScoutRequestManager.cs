using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpScoutNet.Model.Report.User.UserReports;
using HelpScoutNet.Request.Report.User;
using HelpScoutNet;
using NLog;
using System.Net.Http;
using HelpScoutMetrics.Logging;
using HelpScoutNet.Model;
using HelpScoutMetrics.Scripts.Model;
using System.Timers;
using System.Collections;
using HelpScoutNet.Model.Report.User;
using HelpScoutNet.Request.Report;
using HelpScoutNet.Model.Report.Team;
using HelpScoutNet.Model.Report;
using System.Threading;
// Used to manage HelpScout API requests to throttle and retry when necessary
namespace HelpScoutMetrics.Model
{
    public static class HelpScoutRequestManager
    {
        static HelpScoutRequestManager()
        {
            //new System.Threading.Thread(SetupTimerLoop) { IsBackground = true }.Start();
            SetupTimerLoop();
        }

        private static HelpScoutClient client = new HelpScoutClient(ApplicationData.ApplicationSettings.APIKey);
        private static Logger logger = LogManager.GetLogger("HelpScoutRequestManager");
        static System.Timers.Timer queueLoopTimer = new System.Timers.Timer();

        private static ConcurrentQueue<BaseAPIRequest> requestQueue = new ConcurrentQueue<BaseAPIRequest>();
        private static ConcurrentQueue<object> circularBufferQueue = new ConcurrentQueue<object>();

        /*============================================================
         *  Queue Methods....etc
         * ==========================================================*/

        //Accepts a new API Request
        public static void NewQueueItem(BaseAPIRequest request)
        {
            if(ApplicationData.ApplicationSettings.ValidAPIKeyExists)
            {
                requestQueue.Enqueue(request);
                ApplicationData.APICallHistory.CurrentAPIQueueSize++;
            }
            else
            {
                logger.Log(LogLevel.Error, "An API Key Has Not Yet Been Validated For Use. Please Check Your Settings.");
                string requestType = request.RequestType.ToString();
                request.PerformResultFailed(requestType + " Request Failed, No Valid API Key Exists");
            }
        }

        private static void QueueItemUsed()
        {
            System.Timers.Timer timer = new System.Timers.Timer(60000);
            timer.Elapsed += new ElapsedEventHandler(RemoveItemFromCircularBuffer);
            timer.Start();
        }

        private static void RemoveItemFromCircularBuffer(object source, ElapsedEventArgs e)
        {
            if(circularBufferQueue.Count > 0)
            {
                object disposableResult;
                circularBufferQueue.TryDequeue(out disposableResult);
            }
            ApplicationData.APICallHistory.Last60SecondsAPICalls--;
            System.Timers.Timer timer = source as System.Timers.Timer;
            timer.Stop();
            timer.Dispose();
        }

        public static void UpdatedAPIKey(string key)
        {
            client = new HelpScoutClient(key);
        }

        //Removes all items from the queue
        public static void CancelAllRequests()
        {
            //Necessary to facuilitate clearing a concurrentqueue which doe snot support an atomic way of clearing like .clear()
            Interlocked.Exchange<ConcurrentQueue<BaseAPIRequest>>(ref requestQueue, new ConcurrentQueue<BaseAPIRequest>());
        }

        /*============================================================
        *  Queue Loop Methods
        * ==========================================================*/

        private static void SetupTimerLoop()
        {
            queueLoopTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            queueLoopTimer.Interval = 100;
            queueLoopTimer.Start();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            OnLoop();
        }

        //The start of the loop
        private static void OnLoop()
        {
            if(ApplicationData.ApplicationSettings.ValidAPIKeyExists)
            {
                if (requestQueue.Count > 0 && circularBufferQueue.Count < ApplicationData.ApplicationSettings.APIThrottleCount)
                {
                    BaseAPIRequest dequeuedRequest;
                    if (requestQueue.TryDequeue(out dequeuedRequest))
                    {
                        Task.Run(() => DetermineAPICall(dequeuedRequest));
                        circularBufferQueue.Enqueue(1);

                        ApplicationData.APICallHistory.Last60SecondsAPICalls++;
                        ApplicationData.APICallHistory.TotalAPICalls++;
                        ApplicationData.APICallHistory.CurrentAPIQueueSize--;

                        QueueItemUsed();
                    }
                }
            }
            ApplicationData.APICallHistory.TotalQueueIterations++;
        }

        /*============================================================
         *  API Call Methods
         * ==========================================================*/

        private static void DetermineAPICall(object request)
        {
            dynamic requestCast = (dynamic) request;
            APICallType requestType = requestCast.RequestType;

            try
            {
                switch (requestType)
                {
                    case APICallType.ListUsers:
                        ListUsers((BaseApiRequest<Paged<HelpScoutNet.Model.User>>)request); 
                        break;
                    case APICallType.UserReport:
                        GetUserReport((ParameterAPIRequest<UserReport, UserRequest>)request);
                        break;
                    case APICallType.UserHappiness:
                        GetUserHappiness((ParameterAPIRequest<UserHappiness, UserRequest>)request);
                        break;
                    case APICallType.VerifyAPI:
                        VeryAPI((ParameterAPIRequest<Paged<Mailbox>, string>)request);
                        break;
                    case APICallType.TeamOverall:
                        GetTeamOverallReport((ParameterAPIRequest<TeamReport, CompareRequest>)request);
                        break;
                    case APICallType.UserRatings:
                        GetUserRatings((ParameterAPIRequest<PagedReport<HelpScoutNet.Model.Report.Common.Rating>,UserRatingsRequest>)request);
                        break;
                    case APICallType.GetConversation:
                        GetConversation((ParameterAPIRequest<Conversation, int>)request);
                        break;
                    default:
                        logger.Log(LogLevel.Error, "No matches found for api request"); 
                        break;
                }
            }
            catch (AggregateException exception)
            {
                if (exception.InnerException.GetType() == typeof(HelpScoutApiException))
                {
                    HelpScoutLogHelpers.LogHelpScoutException((HelpScoutApiException)exception.InnerException, "HelpScoutRequestManager");
                }
                return;
            }
            catch (HelpScoutApiException exception)
            {
                HelpScoutLogHelpers.LogHelpScoutException(exception, "HelpScoutRequestManager");
                return;
            }
            catch (HttpRequestException exception)
            {
                logger.Log(LogLevel.Error, exception, "Could not reach the HelpScout API Service. Please Check your internet connection.");
                return;
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, exception, "Unexpected Exception Was Thrown:");
            }
        }

        public static void GetUserReport(ParameterAPIRequest<UserReport, UserRequest> request)
        {
            Task<UserReport> task = new Task<UserReport>(() => client.GetUserOverallReport(request.RequestArgs));
            task.Start();
            request.SetResult(task.Result);
        }

        public static void ListUsers(BaseApiRequest<Paged<HelpScoutNet.Model.User>> request)
        {
            logger.Log(LogLevel.Debug, "Attempting To Retrieve HelpScout Users List");
            request.SetResult(client.ListUsers());
            logger.Log(LogLevel.Debug, "Sucessfully Retrieved HelpScout Users List");
        }

        public static void GetUserHappiness(ParameterAPIRequest<UserHappiness, UserRequest> request)
        {
            logger.Log(LogLevel.Debug, "Attempting To Retrieve User Happiness");
            Task<UserHappiness> task = new Task<UserHappiness>(() => client.GetUserHappiness(request.RequestArgs));
            task.Start();
            request.SetResult(task.Result);
            logger.Log(LogLevel.Debug, "Sucessfully Retrieved User Happiness");
        }

        public static void GetUserRatings(ParameterAPIRequest<PagedReport<HelpScoutNet.Model.Report.Common.Rating>, UserRatingsRequest> request)
        {
            logger.Log(LogLevel.Debug, "Attempting To Retrieve User Ratings");
            Task<PagedReport<HelpScoutNet.Model.Report.Common.Rating>> task = new Task<PagedReport<HelpScoutNet.Model.Report.Common.Rating>>(() => client.GetUserRatings(request.RequestArgs));
            task.Start();
            request.SetResult(task.Result);
            logger.Log(LogLevel.Debug, "Sucessfully Retrieved User Ratings");
        }

        private static void GetTeamOverallReport(ParameterAPIRequest<TeamReport, CompareRequest> request)
        {
            logger.Log(LogLevel.Debug, "Attempting To Retrieve Team Overall Report");
            Task<TeamReport> task = new Task<TeamReport>(() => client.GetTeamOverall(request.RequestArgs));
            task.Start();
            request.SetResult(task.Result);
            logger.Log(LogLevel.Debug, "Sucessfully Retrieved Team Overall");
        }

        private static void GetConversation(ParameterAPIRequest<Conversation, int> request)
        {
            logger.Log(LogLevel.Debug, "Attempting To Retrieve Conversation: " + request.RequestArgs);
            Task<Conversation> task = new Task<Conversation>(() => client.GetConversation(request.RequestArgs));
            task.Start();
            request.SetResult(task.Result);
            logger.Log(LogLevel.Debug, "Sucessfully Retrieved Conversation: " + request.RequestArgs);

        }

        public static void VeryAPI(ParameterAPIRequest<Paged<Mailbox>,string> request)
        {
            logger.Log(LogLevel.Debug, "Attempting To Verify API Key");

            HelpScoutClient testClient = new HelpScoutClient(request.RequestArgs);
            Task<Paged<Mailbox>> task = new Task<Paged<Mailbox>>(() => testClient.ListMailboxes());
            request.SetResult(task.Result);

            logger.Log(LogLevel.Debug, "Sucessfully Verified API Key");
        }

    }

    public enum APICallType
    {
        UserReport,
        ListUsers,
        UserHappiness,
        UserRatings,
        VerifyAPI,
        GetConversation,
        TeamOverall
    }
}
