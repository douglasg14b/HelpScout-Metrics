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
using System.Linq;
using HelpScoutMetrics.ViewModel;
using System.Collections.ObjectModel;

namespace HelpScoutMetrics.Views
{
    /// <summary>
    /// Interaction logic for MainScreenView.xaml
    /// </summary>
    public partial class MainScreenView : UserControl
    {
        public MainScreenView()
        {
            InitializeComponent();
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            IList<DateTime> myList = e.AddedItems.Cast<DateTime>().ToList();

            MainScreenViewModel viewModel = DataContext as MainScreenViewModel;
            viewModel.SelectedDates = new ObservableCollection<DateTime>(myList);
        }
    }
}
