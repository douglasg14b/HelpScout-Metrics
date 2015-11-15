using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Holds data on a rating for a specific user.

namespace HelpScoutMetrics.Model.DataTypes
{
    [Serializable]
    public class UserRating
    {
        public UserRating(int userID, string user, int rating, int customerID, string customerName, int conversationID, int conversationNumber, DateTime ratingTime, string comment)
        {
            UserID = userID;
            UserName = user;
            ConvertRating(rating);
            CustomerID = customerID;
            CustomerName = customerName;
            ConversationID = conversationID;
            ConversationNumber = conversationNumber;
            RatingTime = ratingTime;
            Comments = comment;

            ConversationURL = "https://secure.helpscout.net/conversation/" + ConversationID + "/" + ConversationNumber;
        }

        public UserRating() { }


        public int UserID { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// 1 = Great
        /// 2 = OK
        /// 3 = Bad
        /// </summary>
        public int Rating { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public int ConversationID { get; set; }
        public int ConversationNumber { get; set; }
        public DateTime RatingTime { get; set; }
        public string Comments { get; set; }

        public string ConversationURL { get; set; }

        //Temp fields, not dynamic TODO: Remove/Fix
        public Company FCRorDoorDash { get; set; }
        public enum Company
        {
            FCR,
            DoorDash
        }

        //public string Tags { get; set; }
        public List<string> Tags { get; set; }

        //public void AddTags(List<string> tags)
        //{
        //    string concatenatedTags = "";
        //    for(int i = 0; i < tags.Count; i++)
        //    {
        //        concatenatedTags = concatenatedTags + tags[i] + ", ";
        //        if (i == tags.Count - 1)
        //        {
        //            concatenatedTags = concatenatedTags + tags[i];
        //        }
        //    }
        //    Tags = concatenatedTags;
        //}

        public void ConvertRating(int rating)
        {
            switch(rating)
            {
                case 1:
                    Rating = 1;
                    break;
                case 2:
                    rating = 0;
                    break;
                case 3:
                    rating = -1;
                    break;
            }
        }
    }
}
