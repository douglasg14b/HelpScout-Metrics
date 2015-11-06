using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// A list of all saved users
namespace HelpScoutMetrics.Model.DataTypes
{
    [Serializable]
    public class UserList : INotifyPropertyChanged
    {
        private ObservableCollection<User> m_Users = new ObservableCollection<User>();
        public ObservableCollection<User> Users
        {
            get { return m_Users; }
            set { m_Users = value; RaisePropertyChanged("Users"); }
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
