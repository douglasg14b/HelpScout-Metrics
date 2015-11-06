using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpScoutMetrics.Model.WindowLogic;
using System.Windows.Media;
using NLog;
using HelpScoutMetrics.Scripts.Model;

namespace HelpScoutMetrics.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            SettingsWindowLogic.LoadSettings(this);
        }

        private static Logger logger = LogManager.GetLogger("SettingsViewModel");

        public MainWindow MainWindow { get; set; }

        private bool loadingSettings = false;
        private bool resettingSettings = false;

        private string m_APIKey;
        public string APIKey
        {
            get { return m_APIKey; }
            set 
            { 
                if(!loadingSettings) //If settings are not loading, and key is edited, then invalidate
                {
                    KeyValidationButtonText = "Verify Key";
                    VerifyButtonBackground = new SolidColorBrush(Color.FromArgb(255, 213, 213, 213));
                }
                else if (ApplicationData.ApplicationSettings.ValidAPIKeyExists) //If a valid key exists while loading, then change the valid key text to show valid
                {
                    ValidKey = ApplicationData.ApplicationSettings.ValidAPIKeyExists;
                }
                else
                {
                    KeyValidationButtonText = "Verify Key";
                    VerifyButtonBackground = new SolidColorBrush(Color.FromArgb(255, 213, 213, 213));
                }
                m_APIKey = value; 
                TriedToValidateKey = false; 
                RaisePropertyChanged("APIKey"); 
            }
        }

        private bool m_SaveAPIKey = true;
        public bool SaveAPIKey
        {
            get { return m_SaveAPIKey; }
            set { m_SaveAPIKey = value; RaisePropertyChanged("SaveAPIKey"); }
        }

        private int m_APICallLimit = 50;
        public int APICallLimit
        {
            get { return m_APICallLimit; }
            set { m_APICallLimit = value; RaisePropertyChanged("APICallLimit"); }
        }

        /*====================================================================
         *             Key Validation & Button
         * ==================================================================*/

        private bool m_ValidKey;
        public bool ValidKey
        {
            get { return m_ValidKey; }
            set 
            { 
                m_ValidKey = value;
                if(value)
                {
                    KeyValidationButtonText = "Valid!";
                    VerifyButtonBackground = new SolidColorBrush(Color.FromArgb(255, 18, 145, 47));
                    //ApplicationData.ApplicationSettings.ValidAPIKeyExists = true;
                }
                else
                {
                    if(!resettingSettings)
                    {
                        KeyValidationButtonText = "Invalid";
                        VerifyButtonBackground = new SolidColorBrush(Color.FromArgb(255, 153, 18, 18));
                    }
                    else
                    {
                        KeyValidationButtonText = "Verify Key";
                        VerifyButtonBackground = new SolidColorBrush(Color.FromArgb(255, 213, 213, 213));
                    }

                    //ApplicationData.ApplicationSettings.ValidAPIKeyExists = false;
                }
                RaisePropertyChanged("ValidKey"); 
            }
        }

        //Will be true when the eky is in process of verification
        private bool m_CurrentlyVerifyingKey;
        public bool CurrentlyVerifyingKey
        {
            get { return m_CurrentlyVerifyingKey; }
            set 
            {
                if (value)
                {
                    KeyValidationButtonText = "Verifying...";
                }
                m_CurrentlyVerifyingKey = value; 
                RaisePropertyChanged("CurrentlyVerifyingKey");
            }
        }

        private bool m_TriedToValidateKey;
        public bool TriedToValidateKey
        {
            get { return m_TriedToValidateKey; }
            set { m_TriedToValidateKey = value; RaisePropertyChanged("TriedToValidateKey"); }
        }

        private string m_KeyValidationButtonText = "Verify Key";
        public string KeyValidationButtonText
        {
            get { return m_KeyValidationButtonText; }
            set { m_KeyValidationButtonText = value; RaisePropertyChanged("KeyValidationButtonText"); }
        }

        private Brush m_VerifyButtonBackground = new SolidColorBrush(Color.FromArgb(255, 213, 213, 213));
        public Brush VerifyButtonBackground
        {
            get { return m_VerifyButtonBackground; }
            set { m_VerifyButtonBackground = value; RaisePropertyChanged("VerifyButtonBackground"); }
        }

        public async void VerifyAPIKey()
        {
            Task<bool> results = new Task<bool>(() => SettingsWindowLogic.VerifyAPIKey(APIKey));
            CurrentlyVerifyingKey = true;

            results.Start();

            CurrentlyVerifyingKey = false;

            if (await results)
            {
                ValidKey = true;
            }
            else
            {
                ValidKey = false;
            }
            TriedToValidateKey = true;
        }




        public void SaveSettings()
        {
            SettingsWindowLogic.SaveSettings(this);
            logger.Log(LogLevel.Debug, "Saved Settings");
            CloseFlyout();
        }

        public void LoadSettings()
        {
            loadingSettings = true;

            SettingsWindowLogic.LoadSettings(this);

            loadingSettings = false;
        }
        public void ResetSettings()
        {
            resettingSettings = true;

            APIKey = string.Empty;
            SaveAPIKey = false;
            ValidKey = false;
            APICallLimit = 50;
            //ApplicationData.ApplicationSettings.ValidAPIKeyExists = false;
            logger.Log(LogLevel.Debug, "Reset Settings");

            resettingSettings = false;
        }

        public void CloseFlyout()
        {
            MainWindow.SettingsFlyout.IsOpen = false;
            logger.Log(LogLevel.Info, "Closed Settings Flyout");
        }
    }
}
