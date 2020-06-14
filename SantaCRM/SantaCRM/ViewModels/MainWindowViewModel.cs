using ClosedXML.Excel;
using Microsoft.Win32;
using SantaCRM.Infrastructure.Commands;
using SantaCRM.Models;
using SantaCRM.ViewModels.Base;
using SantaCRM.Views.Windows;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace SantaCRM.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        static private AddPersonWindow PersonWindow;
        static private AddLeadWindow LeadWindow;
        static private SettingsWindow settingsWindow;

        #region Видимость кнопки открытия меню
        private string _OpenMenuButton = "Visible";
        public string OpenMenuButton
        {
            get => _OpenMenuButton;
            set
            {
                _OpenMenuButton = value;
                OnPropertyChanged("OpenMenuButton");
            }
        }
        #endregion

        #region Видимость кнопки закрытия меню
        private string _CloseMenuButton = "Collapsed";
        public string CloseMenuButton
        {
            get => _CloseMenuButton;
            set
            {
                _CloseMenuButton = value;
                OnPropertyChanged("CloseMenuButton");
            }
        }
        #endregion

        #region Выбранный пункт меню
        private string _SelectedMenuItem;
        public string SelectedMenuItem
        {
            get => _SelectedMenuItem;
            set
            {
                _SelectedMenuItem = value;
                OnPropertyChanged("SelectedMenuItem");
            }
        }
        #endregion

        #region Видимость таблицы с обращениями
        private string _LeadsVisible = "Collapsed";
        public string LeadsVisible
        {
            get => _LeadsVisible;
            set
            {
                _LeadsVisible = value;
                OnPropertyChanged("LeadsVisible");
            }
        }
        #endregion

        #region Видимость таблицы с заявками
        private string _ToursVisible = "Collapsed";
        public string ToursVisible
        {
            get => _ToursVisible;
            set
            {
                _ToursVisible = value;
                OnPropertyChanged("ToursVisible");
            }
        }
        #endregion

        #region Видимость таблицы с покупателями
        private string _PersonsVisible = "Collapsed";
        public string PersonsVisible
        {
            get => _PersonsVisible;
            set
            {
                _PersonsVisible = value;
                OnPropertyChanged("PersonsVisible");
            }
        }
        #endregion

        #region Таблица с обращениями

        private DataTable _Leads = null;
        public DataTable Leads
        {
            get => _Leads;
            set
            {
                _Leads = value;
                OnPropertyChanged("Leads");
            }
        }

        #endregion

        #region Таблица с заявками

        private DataTable _Tours = null;
        public DataTable Tours
        {
            get => _Tours;
            set
            {
                _Tours = value;
                OnPropertyChanged("Tours");
            }
        }

        #endregion

        #region Таблица с покупателями

        private DataTable _Persons = null;
        public DataTable Persons
        {
            get => _Persons;
            set
            {
                _Persons = value;
                OnPropertyChanged("Persons");
            }
        }

        #endregion

        #region Выбранный DataGridItem

        public DataRowView SelectedRow { get; set; }

        private string _SelectedId;
        public string SelectedId
        {
            get
            {
                return _SelectedId;
            }
            set
            {
                _SelectedId = value;
                OnPropertyChanged("SelectedId");
            }
        }

        private string GetValueFromSelectedRow(string field)
        {
            if (SelectedRow != null)
            {
                return SelectedRow.Row[field].ToString();
            }
            else
            {
                return "NULL";
            }
        }


        #endregion

        #region Поиск в таблице с покупателями

        private string _PersonsString = null;
        public string PersonsString
        {
            get => _PersonsString;
            set
            {
                _PersonsString = value;
                OnPropertyChanged();
                Persons = DbHelper.Persons(PersonsString);
            }
        }

        #endregion

        #region Поиск в таблице с обращениями

        private string _LeadString = null;
        public string LeadString
        {
            get => _LeadString;
            set
            {
                _LeadString = value;
                OnPropertyChanged();
                Leads = DbHelper.Leads(LeadString);
            }
        }

        #endregion

        #region Команды

        #region OpenMenuCommand
        public ICommand OpenMenuCommand { get; }

        private bool CanOpenMenuCommandExecuted(object p) => true;

        private void OnOpenMenuCommandExecuted(object p)
        {
            OpenMenuButton = "Collapsed";
            CloseMenuButton = "Visible";
        }

        #endregion

        #region CloseMenuCommand
        public ICommand CloseMenuCommand { get; }

        private bool CanCloseMenuCommandExecuted(object p) => true;

        private void OnCloseMenuCommandExecuted(object p)
        {
            OpenMenuButton = "Visible";
            CloseMenuButton = "Collapsed";
        }

        #endregion

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecuted(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region OpenPersonCommand
        public ICommand OpenPersonCommand { get; }

        private bool CanOpenPersonCommandExecuted(object p) => true;

        private void OnOpenPersonCommandExecuted(object p)
        {
            SelectedId = GetValueFromSelectedRow("ID");
            Application.Current.MainWindow.IsEnabled = false;
            PersonWindow = new AddPersonWindow(SelectedId);
            PersonWindow.Show();
        }

        #endregion

        #region OpenLeadCommand
        public ICommand OpenLeadCommand { get; }

        private bool CanOpenLeadCommandExecuted(object p) => true;

        private void OnOpenLeadCommandExecuted(object p)
        {
            SelectedId = GetValueFromSelectedRow("#");
            Application.Current.MainWindow.IsEnabled = false;
            LeadWindow = new AddLeadWindow(SelectedId);
            LeadWindow.Show();
        }

        #endregion

        #region CreatePersonCommand
        public ICommand CreatePersonCommand { get; }

        private bool CanCreatePersonCommandExecuted(object p) => true;

        private void OnCreatePersonCommandExecuted(object p)
        {
            Application.Current.MainWindow.IsEnabled = false;
            PersonWindow = new AddPersonWindow("0");
            PersonWindow.Show();
        }

        #endregion

        #region UpdatePersonCommand
        public ICommand UpdatePersonCommand { get; }

        private bool CanUpdatePersonCommandExecuted(object p) => true;

        private void OnUpdatePersonCommandExecuted(object p)
        {
            GetPersons();
        }

        #endregion

        #region ChangeMenuItemCommand
        public ICommand ChangeMenuItemCommand { get; }

        private bool CanChangeMenuItemCommandExecuted(object p) => true;

        private void OnChangeMenuItemCommandExecuted(object p)
        {
            LeadsVisible = "Collapsed";
            ToursVisible = "Collapsed";
            PersonsVisible = "Collapsed";
            switch (SelectedMenuItem)
            {
                case "0":
                    {

                        break;
                    }
                case "1":
                    {
                        LeadsVisible = "Visible";
                        GetLeads();
                        break;
                    }
                case "2":
                    {
                        ToursVisible = "Visible";
                        GetTours();
                        break;
                    }
                case "3":
                    {
                        PersonsVisible = "Visible";
                        GetPersons();
                        break;
                    }
                case "4":
                    {

                        break;
                    }
                default:
                    break;
            }
        }

        #endregion

        #region CreateLeadCommand
        public ICommand CreateLeadCommand { get; }

        private bool CanCreateLeadCommandExecuted(object p) => true;

        private void OnCreateLeadCommandExecuted(object p)
        {
            Application.Current.MainWindow.IsEnabled = false;
            LeadWindow = new AddLeadWindow("0");
            LeadWindow.Show();
        }

        #endregion

        #region UpdateLeadsCommand
        public ICommand UpdateLeadsCommand { get; }

        private bool CanUpdateLeadsCommandExecuted(object p) => true;

        private void OnUpdateLeadsCommandExecuted(object p)
        {
            GetLeads();
        }

        #endregion

        #region OpenSettingsCommand
        public ICommand OpenSettingsCommand { get; }

        private bool CanOpenSettingsCommandExecuted(object p) => true;

        private void OnOpenSettingsCommandExecuted(object p)
        {
            Application.Current.MainWindow.IsEnabled = false;
            settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        #endregion

        #region ExportCommand
        public ICommand ExportCommand { get; }

        private bool CanExportCommandExecuted(object p) => true;

        private void OnExportCommandExecuted(object p)
        {
            DataTable Export = DbHelper.Persons(PersonsString);
            XLWorkbook wb = new XLWorkbook();
            wb.Worksheets.Add(Persons, "Покупатели");
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            sfd.FilterIndex = 2;
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == true)
            {
                wb.SaveAs(sfd.FileName + ".xlsx");
            }
        }

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            #region Команды

            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecuted);
            ChangeMenuItemCommand = new LambdaCommand(OnChangeMenuItemCommandExecuted, CanChangeMenuItemCommandExecuted);
            OpenMenuCommand = new LambdaCommand(OnOpenMenuCommandExecuted, CanOpenMenuCommandExecuted);
            CloseMenuCommand = new LambdaCommand(OnCloseMenuCommandExecuted, CanCloseMenuCommandExecuted);
            OpenPersonCommand = new LambdaCommand(OnOpenPersonCommandExecuted, CanOpenPersonCommandExecuted);
            OpenLeadCommand = new LambdaCommand(OnOpenLeadCommandExecuted, CanOpenLeadCommandExecuted);
            CreatePersonCommand = new LambdaCommand(OnCreatePersonCommandExecuted, CanCreatePersonCommandExecuted);
            CreateLeadCommand = new LambdaCommand(OnCreateLeadCommandExecuted, CanCreateLeadCommandExecuted);
            UpdatePersonCommand = new LambdaCommand(OnUpdatePersonCommandExecuted, CanUpdatePersonCommandExecuted);
            UpdateLeadsCommand = new LambdaCommand(OnUpdateLeadsCommandExecuted, CanUpdateLeadsCommandExecuted);
            OpenSettingsCommand = new LambdaCommand(OnOpenSettingsCommandExecuted, CanOpenSettingsCommandExecuted);
            ExportCommand = new LambdaCommand(OnExportCommandExecuted, CanExportCommandExecuted);

            #endregion
        }

        public void GetLeads()
        {
            Leads = DbHelper.Leads(LeadString);
        }

        public void GetTours()
        {
            Tours = DbHelper.Tours();
        }

        public void GetPersons()
        {
            Persons = DbHelper.Persons(PersonsString);
        }

        static public void ClosePerson()
        {
            PersonWindow.Close();
            Application.Current.MainWindow.IsEnabled = true;
        }

        static public void CloseLead()
        {
            LeadWindow.Close();
            Application.Current.MainWindow.IsEnabled = true;
        }

        static public void CloseSettings()
        {
            settingsWindow.Close();
            Application.Current.MainWindow.IsEnabled = true;
        }
    }
}