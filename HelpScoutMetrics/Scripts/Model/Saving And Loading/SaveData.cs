using HelpScoutMetrics.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Model.SaveAndLoad
{
    public static class SaveData
    {
        public static void SaveAllData(SaveDataType data)
        {
            if (!CheckForFolder("Data"))
                CreateDirectory("Data");

            FileStreamReadWrite.WriteToFile(XMLSerialize<SaveDataType>.Serialize(data), "Data\\UserData.xml");
        }

        private static bool CheckForFolder(string path)
        {
            return Directory.Exists(path);
        }

        private static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
