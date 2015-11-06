using HelpScoutMetrics.Scripts.Model;
using HelpScoutMetrics.ViewModel;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HelpScoutMetrics.Model.DataTypes
{
    [Serializable]
    public class ColumnObject : INotifyPropertyChanged
    {

        public ColumnObject(string name, bool enabled, string bindingName, int index, DataGrid datagridOwner, MainScreenViewModel viewModel)
        {
            initializing = true;

            Name = name;
            BindingName = bindingName;
            DataGridOwner = datagridOwner;
            Index = index;

            Enabled = enabled;

            initializing = false;
        }

        public ColumnObject(ColumnListItem columnItem, DataGrid datagridOwner, MainScreenViewModel viewModel)
        {
            initializing = true;

            Name = columnItem.Name;
            DisplayIndexBindingName = columnItem.DisplayIndexBindingName;
            BindingName = columnItem.DataBindingName;
            DataGridOwner = datagridOwner;
            Index = columnItem.Index;

            Enabled = columnItem.Enabled;
            this.viewModel = viewModel;

            initializing = false;
        }

        private static Logger logger = LogManager.GetLogger("ColumnObject Logger");

        private MainScreenViewModel viewModel;
        private bool initializing; //Is true while the class is initializing. Used to disable verbose logging when class is initializing


        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; RaisePropertyChanged("Name"); }
        }

        private int m_Index;
        public int Index
        {
            get { return m_Index; }
            set 
            {
                ColumnListItem columnListItem = FindColumnListItem();
                if(columnListItem != null)
                {
                    columnListItem.Index = value;
                    if (!initializing) //Avoids verbose debug logging 
                        logger.Log(LogLevel.Debug, "Sucessfully changed index of column " + Name + " from " + Index + " to " + value);
                }
                m_Index = value;
                RaisePropertyChanged("Index"); 
            }
        }

        [NonSerialized]
        private bool m_Enabled;
        public bool Enabled
        {
            get { return m_Enabled; }
            set 
            {
                //Sets the column enabled/disabled status in application data
                ColumnListItem columnListItem = FindColumnListItem();
                if (columnListItem != null)
                {
                    columnListItem.Enabled = value;
                    if (!initializing) //Avoids verbose debug logging 
                    {
                        if(value)
                        {
                            logger.Log(LogLevel.Debug, "Sucessfully Enabled column " + Name);
                        }
                        else
                        {
                            logger.Log(LogLevel.Debug, "Sucessfully Disabled column " + Name);
                        }
                    }
                }
                m_Enabled = value; 
                //Sets the visibility status of the Column
                if(value)
                {
                    ColumnVisibility = Visibility.Visible;
                    if (!CreatedColumn)
                        AddColumn();
                }
                else
                {
                    ColumnVisibility = Visibility.Hidden;
                }
                RaisePropertyChanged("Enabled"); 
            }
        }

        [NonSerialized]
        private Visibility m_ColumnVisibility;
        public Visibility ColumnVisibility
        {
            get { return m_ColumnVisibility; }
            set 
            { 
                m_ColumnVisibility = value;
                if (TextColumn != null)
                {
                    TextColumn.Visibility = value;
                }
                RaisePropertyChanged("ColumnVisibility"); 
            }
        }

        private string m_BindingName;
        public string BindingName
        {
            get { return m_BindingName; }
            set { m_BindingName = value; RaisePropertyChanged("BindingName"); }
        }

        private string m_DisplayIndexBindingName;
        public string DisplayIndexBindingName
        {
            get { return m_DisplayIndexBindingName; }
            set { m_DisplayIndexBindingName = value; RaisePropertyChanged("DisplayIndexBindingName"); }
        }

        [NonSerialized]
        private DataGrid m_DataGridOwner;
        public DataGrid DataGridOwner
        {
            get { return m_DataGridOwner; }
            set { m_DataGridOwner = value; RaisePropertyChanged("DataGridOwner"); }
        }

        [NonSerialized]
        private DataGridTextColumn m_TextColumn;
        public DataGridTextColumn TextColumn
        {
            get { return m_TextColumn; }
            set { m_TextColumn = value; RaisePropertyChanged("TextColumn"); }
        }

        [NonSerialized]
        private bool CreatedColumn = false;

        private void AddColumn()
        {
            DataGridTextColumn textColumn = new DataGridTextColumn();
            textColumn.Binding = new Binding(BindingName);

            //textColumn.DisplayIndex = Index;
            BindingOperations.SetBinding(textColumn, DataGridColumn.DisplayIndexProperty, new Binding("Index") { Source = this, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });

            textColumn.Header = Name;
            textColumn.MinWidth = 20;
            TextColumn = textColumn;
            CreatedColumn = true;
            DataGridOwner.Columns.Add(textColumn); // Datagrid owner is the DataGrid


        }

        //Finds the column list item in application data
        private ColumnListItem FindColumnListItem()
        {
            try
            {
                ColumnListItem columnListItem = ApplicationData.ApplicationColumns.QuickStatColumns.Single(i => i.Name == Name);
                return columnListItem;
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "Input Sequence Contained Zero, Or More Than One Item. Unable to save index for column: " + Name, exception);
                return null;
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler temp = PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
