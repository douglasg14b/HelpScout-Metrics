using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HelpScoutMetrics.NLogViewer
{
    /// <summary>
    /// Interaction logic for NLogViewerView.xaml
    /// </summary>
    public partial class NLogViewerView : Window
    {
        public NLogViewerView()
        {
            InitializeComponent();
        }

        private static Logger logger = LogManager.GetLogger("NLogViewerView");

        protected override void OnClosed(EventArgs e)
        {
            logger.Log(LogLevel.Info, "Closed Log Window");
            base.OnClosed(e);
        }
    }
}
