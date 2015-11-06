using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Model
{
    public class ParameterAPIRequest<T,R> : BaseApiRequest<T>
    {
        public ParameterAPIRequest(R requestArgs, APICallType requestType)
            : base(requestType)
        {
            RequestArgs = requestArgs;
        }

        public R RequestArgs { get; private set; }

    }
}
