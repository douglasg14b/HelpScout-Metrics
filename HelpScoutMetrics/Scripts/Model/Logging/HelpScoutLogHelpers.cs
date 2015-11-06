using HelpScoutNet;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Logging
{
    public static class HelpScoutLogHelpers
    {
        static Logger logger = LogManager.GetLogger("HelpScoutLogHelper");
        public static void LogHelpScoutException(HelpScoutApiException exception, string loggerName)
        {
            switch(exception.Code)
            {
                case 400:
                    logger.Error("Error: 400 Request Not Formatted Correctly In: " + loggerName, exception);
                    break;
                case 401:
                    logger.Error("Error: 401 Invalid API Key In: " + loggerName, exception);
                    break;
                case 402:
                    logger.Error("Error: 402 API Key Suspended In: " + loggerName, exception);
                    break;
                case 403:
                    logger.Error("Error: 403 Access Denied In: " + loggerName, exception);
                    break;
                case 404:
                    logger.Error("Error: 404 Selected Resource Was Not Found In: " + loggerName, exception);
                    break;
                case 405:
                    logger.Error("Error: 405 Invalid Method Type In: " + loggerName, exception);
                    break;
                case 409:
                    logger.Error("Error: 409 Resource Being Created Already Exists In: " + loggerName, exception);
                    break;
                case 429:
                    logger.Error("Error: 429 Too Many Requests, Throttle Limit Reached In: " + loggerName, exception);
                    break;
                case 500:
                    logger.Error("Error: 500 Application Or Server Error In: " + loggerName, exception);
                    break;
                case 503:
                    logger.Error("Error: 503 Service Temporarily Unavailable In: " + loggerName, exception);
                    break;
            }
        }
    }
}
