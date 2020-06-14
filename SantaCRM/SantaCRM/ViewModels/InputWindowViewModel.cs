using SantaCRM.Infrastructure.Commands;
using SantaCRM.Models;
using SantaCRM.ViewModels.Base;
using System.Windows.Input;

namespace SantaCRM.ViewModels
{
    internal class InputWindowViewModel : ViewModel
    {
        public InputWindowViewModel(int Select)
        {
            SelectedItem = Select;

            ExitCommand = new LambdaCommand(OnExitCommandExecuted, CanExitCommandExecuted);
            OkCommand = new LambdaCommand(OnOkCommandExecuted, CanOkCommandExecuted);
        }

        public int SelectedItem;

        #region Введенная строка

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Кнопка выйти
        public ICommand ExitCommand { get; }

        private bool CanExitCommandExecuted(object p) => true;

        private void OnExitCommandExecuted(object p)
        {
            SettingsWindowViewModel.CloseInput();
        }

        #endregion

        #region Кнопка подтверждения
        public ICommand OkCommand { get; }

        private bool CanOkCommandExecuted(object p) => true;

        private void OnOkCommandExecuted(object p)
        {
            switch (SelectedItem)
            {
                case 0:
                    {
                        DbHelper.CreateMark(Name);
                        break;
                    }
                case 1:
                    {
                        DbHelper.CreateCountry(Name);
                        break;
                    }
                case 2:
                    {
                        DbHelper.CreateFood(Name);
                        break;
                    }
                case 3:
                    {
                        DbHelper.CreatePlace(Name);
                        break;
                    }
                case 4:
                    {
                        DbHelper.CreateStatus(Name);
                        break;
                    }
                case 5:
                    {
                        DbHelper.CreateSource(Name);
                        break;
                    }
                case 6:
                    {
                        DbHelper.CreateType(Name);
                        break;
                    }
                default:
                    break;
            }
            SettingsWindowViewModel.CloseInput();
        }

        #endregion
    }
}
