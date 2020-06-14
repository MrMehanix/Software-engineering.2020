using SantaCRM.Infrastructure.Commands;
using SantaCRM.Models;
using SantaCRM.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace SantaCRM.ViewModels
{
    internal class AddPersonWindowViewModel : ViewModel
    {
        public AddPersonWindowViewModel(string PersonId)
        {
            Id = PersonId;
            AllMarks = DbHelper.AllMarks();
            if (Id != "0")
            {
                Person = DbHelper.CurrentPerson(PersonId);
                Row = Person.Rows[0];
                Surname = GetValueFromSelectedPerson("Фамилия");
                Name = GetValueFromSelectedPerson("Имя");
                FatherName = GetValueFromSelectedPerson("Отчество");
                Phone1 = GetValueFromSelectedPerson("Номер телефона 1");
                Phone2 = GetValueFromSelectedPerson("Номер телефона 2");
                Phone3 = GetValueFromSelectedPerson("Номер телефона 3");
                Mail = GetValueFromSelectedPerson("E-mail");
                Birthday = GetValueFromSelectedPerson("День рождения");
                if (Birthday != null)
                {
                    DateTime birth = DateTime.ParseExact(Birthday, "dd.MM.yyyy h:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    Birthday = birth.Month.ToString() + "/" + birth.Day.ToString() + "/" + birth.Year.ToString() + " " + birth.Hour.ToString("00") + ":" + birth.Minute.ToString("00") + ":" + birth.Second.ToString("00") + " AM";
                }
                SelectedItems = DbHelper.SelectedMarks(Id);
            }
            else
            {
                SelectedItems = new ObservableCollection<string>();
            }

            #region Команды

            CloseWindowCommand = new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecuted);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecuted);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecuted);

            #endregion
        }

        public ObservableCollection<string> AllMarks { get; set; } = new ObservableCollection<string>();

        public DataTable Person;

        public DataRow Row;

        #region ID

        private string _Id;
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Фамилия

        private string _Surname;
        public string Surname
        {
            get
            {
                return _Surname;
            }
            set
            {
                _Surname = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Имя

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

        #region Отчество

        private string _FatherName;
        public string FatherName
        {
            get
            {
                return _FatherName;
            }
            set
            {
                _FatherName = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Телефон 1

        private string _Phone1;
        public string Phone1
        {
            get
            {
                return _Phone1;
            }
            set
            {
                _Phone1 = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Телефон 2

        private string _Phone2;
        public string Phone2
        {
            get
            {
                return _Phone2;
            }
            set
            {
                _Phone2 = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Телефон 3

        private string _Phone3;
        public string Phone3
        {
            get
            {
                return _Phone3;
            }
            set
            {
                _Phone3 = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Почта

        private string _Mail;
        public string Mail
        {
            get
            {
                return _Mail;
            }
            set
            {
                _Mail = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region День рождения

        private string _Birthday;
        public string Birthday
        {
            get
            {
                return _Birthday;
            }
            set
            {
                _Birthday = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Выбранная метка

        private ObservableCollection<string> _SelectedItems;
        public ObservableCollection<string> SelectedItems
        {
            get
            {
                return _SelectedItems;
            }
            set
            {
                _SelectedItems = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private string GetValueFromSelectedPerson(string field)
        {
            if (Row != null)
            {
                //if (Row.Field<string>(field) is DateTime)
                if (Row.Field<string>(field) != null)
                    return Row.Field<string>(field);
                else return null;
            }
            else
            {
                return "NULL";
            }
        }

        #region CloseWindowCommand
        public ICommand CloseWindowCommand { get; }

        private bool CanCloseWindowCommandExecuted(object p) => true;

        private void OnCloseWindowCommandExecuted(object p)
        {
            MainWindowViewModel.ClosePerson();
        }

        #endregion

        #region SaveCommand
        public ICommand SaveCommand { get; }

        private bool CanSaveCommandExecuted(object p) => true;

        private void OnSaveCommandExecuted(object p)
        {
            MainWindowViewModel.ClosePerson();
            if (Id != "0")
                DbHelper.UpdatePerson(Surname, Name, FatherName, Phone1, Phone2, Phone3, Mail, Birthday, Id);
            else
                DbHelper.CreatePerson(Surname, Name, FatherName, Phone1, Phone2, Phone3, Mail, Birthday);
            DbHelper.SaveMarks(Id, SelectedItems);
        }

        #endregion

        #region DeleteCommand
        public ICommand DeleteCommand { get; }

        private bool CanDeleteCommandExecuted(object p) => true;

        private void OnDeleteCommandExecuted(object p)
        {
            MainWindowViewModel.ClosePerson();
            try
            {
                DbHelper.DeletePerson(Id);
            }
            catch (Exception)
            {
                MessageBox.Show("Вы не можете удалить этого покупателя, потому что за ним закреплены обращения!");
            }
        }

        #endregion
    }
}
