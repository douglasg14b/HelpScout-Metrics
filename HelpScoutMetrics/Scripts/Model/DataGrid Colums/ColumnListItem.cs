using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//A Column item containing basic information
//So a real column can be instantiated when needed
namespace HelpScoutMetrics.Model.DataTypes
{
    [Serializable]
    public class ColumnListItem
    {
        public ColumnListItem(string name, string bindingName, string displayIndexBindingName, bool enabled, int index)
        {
            Name = name;
            DataBindingName = bindingName;
            DisplayIndexBindingName = displayIndexBindingName;
            Enabled = enabled;
            Index = index;
        }

        public ColumnListItem() { }

        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private string m_DataBindingName;
        public string DataBindingName
        {
            get { return m_DataBindingName; }
            set { m_DataBindingName = value; }
        }

        private string m_DisplayIndexBindingName;
        public string DisplayIndexBindingName
        {
            get { return m_DisplayIndexBindingName; }
            set { m_DisplayIndexBindingName = value; }
        }

        private int m_Index;
        public int Index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

        private bool m_Enabled;
        public bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }
        public string ColumnType { get; set; } //Unused
    }
}
