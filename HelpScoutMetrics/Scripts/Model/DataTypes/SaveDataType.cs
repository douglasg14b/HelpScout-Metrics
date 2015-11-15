using HelpScoutMetrics.Scripts.Model.DataTypes;
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
        public SaveDataType(Settings settings, ColumnPreferences columns, UserList userList, List<UserRating> userRatings)
        {
            Settings = settings;
            Columns = columns;
            UsersList = userList;
            UserRatings = userRatings;
        }

        public SaveDataType(){}

        public UserList UsersList { get; set; }
        public Settings Settings { get; set; }
        public ColumnPreferences Columns { get; set; }
        public List<UserRating> UserRatings { get; set; }
        //public UserList UsersList { get; set; }
    }
}
