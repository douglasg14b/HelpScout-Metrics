using HelpScoutMetrics.Model.DataTypes;
using HelpScoutMetrics.Scripts.Model;
using HelpScoutMetrics.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using HelpScoutNet;
using HelpScoutNet.Model;
using NLog;
using HelpScoutMetrics.Logging;
using HelpScoutMetrics.Scripts.Model.WindowLogic;
using System.Net.Http;

namespace HelpScoutMetrics.Model.WindowLogic
{
    public static class UserListWindowLogic
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private delegate void ViewModelUserListCallback(List<DataTypes.User> users); //Callback delegate for the viewmodel
        private static ViewModelUserListCallback userListCallbackHandler; //Handler for the viewmodel callback

        public static string[] HelpScoutUseListStringStatus = new string[2]
        {
            "Successfully Loaded HelpScout Users",
            "Failed To Load HelpScout Users"
        };

        //Adds a new call to the API queue to get the users list
        public static void GetHelpScoutUserList(UserListViewModel viewModel)
        {
            userListCallbackHandler = new ViewModelUserListCallback(viewModel.RecieveHelpScoutUserList); //Sets teh delegate for the viewmodel callback
            BaseApiRequest<Paged<HelpScoutNet.Model.User>> apiRequest = new BaseApiRequest<Paged<HelpScoutNet.Model.User>>(APICallType.ListUsers);
            apiRequest.ResultReady += AcceptHelpScoutUserList;
            apiRequest.resultsFailedHandler = new BaseAPIRequest.ResultsFailed(viewModel.FailedToRetrieveHelpScoutUsersList);
            HelpScoutRequestManager.NewQueueItem(apiRequest);
        }

        //Recieves the callback from the helpscout queue with the data
        private static void AcceptHelpScoutUserList(object sender, BaseApiRequest<Paged<HelpScoutNet.Model.User>>.ResultReadyEventArgs<Paged<HelpScoutNet.Model.User>> e)
        {
            List<DataTypes.User> usersToReturn = new List<DataTypes.User>();
            foreach (HelpScoutNet.Model.User userItem in e.Result.Items)
            {
                usersToReturn.Add(new HelpScoutMetrics.Model.DataTypes.User() { Name = userItem.FirstName + " " + userItem.LastName, ID = userItem.Id, Valid = true });
            }
            userListCallbackHandler(usersToReturn);
        }


        public static void SaveUserList(UserListViewModel viewModel)
        {
            ApplicationData.Users = viewModel.UsersList;
            //ApplicationData.QuickStatistics.UserQuickStats.Clear();
            foreach (DataTypes.User userObject in ApplicationData.Users.Users)
            {
                if (!CheckIfUserExistsInList(userObject) && userObject.ID != 0 && userObject.Valid)
                {
                    ApplicationData.QuickStatistics.UserQuickStats.Add(new UserQuickStat(userObject.Name, userObject.ID));
                }
            }

            for (int i = ApplicationData.QuickStatistics.UserQuickStats.Count - 1; i >= 0; i--)
            {
                if (!CheckIfUserExistsInList(ApplicationData.QuickStatistics.UserQuickStats[i].Name, ApplicationData.Users.Users))
                {
                    ApplicationData.QuickStatistics.UserQuickStats.RemoveAt(i);
                }
            }
        }

        //Checks if the user exists in the quickstats list
        private static bool CheckIfUserExistsInList(DataTypes.User user)
        {
            foreach (UserQuickStat quickStat in ApplicationData.QuickStatistics.UserQuickStats)
            {
                if(quickStat.Name == user.Name)
                {
                    return true;
                }
            }
            return false;
        }

        //Checks if the user exists in the quickstats list
        private static bool CheckIfUserExistsInList(string name, IList<DataTypes.User> list)
        {
            foreach (DataTypes.User userObject in list)
            {
                if (userObject.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public static UserList CreateNewUserList()
        {
            UserList userList = new UserList();
            userList.Users.Add(new HelpScoutMetrics.Model.DataTypes.User() { Name = "test" });
            return userList;
        }

        public static DataTypes.User FindUserByName(string name, List<DataTypes.User> users)
        {
            foreach(DataTypes.User userObject in users)
            {
                if(userObject.Name == name)
                {
                    return userObject;
                }
            }
            return null;
        }

        public static void CheckUserListValidity(List<DataTypes.User> helpScoutUsers, UserList userList)
        {
            foreach(DataTypes.User userObject in userList.Users)
            {
                foreach(DataTypes.User helpScoutUser in helpScoutUsers)
                {
                    if(userObject.Name == helpScoutUser.Name)
                    {
                        userObject.ID = helpScoutUser.ID;
                        userObject.Valid = true;
                        break;
                    }
                    else
                    {
                        userObject.Valid = false;
                    }
                }
            }
        }
    }
}
