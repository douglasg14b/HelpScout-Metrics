using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using HelpScoutMetrics;
using HelpScoutMetrics.ViewModel;
using HelpScoutMetrics.Scripts.Model;
using HelpScoutMetrics.Model;
using HelpScoutMetrics.Model.DataTypes;
using HelpScoutNet.Model.Report.User.UserReports;
using HelpScoutNet.Request.Report.User;
using HelpScoutNet.Model;
using HelpScoutMetrics.Model.SaveAndLoad;
using System.Threading.Tasks;
using NLog;

namespace HelpScoutMetrics
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Logger logger = LogManager.GetLogger("App");
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            LoadData.LoadUserData();
            base.OnStartup(e);

            HelpScoutMetrics.MainWindow mainWindow = new MainWindow();
            MainScreenViewModel viewModel = mainWindow.MainScreenView.DataContext as MainScreenViewModel;
            viewModel.Window = mainWindow;
            viewModel.SetUpColumns();
            mainWindow.Show();

            //Task.Run(new Action(() => AddLoggerEntries(5000)));


        }

        private void AddLoggerEntries(int count)
        {
            for(int i = 0; i< count; i++)
            {
                logger.Log(LogLevel.Debug, "Test Debug Log # " + i.ToString());
                logger.Log(LogLevel.Info, "Test Info Log # " + i.ToString());

                System.Threading.Thread.Sleep(2);
            }
        }

        private void TestCallBack(object sender, BaseApiRequest<Paged<HelpScoutNet.Model.User>>.ResultReadyEventArgs<Paged<HelpScoutNet.Model.User>> e)
        {
           
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SaveDataType dataToSave = new SaveDataType(ApplicationData.ApplicationSettings, ApplicationData.ApplicationColumns, ApplicationData.Users, ApplicationData.UserRatingsList);
            SaveData.SaveAllData(dataToSave);
            base.OnExit(e);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // put your tracing or logging code here (I put a message box as an example)
            MessageBox.Show(e.ExceptionObject.ToString());
            SaveDataType dataToSave = new SaveDataType(ApplicationData.ApplicationSettings, ApplicationData.ApplicationColumns, ApplicationData.Users, ApplicationData.UserRatingsList);
            SaveData.SaveAllData(dataToSave);
        }


    }
}
