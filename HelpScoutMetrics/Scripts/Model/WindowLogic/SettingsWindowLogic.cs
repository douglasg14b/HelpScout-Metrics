using HelpScoutMetrics.Scripts.Model;
using HelpScoutMetrics.ViewModel;
using HelpScoutNet;
using HelpScoutNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NLog;
using HelpScoutMetrics.Logging;

namespace HelpScoutMetrics.Model.WindowLogic
{
    public static class SettingsWindowLogic
    {
        private delegate void ViewModelVerifyAPICallback(bool valid); //Callback delegate for the viewmodel
        private static ViewModelVerifyAPICallback verifyAPICallbackHandler; //Handler for the viewmodel callback

        public static Logger logger = LogManager.GetLogger("SettingsWindowLogic");

        public static bool VerifyAPIKey(string Key)
        {
            logger.Log(LogLevel.Debug, "Attempting To Verify API Key");
            HelpScoutClient client = new HelpScoutClient(Key);
            try
            {
                Paged<Mailbox> testPull = client.ListMailboxes();
            }
            catch (HelpScoutApiException exception)
            {
                HelpScoutLogHelpers.LogHelpScoutException(exception, "SettingsWindowLogic");
                if (exception.Code == 401 || exception.Code == 402)
                    return false;
            }
            logger.Log(LogLevel.Debug, "Sucessfully Verified API Key");
            return true;
        }

        public static void SaveSettings(SettingsViewModel viewModel)
        {
            ApplicationData.ApplicationSettings.APIKey = viewModel.APIKey;
            ApplicationData.ApplicationSettings.ValidAPIKeyExists = viewModel.ValidKey;
            ApplicationData.ApplicationSettings.SaveAPIKey = viewModel.SaveAPIKey;
            ApplicationData.ApplicationSettings.APIThrottleCount = viewModel.APICallLimit;
            HelpScoutRequestManager.UpdatedAPIKey(viewModel.APIKey);
        }

        public static void LoadSettings(SettingsViewModel viewModel)
        {
            viewModel.APIKey = ApplicationData.ApplicationSettings.APIKey;
            viewModel.SaveAPIKey = ApplicationData.ApplicationSettings.SaveAPIKey;
            viewModel.APICallLimit = ApplicationData.ApplicationSettings.APIThrottleCount;
        }
    }
}
