using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Model.DataTypes
{
    [Serializable]
    public class SaveDataType
    {
        public SaveDataType(Settings settings, ColumnPreferences columns, UserList userList)
        {
            Settings = settings;
            Columns = columns;
            UsersList = userList;
        }

        public SaveDataType(){}

        public UserList UsersList { get; set; }
        public Settings Settings { get; set; }
        public ColumnPreferences Columns { get; set; }
        //public UserList UsersList { get; set; }
    }
}
