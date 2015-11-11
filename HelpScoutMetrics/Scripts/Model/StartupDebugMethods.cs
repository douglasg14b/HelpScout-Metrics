﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpScoutMetrics.Model;
using HelpScoutNet.Request.Report.User;
using HelpScoutNet.Model.Report;
using HelpScoutNet.Model.Report.Common;
using HelpScoutMetrics.Scripts.Model.DataTypes;
using CsvHelper;
using System.IO;
using HelpScoutMetrics.Scripts.Model;
using HelpScoutNet.Model;
using NLog;
using HelpScoutMetrics.Model.DataTypes;

namespace HelpScoutMetrics.Model
{
    public static class StartupDebugMethods
    {
        public static void RunMethods()
        {
            GetUserList();
        }

        static Logger logger = LogManager.GetLogger("StartupDebug");

        #region Rating Retrieval      

        static int numRatingstoWaitFor = 0;
        static int namesIndex = 0;

        //Get the first returned user ratigns result to determine page count.
        private static void GetUserRatings()
        {
            DateTime start = new DateTime(2015, 9, 1, 0, 0, 0, DateTimeKind.Local);
            DateTime end = new DateTime(2015, 10, 31, 23, 59, 59, DateTimeKind.Local);
            ParameterAPIRequest<PagedReport<HelpScoutNet.Model.Report.Common.Rating>, UserRatingsRequest> request = new ParameterAPIRequest<PagedReport<HelpScoutNet.Model.Report.Common.Rating>, UserRatingsRequest>(new UserRatingsRequest(allUserIDs[namesIndex], start.ToUniversalTime(), end.ToUniversalTime(), Ratings.All), APICallType.UserRatings);
            request.ResultReady += RatingsFirstResults;
            HelpScoutRequestManager.NewQueueItem(request);
        }

        private static void RatingsFirstResults(object sender, BaseApiRequest<PagedReport<Rating>>.ResultReadyEventArgs<PagedReport<Rating>> e)
        {

            if(e.Result.Count > 0)
            {
                numRatingstoWaitFor = e.Result.Pages;
                logger.Log(LogLevel.Info, "Getting Replies For: " + e.Result.Results[0].RatingUserName + " Number of Results: " + e.Result.Count);
                GetUserRatings(e.Result.Pages, e.Result.Results[0].RatingUserID);
            }
            else
            {
                numRatingstoWaitFor = 0;
                namesIndex++;
                if (namesIndex < allUserIDs.Count)
                {
                    GetUserRatings();
                }
                else
                {
                    WriteToCSV(ApplicationData.UserRatings);
                    //QueueGetConversations();
                }
            }
        }

        //Gets users list to pull all user ID's for comparison
        private static void GetUserList()
        {
            BaseApiRequest<Paged<HelpScoutNet.Model.User>> request = new BaseApiRequest<Paged<HelpScoutNet.Model.User>>(APICallType.ListUsers);
            request.ResultReady += RecieveUserList;
            HelpScoutRequestManager.NewQueueItem(request);
        }

        private static void RecieveUserList(object sender, BaseApiRequest<Paged<HelpScoutNet.Model.User>>.ResultReadyEventArgs<Paged<HelpScoutNet.Model.User>> e)
        {
            foreach(HelpScoutNet.Model.User user in e.Result.Items)
            {
                allUserIDs.Add(user.Id);
            }
            GetUserRatings(); //Calls user ratigns once all the names are retrieved
        }

        private static void GetUserRatings(int pagecount, int userID)
        {
            DateTime start = new DateTime(2015, 9, 1, 0, 0, 0, DateTimeKind.Local);
            DateTime end = new DateTime(2015, 10, 31, 23, 59, 59, DateTimeKind.Local);
            for(int i = 1; i <= pagecount; i++)
            {
                ParameterAPIRequest<PagedReport<HelpScoutNet.Model.Report.Common.Rating>, UserRatingsRequest> request = new ParameterAPIRequest<PagedReport<HelpScoutNet.Model.Report.Common.Rating>, UserRatingsRequest>(new UserRatingsRequest(userID, start.ToUniversalTime(), end.ToUniversalTime(), Ratings.All) { Page = i}, APICallType.UserRatings);
                request.ResultReady += RecieveRatingsResults;
                HelpScoutRequestManager.NewQueueItem(request);
            }
        }

        private static void RecieveRatingsResults(object sender, BaseApiRequest<PagedReport<Rating>>.ResultReadyEventArgs<PagedReport<Rating>> e)
        {
            numRatingstoWaitFor--;
            foreach(Rating rate in e.Result.Results)
            {
                UserRating.Company whichCompany;

                if (FCRUserIDs.Contains(rate.RatingUserID))
                {
                    whichCompany = UserRating.Company.FCR;
                }
                else
                {
                    whichCompany = UserRating.Company.DoorDash;
                }

                UserRating userRating = new UserRating(rate.RatingUserID, rate.RatingUserName, rate.RatingID, rate.RatingCustomerID, rate.RatingCustomerName, rate.ID, rate.Number, rate.RatingCreatedAt.GetValueOrDefault(), rate.RatingComments)
                {
                    WhichCompany = whichCompany
                };

                //Necessary since multiple ratings can come from a single conversation. 
                if(!ApplicationData.UserRatings.ContainsKey(userRating.ConversationID))
                {
                    ApplicationData.UserRatings.Add(userRating.ConversationID, new List<UserRating>() { userRating });
                }
                else
                {
                    ApplicationData.UserRatings[userRating.ConversationID].Add(userRating);
                }
            }

            if(numRatingstoWaitFor == 0)
            {
                namesIndex++;
                if(namesIndex < allUserIDs.Count)
                {
                    GetUserRatings();
                }
                else
                {
                    WriteToCSV(ApplicationData.UserRatings);
                    //QueueGetConversations();
                }
            }
        }

        #endregion

        #region Conversation Retrieval

        static int conversationsRetrieved = 0;

        private static void QueueGetConversations()
        {
            foreach(List<UserRating> ratingList in ApplicationData.UserRatings.Values)
            {
                foreach(UserRating rating in ratingList)
                {
                    GetConversation(rating.ConversationID);
                }
            }
        }

        private static void GetConversation(int ID)
        {
            ParameterAPIRequest<Conversation, int> request = new ParameterAPIRequest<Conversation, int>(ID, APICallType.GetConversation);
            request.ResultReady += RecieveConversation;
            HelpScoutRequestManager.NewQueueItem(request);
        }

        private static void RecieveConversation(object sender, BaseApiRequest<Conversation>.ResultReadyEventArgs<Conversation> e)
        {
            if (e.Result.Tags != null)
            {
                if (e.Result.Tags.Count != 0)
                {
                    AddTagsToReport(e.Result.Id, e.Result.Tags);
                }

                conversationsRetrieved++;

                if (conversationsRetrieved == ApplicationData.UserRatings.Count)
                {
                    WriteToCSV(ApplicationData.UserRatings);
                }
            }
        }

        private static void AddTagsToReport(int conversationID, List<string> tags)
        {
            foreach(UserRating rating in ApplicationData.UserRatings[conversationID])
            {
                rating.AddTags(tags);
            }
        }

        #endregion

        private static void WriteToCSV(Dictionary<int,List<UserRating>> ratings)
        {
            using (StreamWriter writer = new StreamWriter("testoutput.csv"))
            {
                string firstLine = string.Format("{0},{1},{2},{3},{4},{5},{5},{7},{8},{9},{10},{11}",
                    "Conversation URL",
                    "Conversation Number",
                    "Conversation ID",
                    "Customer ID",
                    "Customer Name",
                    "User ID",
                    "User Name",
                    "Rating Time",
                    "Rating",
                    "Tags",
                    "FCR Or DoorDash",
                    "Comments");
                writer.WriteLine(firstLine);
                writer.Flush();

                foreach (List<UserRating> ratingList in ratings.Values)
                {
                    foreach(UserRating rating in ratingList)
                    {
                        string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},\"{11}\"",
                        rating.ConversationURL,
                        rating.ConversationNumber,
                        rating.ConversationID,
                        rating.CustomerID,
                        rating.CustomerName,
                        rating.UserID,
                        rating.UserName,
                        rating.RatingTime.ToString(),
                        rating.Rating,
                        rating.Tags,
                        rating.WhichCompany.ToString(),
                        CleanStringForCSV(rating.RatingComment));

                        writer.WriteLine(line);
                        writer.Flush();
                    }                    
                }
                writer.Close();
            }
        }


        //Cleans all /b from a string
        private static string CleanStringForCSV(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return value.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(lineSeparator, string.Empty).Replace(paragraphSeparator, string.Empty);
        }

        //hold all use ID's for pulling reports, is dynamic.
        static List<int> allUserIDs = new List<int>();

        //Holds just the FCR suer ID's for comparison purposes, not dynamic
        static List<int> FCRUserIDs = new List<int>()
        {
            89080,
            89087,
            83076,
            87210,
            89097,
            80767,
            89085,
            87201,
            89082,
            89095,
            79798,
            83070,
            79812,
            79805,
            83079,
            89078,
            83069,
            87217,
            80774,
            89088,
            83083,
            83072,
            79804,
            79808,
            89081,
            87214,
            87207,
            80772,
            89084,
            87209,
            83078,
            89079,
            87199,
            87215,
            83081,
            83084,
            87216,
            89091,
            89096,
            80766,
            83077,
            80769,
            79806,
            87205,
            89086,
            79799,
            79807,
            79802,
            89090,
            87212,
            89094,
            83075,
            87203,
            83074
        };
    }
}
