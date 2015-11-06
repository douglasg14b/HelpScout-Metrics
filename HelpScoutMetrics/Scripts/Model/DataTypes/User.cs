using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Class that defines a User
namespace HelpScoutMetrics.Model.DataTypes
{
    [Serializable]
    public class User : INotifyPropertyChanged
    {
        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; RaisePropertyChanged("Name"); }
        }

        private int m_ID;
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; RaisePropertyChanged("ID"); }
        }

        private bool m_Valid;
        public bool Valid
        {
            get { return m_Valid; }
            set { m_Valid = value; RaisePropertyChanged("Valid"); }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler temp = PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
