using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Model
{
    public class UserAPIRequest<T,R> : ParameterAPIRequest<T,R>
    {
        public UserAPIRequest( R requestArgs, APICallType requestType, int userID, string name)
            : base(requestArgs, requestType)
        {
            UserID = userID;
            Name = name;
        }

        int UserID { get; set; }
        string Name { get; set; }

        public override void SetResult(T result)
        {
            base.SetResult(result);
            _signal.Set();
            var handler = UserResultReady;
            if (handler != null)
                handler(this, new UserResultReadyEventArgs<T>(_result, UserID, Name));
        }


        public event EventHandler<UserResultReadyEventArgs<T>> UserResultReady;

        public class UserResultReadyEventArgs<T> : ResultReadyEventArgs<T>
        {
            public UserResultReadyEventArgs(T results, int userID, string name)
                : base (results)
            {
                UserID = userID;
                Name = name;
            }

            public int UserID { get; set; }
            public string Name { get; set; }
        }
    }
}
