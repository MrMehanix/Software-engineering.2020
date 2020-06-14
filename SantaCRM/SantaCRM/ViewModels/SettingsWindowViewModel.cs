using SantaCRM.Infrastructure.Commands;
using SantaCRM.Models;
using SantaCRM.ViewModels.Base;
using SantaCRM.Views.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SantaCRM.ViewModels
{
    internal class SettingsWindowViewModel : ViewModel
    {
        static private InputWindow inputWindow;
        public SettingsWindowViewModel()
        {
            SelectedSetting = 0;

            ExitCommand = new LambdaCommand(OnExitCommandExecuted, CanExitCommandExecuted);
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecuted);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecuted);
        }

        #region Выбранный пункт меню

        private int _SelectedSetting;
        public int SelectedSetting
        {
            get
            {
                return _SelectedSetting;
            }
            set
            {
                _SelectedSetting = value;
                OnPropertyChanged();
                OpenSetting(SelectedSetting);
            }
        }

        #endregion

        #region Выбранный пункт меню

        private string _SelectedItem;
        public string SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Все позиции в словаре

        private ObservableCollection<string> _AllItems;
        public ObservableCollection<string> AllItems
        {
            get
            {
                return _AllItems;
            }
            set
            {
                _AllItems = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Кнопка выйти
        public ICommand ExitCommand { get; }

        private bool CanExitCommandExecuted(object p) => true;

        private void OnExitCommandExecuted(object p)
        {
            MainWindowViewModel.CloseSettings();
        }

        #endregion

        #region Кнопка добавить
        public ICommand AddCommand { get; }

        private bool CanAddCommandExecuted(object p) => true;

        private void OnAddCommandExecuted(object p)
        {
            inputWindow = new InputWindow(SelectedSetting);
            inputWindow.Show();
        }

        #endregion

        #region Кнопка удалить
        public ICommand DeleteCommand { get; }

        private bool CanDeleteCommandExecuted(object p)
        {
            if (SelectedItem == null)
                return false;
            else
                return true;
        }

        private void OnDeleteCommandExecuted(object p)
        {
            switch (SelectedSetting)
            {
                case 0:
                    {
                        DbHelper.DeleteMark(SelectedItem);
                        break;
                    }
                case 1:
                    {
                        DbHelper.DeleteCountry(SelectedItem);
                        break;
                    }
                case 2:
                    {
                        DbHelper.DeleteFood(SelectedItem);
                        break;
                    }
                case 3:
                    {
                        DbHelper.DeletePlace(SelectedItem);
                        break;
                    }
                case 4:
                    {
                        DbHelper.DeleteStatus(SelectedItem);
                        break;
                    }
                case 5:
                    {
                        DbHelper.DeleteSource(SelectedItem);
                        break;
                    }
                case 6:
                    {
                        DbHelper.DeleteType(SelectedItem);
                        break;
                    }
                default:
                    break;
            }
            OpenSetting(SelectedSetting);
        }

        #endregion

        public void OpenSetting(int Index)
        {
            switch (Index)
            {
                case 0:
                    {
                        AllItems = DbHelper.AllMarks();
                        break;
                    }
                case 1:
                    {
                        AllItems = DbHelper.AllCountries();
                        break;
                    }
                case 2:
                    {
                        AllItems = DbHelper.AllFood();
                        break;
                    }
                case 3:
                    {
                        AllItems = DbHelper.AllPlaces();
                        break;
                    }
                case 4:
                    {
                        AllItems = DbHelper.AllStatus();
                        break;
                    }
                case 5:
                    {
                        AllItems = DbHelper.AllSources();
                        break;
                    }
                case 6:
                    {
                        AllItems = DbHelper.AllTypes();
                        break;
                    }
                default:
                    break;
            }
        }

        static public void CloseInput()
        {
            inputWindow.Close();
        }
    }
}
