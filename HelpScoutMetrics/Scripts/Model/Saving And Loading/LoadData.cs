using HelpScoutMetrics.Model.DataTypes;
using HelpScoutMetrics.Scripts.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Model.SaveAndLoad
{
    public class LoadData
    {
        public static void LoadUserData()
        {
            string path = "Data\\UserData.xml";
            if(CheckIfDataExists(path))
            {
                SaveDataType savedData = XMLSerialize<SaveDataType>.Deserialize(FileStreamReadWrite.ReadFromFile(path));
                if (savedData.Settings != null)
                    ApplicationData.ApplicationSettings = savedData.Settings;
                if (savedData.Columns != null)
                    ApplicationData.ApplicationColumns = savedData.Columns;
                if (savedData.UsersList != null)
                    ApplicationData.Users = savedData.UsersList;
            }
            else
            {
                CreateDefaultSettings();
            }
        }

        private static void CreateDefaultSettings()
        {
            ApplicationData.ApplicationSettings = new Settings();
        }

        private static bool CheckIfDataExists(string path)
        {
            return File.Exists(path);
        }
    }
}
