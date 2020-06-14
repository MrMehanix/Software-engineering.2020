using SantaCRM.Infrastructure.Commands;
using SantaCRM.Models;
using SantaCRM.ViewModels.Base;
using System.Windows;
using System.Windows.Input;

namespace SantaCRM.ViewModels
{
    internal class LoginWindowViewModel : ViewModel
    {
        private MainWindow mainWindow;
        public LoginWindowViewModel()
        {
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecuted);
            AuthorizationCommand = new LambdaCommand(OnAuthorizationCommandExecuted, CanAuthorizationCommandExecuted);
        }

        #region Свойства

        #region Содержимое логина

        private string _LoginContent;
        public string LoginContent
        {
            get => _LoginContent;
            set
            {
                _LoginContent = value;
                OnPropertyChanged("LoginContent");
            }
        }

        #endregion

        #region Содержимое пароля

        private string _PasswordContent;
        public string PasswordContent
        {
            get => _PasswordContent;
            set
            {
                _PasswordContent = value;
                OnPropertyChanged("PasswordContent");
            }
        }

        #endregion

        #region Содержимое ошибки

        private string _ErrorContent;
        public string ErrorContent
        {
            get => _ErrorContent;
            set
            {
                _ErrorContent = value;
                OnPropertyChanged("ErrorContent");
            }
        }

        #endregion

        #endregion

        #region Команды

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecuted(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region AuthorizationCommand
        public ICommand AuthorizationCommand { get; }

        private bool CanAuthorizationCommandExecuted(object p) => true;

        private void OnAuthorizationCommandExecuted(object p)
        {
            switch (DbHelper.EstablishConnection(LoginContent, PasswordContent))
            {
                case 0:
                    {
                        mainWindow = new MainWindow();
                        mainWindow.Show();
                        Application.Current.MainWindow.Close();
                        Application.Current.MainWindow = mainWindow;
                        break;
                    }
                case 1:
                    {
                        ErrorContent = "Введите логин и пароль";
                        break;
                    }
                case 2:
                    {
                        ErrorContent = "Неправильный логин и/или пароль";
                        break;
                    }
                case 100:
                    {
                        ErrorContent = "Ну это что-то новенькое...";
                        break;
                    }
                default:
                    break;
            }

            #endregion

            #endregion
        }
    }
}