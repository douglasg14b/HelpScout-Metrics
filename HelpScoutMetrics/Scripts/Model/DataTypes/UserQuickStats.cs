using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpScoutMetrics.ViewModel;

namespace HelpScoutMetrics.Model.DataTypes
{
    public class UserQuickStat : ViewModelBase
    {
        public UserQuickStat(string name, int id, int totalReplies, int totalConversations, double happinessScore, double handleTime, int customersHelped, int conversationsCreated, int greatRatings, int totalRatings)
        {
            Name = name;
            UserID = id;
            TotalReplies = totalReplies;
            TotalConversations = totalConversations;
            HappinessScore = happinessScore;
            HandleTime = handleTime;
            CustomersHelped = customersHelped;
            ConversationsCreated = conversationsCreated;
            GreatRatingsCount = greatRatings;
            TotalRatingsCount = totalRatings;
        }

        public UserQuickStat(string name, int id, int totalReplies, int totalConversations, double handleTime, int customersHelped, int conversationsCreated)
        {
            Name = name;
            UserID = id;
            TotalReplies = totalReplies;
            TotalConversations = totalConversations;
            HandleTime = handleTime;
            CustomersHelped = customersHelped;
            ConversationsCreated = conversationsCreated;
        }

        public UserQuickStat(string name, int ID)
        {
            Name = name;
            UserID = ID;
            TotalReplies = 0;
            TotalConversations = 0;
        }

        public UserQuickStat(string name)
        {
            Name = name;
            TotalReplies = 0;
            TotalConversations = 0;
        }

        //The users name
        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; RaisePropertyChanged("Name"); }
        }

        private int m_UserID;
        public int UserID
        {
            get { return m_UserID; }
            set { m_UserID = value; RaisePropertyChanged("UserID"); }
        }

        private int m_TotalReplies;
        public int TotalReplies
        {
            get { return m_TotalReplies; }
            set { m_TotalReplies = value; RaisePropertyChanged("TotalReplies"); }
        }

        private int m_TotalConversations;
        public int TotalConversations
        {
            get { return m_TotalConversations; }
            set { m_TotalConversations = value; RaisePropertyChanged("TotalConversations"); }
        }

        private double m_Happiness;
        public double HappinessScore
        {
            get { return m_Happiness; }
            set { m_Happiness = (Math.Truncate(value * 100) / 100); RaisePropertyChanged("HappinessScore"); }
        }

        private int m_GreatRatingsCount;
        public int GreatRatingsCount
        {
            get { return m_GreatRatingsCount; }
            set { m_GreatRatingsCount = value; RaisePropertyChanged("GreatRatingsCount"); }
        }

        private int m_TotalRatingsCount;
        public int TotalRatingsCount
        {
            get { return m_TotalRatingsCount; }
            set { m_TotalRatingsCount = value; RaisePropertyChanged("TotalRatingsCount"); }
        }

        private double m_HandleTime;
        public double HandleTime
        {
            get { return m_HandleTime; }
            set { m_HandleTime = (Math.Truncate(value * 100) / 100); RaisePropertyChanged("HandleTime"); }
        }

        private int m_CustomersHelped;
        public int CustomersHelped
        {
            get { return m_CustomersHelped; }
            set { m_CustomersHelped = value; RaisePropertyChanged("CustomersHelped"); }
        }

        private int m_ConversationsCreated;
        public int ConversationsCreated
        {
            get { return m_ConversationsCreated; }
            set { m_ConversationsCreated = value; RaisePropertyChanged("ConversationsCreated"); }
        }

        public void UpdateQuickStat(UserQuickStat quickStat)
        {
            UserID = quickStat.UserID;
            TotalReplies = quickStat.TotalReplies;
            TotalConversations = quickStat.TotalConversations;
            HappinessScore = quickStat.HappinessScore;
            HandleTime = quickStat.HandleTime;
            CustomersHelped = quickStat.CustomersHelped;
            ConversationsCreated = quickStat.ConversationsCreated;
            TotalRatingsCount = quickStat.TotalRatingsCount;
            GreatRatingsCount = quickStat.GreatRatingsCount;
        }

        /// <summary>
        ///  Updates The QuickStat Item With User Report Parameters
        /// </summary>
        public void UpdateQuickStat(int totalReplies, int totalConversations, double happinessScore, double handleTime, int customersHelped, int conversationsCreated)
        {
            TotalReplies = totalReplies;
            TotalConversations = totalConversations;
            HappinessScore = happinessScore;
            HandleTime = handleTime;
            CustomersHelped = customersHelped;
            ConversationsCreated = conversationsCreated;
        }

        /// <summary>
        ///  Updates The QuickStat Item With Happiness Report Parameters
        /// </summary>
        public void UpdateQuickStat(int totalRatings, int greatRatings)
        {
            TotalRatingsCount = totalRatings;
            GreatRatingsCount = greatRatings;
        }
    }
}
