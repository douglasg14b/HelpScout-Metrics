using HelpScoutMetrics.Model.DataTypes;
using HelpScoutMetrics.Model.SaveAndLoad;
using HelpScoutMetrics.Model.WindowLogic;
using HelpScoutMetrics.Scripts.Model;
using HelpScoutMetrics.Views;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HelpScoutMetrics.ViewModel
{
    public class UserListViewModel : ViewModelBase
    {
        public UserListViewModel()
        {
           //LoadUserList();
        }

        public static Logger logger = LogManager.GetLogger("UserListViewModel");

        public MainWindow MainWindow { get; set; }
        public UserListView UserListView { get; set; }

        private UserList m_UserList;
        public UserList UsersList
        {
            get { return m_UserList; }
            set { m_UserList = value; RaisePropertyChanged("UsersList"); }
        }

        private List<string> m_TestItems;
        public List<string> TestItems
        {
            get { return m_TestItems; }
            set { m_TestItems = value; RaisePropertyChanged("TestItems"); }
        }

        private string m_NewUserName;
        public string NewUserName
        {
            get { return m_NewUserName; }
            set { m_NewUserName = value; RaisePropertyChanged("NewUserName"); }
        }

        private string m_NewUserNames;
        public string NewUserNames
        {
            get { return m_NewUserNames; }
            set { m_NewUserNames = value; RaisePropertyChanged("NewUserNames"); }
        }

        private string m_HelpScoutUserListStatus = "Attempting To Load HelpScout Users...";
        public string HelpScoutUserListStatus
        {
            get { return m_HelpScoutUserListStatus; }
            set { m_HelpScoutUserListStatus = value; RaisePropertyChanged("HelpScoutUserListStatus"); }
        }

        private Brush m_HelpScoutUserListStatusColor = new SolidColorBrush(Color.FromArgb(255, 187, 95, 32));
        public Brush HelpScoutUserListStatusColor
        {
            get { return m_HelpScoutUserListStatusColor; }
            set { m_HelpScoutUserListStatusColor = value; RaisePropertyChanged("HelpScoutUserListStatusColor"); }
        }

        private bool m_HelpScoutUserListLoaded;
        public bool HelpScoutUserListLoaded
        {
            get { return m_HelpScoutUserListLoaded; }
            set 
            { 
                if(value)
                {
                    HelpScoutUserListStatus = UserListWindowLogic.HelpScoutUseListStringStatus[0];

                    Brush brush = new SolidColorBrush(Color.FromArgb(255, 27, 147, 38));
                    brush.Freeze();
                    HelpScoutUserListStatusColor = brush;

                    ReverifyUserList();
                }
                else
                {
                    HelpScoutUserListStatus = UserListWindowLogic.HelpScoutUseListStringStatus[1];

                    Brush brush = new SolidColorBrush(Color.FromArgb(255, 147, 27, 27));
                    brush.Freeze();
                    HelpScoutUserListStatusColor = brush;
                }

                m_HelpScoutUserListLoaded = value; 
                RaisePropertyChanged("HelpScoutUserListLoaded"); 
            }
        }

        private List<User> m_HelpScoutUsersList;
        public List<User> HelpScoutUsersList
        {
            get { return m_HelpScoutUsersList; }
            set { m_HelpScoutUsersList = value; RaisePropertyChanged("HelpScoutUsersList"); }
        }

        private List<string> m_HelpScoutUsersListStrings = new List<string>();
        public List<string> HelpScoutUsersListStrings
        {
            get { return m_HelpScoutUsersListStrings; }
            set { m_HelpScoutUsersListStrings = value; RaisePropertyChanged("HelpScoutUsersListStrings"); }
        }

        public async void RetrieveHelpScoutUserList()
        {
            HelpScoutUsersListStrings.Clear();
            await Task.Run(new Action(() => UserListWindowLogic.GetHelpScoutUserList(this)));
        }

        //Recieves a callback with the usersList
        public void RecieveHelpScoutUserList(List<User> usersList)
        {
            if (usersList != null)
            {
                HelpScoutUsersList = usersList;
                foreach (User userObject in usersList)
                {
                    HelpScoutUsersListStrings.Add(userObject.Name);
                }
                HelpScoutUserListLoaded = true;
            }
            else
            {
                HelpScoutUserListLoaded = false;
            }
        }

        internal void FailedToRetrieveHelpScoutUsersList(string message)
        {
            HelpScoutUserListLoaded = false;
        }

        private User MatchUserID(string name)
        {
            return UserListWindowLogic.FindUserByName(name, HelpScoutUsersList);
        }

        //Removes a user from the Views user list called when an "x" if hit beside a user in the list
        public void RemoveUser()
        {
            User user = UserListView.NamesDataGrid.SelectedItem as User;
            UsersList.Users.Remove(user);
            logger.Log(LogLevel.Debug, "Removed User: " + user.Name + " From User List");
            if (user.Valid)
            {
                AddUserToAutsuggestions(user.Name);
            }
        }

        //Adds a new user to the Views user list Called from the add user button
        public void AddUser()
        {
            if (!string.IsNullOrEmpty(NewUserName) && HelpScoutUserListLoaded)
            {
                User user = MatchUserID(NewUserName);
                if(user != null)
                {
                    UsersList.Users.Add(user);
                    RemoveUserFromAutosuggestions(user.Name);
                    logger.Log(LogLevel.Debug, "Added New Valid User: " + user.Name + " To User List");
                }
                else
                {
                    UsersList.Users.Add(new User() { Name = NewUserName });
                    logger.Log(LogLevel.Debug, "Added New Non-Validated User: " + NewUserName + " To User List");
                }
            }
            else
            {
                UsersList.Users.Add(new User() { Name = NewUserName });
                logger.Log(LogLevel.Debug, "Added New Non-Validated User: " + NewUserName + " To User List");
            }
            ClearNewUserNameTextBox();
        }

        //Adds a list of users to the Views user list. Called from the add users button
        public void AddUsers()
        {
            if (!string.IsNullOrEmpty(NewUserNames) && HelpScoutUserListLoaded)
            {
                string[] users = NewUserNames.Split(new string[] {"\r\n", "\n", ","}, StringSplitOptions.RemoveEmptyEntries).Select(name => name.Trim()).ToArray();
                foreach(string userString in users)
                {
                    User user = MatchUserID(userString);
                    if (user != null)
                    {
                        UsersList.Users.Add(user);
                        RemoveUserFromAutosuggestions(user.Name);
                        logger.Log(LogLevel.Debug, "Added New Valid User: " + user.Name + " To User List");
                    }
                    else
                    {
                        UsersList.Users.Add(new User() { Name = userString });
                        logger.Log(LogLevel.Debug, "Added New Non-Validated User: " + userString + " To User List");
                    }
                }
            }
            else
            {
                string[] users = NewUserNames.Split(new string[] { "\r\n", "\n", "," }, StringSplitOptions.RemoveEmptyEntries).Select(name => name.Trim()).ToArray();
                foreach (string userString in users)
                {
                    UsersList.Users.Add(new User() { Name = userString });
                    logger.Log(LogLevel.Debug, "Added New Non-Validated User: " + userString + " To User List");
                }
            }
            ClearNewUserNamesTextBox();
        }

        //Removes a user from the auto suggestions list
        private void RemoveUserFromAutosuggestions(string name)
        {
            if(HelpScoutUsersListStrings.Contains(name))
            {
                HelpScoutUsersListStrings.Remove(name);
                logger.Log(LogLevel.Debug, "Removed User: " + name + "From AutoSuggestions");
            }
        }

        //Adds a user from the auto suggestions list
        private void AddUserToAutsuggestions(string name)
        {
            foreach(string user in HelpScoutUsersListStrings)
            {
                if(name == user)
                {
                    return;
                }
            }
            HelpScoutUsersListStrings.Add(name);
            logger.Log(LogLevel.Debug, "Added " + name + " Back To AutoComplete List");
        }

        //Clears the new user textbox
        public void ClearNewUserNameTextBox()
        {
            NewUserName = string.Empty;
        }

        public void ClearNewUserNamesTextBox()
        {
            NewUserNames = string.Empty;
        }

        public void SetupFreshViewModel()
        {
            m_HelpScoutUserListLoaded = false;
            HelpScoutUserListStatusColor = new SolidColorBrush(Color.FromArgb(255, 187, 95, 32));
            HelpScoutUserListStatus = "Attempting To Load HelpScout Users...";
            LoadUserList();
            RetrieveHelpScoutUserList();
        }

        public void SaveUserList()
        {
            UserListWindowLogic.SaveUserList(this);
            logger.Log(LogLevel.Debug, "Saved Users List");
            CloseFlyout();
        }

        //Verifies the users on the users list with the helpscout users
        public void ReverifyUserList()
        {
            UserListWindowLogic.CheckUserListValidity(HelpScoutUsersList, UsersList);
        }

        public void LoadUserList()
        {
            UserList userList;
            if(ApplicationData.Users != null)
            {
                userList = XMLSerialize<UserList>.CopyData(ApplicationData.Users);
            }
            else
            {
                userList = UserListWindowLogic.CreateNewUserList();
            }
            UsersList = userList;
        }

        public void ResetUserList()
        {
            UsersList = new UserList();
            logger.Log(LogLevel.Debug, "Reset Users List");
        }

        public void CloseFlyout()
        {
            MainWindow.UserListFlyout.IsOpen = false;
            logger.Log(LogLevel.Info, "Closed Users List Flyout");
        }
    }
}
