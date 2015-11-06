using HelpScoutMetrics.Scripts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Used to store preferances and user interface setups
namespace HelpScoutMetrics.Model.DataTypes
{
    public class UserInterfacePreferances
    {
        private static List<ColumnListItem> m_QuickStatsColums = new List<ColumnListItem>();
        public static List<ColumnListItem> QuickStatsColums
        {
            get { return m_QuickStatsColums; }
            set { m_QuickStatsColums = value; }
        }
    }
}
