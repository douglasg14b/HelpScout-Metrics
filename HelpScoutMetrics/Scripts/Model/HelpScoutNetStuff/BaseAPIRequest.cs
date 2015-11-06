using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Model
{

    public abstract class BaseAPIRequest : IDisposable
    {
        public BaseAPIRequest(APICallType requestType)
        {
            RequestType = requestType;
        }

        public delegate void ResultsFailed(string message);
        public delegate void RequestCancelled();

        public ResultsFailed resultsFailedHandler;
        public RequestCancelled requestCancelledHandler;


        public abstract void Dispose();
        public APICallType RequestType { get; private set; }

        public void PerformResultFailed(string message)
        {
            if (resultsFailedHandler != null)
            {
                resultsFailedHandler(message);
            }
        }

        public void PerformCancelRequest()
        {
            if(requestCancelledHandler != null)
            {
                requestCancelledHandler();
            }
        }
    }

    public class BaseApiRequest<T> : BaseAPIRequest 
    {
        protected readonly ManualResetEventSlim _signal;
        protected Exception requestException;
        protected T _result;

        public BaseApiRequest(APICallType requestType)
            : base(requestType)
        {
            _signal = new ManualResetEventSlim(false);
        }

        //Returns the result.
        public T GetResult()
        {
            _signal.Wait();
            if (requestException != null)
                throw new Exception("Exception during request processing. See inner exception for details", requestException);
            return _result;
        }

        public T GetResult(CancellationToken token)
        {
            _signal.Wait(token);
            if (requestException != null)
                throw new Exception("Exception during request processing. See inner exception for details", requestException);
            return _result;
        }

        public bool TryGetResult(TimeSpan timeout, out T result)
        {
            result = default(T);
            if (_signal.Wait(timeout))
            {
                result = _result;
                return true;
            }
            return false;
        }

        public virtual void SetResult(T result)
        {
            _result = result;
            _signal.Set();
            var handler = ResultReady;
            if (handler != null)
                handler(this, new ResultReadyEventArgs<T>(_result));
        }

        public override void Dispose()
        {
            _signal.Dispose();
        }

        public event EventHandler<ResultReadyEventArgs<T>> ResultReady;

        public class ResultReadyEventArgs<T> : EventArgs
        {
            public ResultReadyEventArgs(T result)
            {
                this.Result = result;
            }

            public T Result { get; private set; }
        }
    }
}
