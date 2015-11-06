using HelpScoutMetrics.Scripts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Model.DataTypes
{
    [Serializable]
    public class ColumnPreferences
    {
        public ColumnPreferences(List<ColumnListItem> columnItems)
        {
            QuickStatColumns = columnItems;
        }

        public ColumnPreferences(bool defaultSetup)
        {
            if(defaultSetup)
            {
                QuickStatColumns.Add(new ColumnListItem("Name", "Name", "NameColumnIndex", true, 0));
                QuickStatColumns.Add(new ColumnListItem("Total Conversations", "TotalConversations", "TotalConversationsColumnIndex", true, 1));
                QuickStatColumns.Add(new ColumnListItem("Total Replies", "TotalReplies", "TotalRepliesColumnIndex", true, 2));
                QuickStatColumns.Add(new ColumnListItem("Handle Time", "HandleTime", "HandleTimeColumnIndex", true, 3));
                QuickStatColumns.Add(new ColumnListItem("Great Ratings", "GreatRatingsCount", "GreatRatingsColumnIndex", true, 4));
                QuickStatColumns.Add(new ColumnListItem("Total Ratings", "TotalRatingsCount", "TotalRatingsColumnIndex", true, 5));
                QuickStatColumns.Add(new ColumnListItem("Happiness Score", "HappinessScore", "HappinessScoreColumnIndex", false, 6));
                QuickStatColumns.Add(new ColumnListItem("Customers Helped", "CustomersHelped", "CustomersHelpedColumnIndex", false, 7));
                QuickStatColumns.Add(new ColumnListItem("Conversations Created", "ConversationsCreated", "ConversationsCreatedColumnIndex", false, 8));
            }
        }

        public ColumnPreferences() { }



        private List<ColumnListItem> m_QuickStatColumns = new List<ColumnListItem>();
        public List<ColumnListItem> QuickStatColumns
        {
            get { return m_QuickStatColumns; }
            set { m_QuickStatColumns = value; }
        }
    }
}
