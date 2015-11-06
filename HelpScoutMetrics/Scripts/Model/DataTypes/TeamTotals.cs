using HelpScoutMetrics.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Scripts.Model.DataTypes
{
    public class TeamTotals : ViewModelBase
    {
        private int m_TotalConversations;
        public int TotalConversations
        {
            get { return m_TotalConversations; }
            set { m_TotalConversations = value; RaisePropertyChanged("TotalConversations"); }
        }

        private int m_TotalReplies;
        public int TotalReplies
        {
            get { return m_TotalReplies; }
            set { m_TotalReplies = value; RaisePropertyChanged("TotalReplies"); }
        }


        private int m_CustomersHelped;
        public int CustomersHelped
        {
            get { return m_CustomersHelped; }
            set { m_CustomersHelped = value; RaisePropertyChanged("CustomersHelped"); }
        }


        private int m_TicketsClosed;
        public int TicketsClosed
        {
            get { return m_TicketsClosed; }
            set { m_TicketsClosed = value; RaisePropertyChanged("TicketsClosed"); }
        }

        private int m_TotalUsers;
        public int TotalUsers
        {
            get { return m_TotalUsers; }
            set { m_TotalUsers = value; RaisePropertyChanged("TotalUsers"); }
        }
    }
}
