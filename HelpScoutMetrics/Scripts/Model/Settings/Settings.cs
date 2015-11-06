using HelpScoutMetrics.Scripts.Model;
using HelpScoutMetrics.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Model.DataTypes
{
    [Serializable]
    public class Settings : INotifyPropertyChanged
    {
        [field: NonSerialized]
        private string m_APIKey;
        [System.Xml.Serialization.XmlIgnore]
        public string APIKey
        {
            get { return m_APIKey; }
            set { m_APIKey = value; }
        }

        public bool SaveAPIKey { get; set; }

        [field: NonSerialized]
        private bool m_ValidAPIKeyExists;
        [System.Xml.Serialization.XmlIgnore]
        public bool ValidAPIKeyExists
        {
            get { return m_ValidAPIKeyExists; }
            set 
            { 
                m_ValidAPIKeyExists = value; 
                RaisePropertyChanged("ValidAPIKeyExists");
                if (ApplicationData.MainViewModel != null)
                {
                    ApplicationData.MainViewModel.EnableLoadQuickStatsButton = value;
                }
            }
        }

        private int m_APIThrottleCount = 50;
        public int APIThrottleCount
        {
            get { return m_APIThrottleCount; }
            set { m_APIThrottleCount = value; RaisePropertyChanged("APIThrottleCount"); }
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
