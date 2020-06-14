using SantaCRM.Infrastructure.Commands;
using SantaCRM.Models;
using SantaCRM.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace SantaCRM.ViewModels
{
    internal class AddLeadWindowViewModel : ViewModel
    {
        public AddLeadWindowViewModel(string leadId)
        {
            LeadId = leadId;
            AllMarks = DbHelper.AllMarks();
            AllCountries = DbHelper.AllCountries();
            AllFood = DbHelper.AllFood();
            AllPlaces = DbHelper.AllPlaces();
            AllStatus = DbHelper.AllStatus();
            AllManagers = DbHelper.AllManagers();
            AllSources = DbHelper.AllSources();
            AllTypes = DbHelper.AllTypes();
            if (LeadId == "0")
            {
                PersonId = "0";
                SelectedCountries = new ObservableCollection<string>();
                SelectedFood = new ObservableCollection<string>();
                SelectedPlaces = new ObservableCollection<string>();
                SelectedMarks = new ObservableCollection<string>();
                CreationDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            }
            else
            {
                Lead = DbHelper.CurrentLead(LeadId);
                LeadRow = Lead.Rows[0];
                PersonId = GetStringValue("ID покупателя", LeadRow);
                CreationDate = GetStringValue("Дата создания", LeadRow);
                if (CreationDate != null)
                {
                    DateTime date = DateTime.ParseExact(CreationDate, "dd.MM.yyyy H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    CreationDate = date.ToString("MM/dd/yyyy hh:mm:ss");
                    //CreationDate = date.Month.ToString() + "/" + date.Day.ToString() + "/" + date.Year.ToString() + " " + date.Hour.ToString() + ":" + date.Minute.ToString() + ":" + date.Second.ToString();
                }
                Note = GetStringValue("Примечание", LeadRow);
                DateShipment = GetStringValue("Желаемая дата", LeadRow);
                if (DateShipment != null)
                {
                    DateTime date = DateTime.ParseExact(DateShipment, "dd.MM.yyyy h:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    DateShipment = date.Month.ToString() + "/" + date.Day.ToString() + "/" + date.Year.ToString() + " " + date.Hour.ToString() + ":" + date.Minute.ToString() + ":" + date.Second.ToString() + " AM";
                }
                Nights = GetStringValue("Кол-во ночей", LeadRow);
                Adults = GetStringValue("Кол-во взрослых", LeadRow);
                Сhildren = GetStringValue("Кол-во детей", LeadRow);
                Budget = GetStringValue("Бюджет", LeadRow);
                SelectedManager = GetStringValue("Менеджер", LeadRow);
                SelectedStatus = GetStringValue("Статус", LeadRow);
                SelectedType = GetStringValue("Тип заявки", LeadRow);
                SelectedSource = GetStringValue("Источник обращения", LeadRow);
                SelectedCountries = DbHelper.SelectedCountries(LeadId);
                SelectedFood = DbHelper.SelectedFood(LeadId);
                SelectedPlaces = DbHelper.SelectedPlaces(LeadId);
                if (PersonId != "0")
                {
                    GetPerson();
                    IsPersonEdit = false;
                }
                else
                {
                    SelectedMarks = new ObservableCollection<string>();
                }
            }

            #region Команды

            CloseWindowCommand = new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecuted);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecuted);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecuted);

            #endregion
        }

        public ObservableCollection<string> AllMarks { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AllCountries { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AllFood { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AllPlaces { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AllStatus { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AllManagers { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AllSources { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AllTypes { get; set; } = new ObservableCollection<string>();

        public DataTable Lead;
        public DataRow LeadRow;

        public DataTable Person;
        public DataRow PersonRow;

        #region Текст поиска

        private string _Search;
        public string Search
        {
            get
            {
                return _Search;
            }
            set
            {
                _Search = value;
                OnPropertyChanged();
                if (Search != null)
                    SearchResult = DbHelper.FindPerson(Search);
            }
        }

        #endregion

        #region Результаты поиска

        private ObservableCollection<string> _SearchResult;
        public ObservableCollection<string> SearchResult
        {
            get
            {
                return _SearchResult;
            }
            set
            {
                _SearchResult = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Выбранный покупатель

        private string _SelectedPerson;
        public string SelectedPerson
        {
            get
            {
                return _SelectedPerson;
            }
            set
            {
                _SelectedPerson = value;
                OnPropertyChanged();
                PersonId = DbHelper.FindPersonId(SelectedPerson);
                string _id = PersonId;
                GetPerson();
                IsPersonEdit = false;
                PersonId = _id;
            }
        }

        #endregion

        #region Индикатор редактирования покупателя

        private bool _IsPersonEdit = true;
        public bool IsPersonEdit
        {
            get
            {
                return _IsPersonEdit;
            }
            set
            {
                _IsPersonEdit = value;
                OnPropertyChanged();
                if (IsPersonEdit == true)
                    PersonId = "0";
                PersonIndicator();
            }
        }

        private string _PersonHeader;
        public string PersonHeader
        {
            get
            {
                return _PersonHeader;
            }
            set
            {
                _PersonHeader = value;
                OnPropertyChanged();
            }
        }

        public void PersonIndicator()
        {
            if (IsPersonEdit == false)
                PersonHeader = "Покупатель";
            else
                PersonHeader = "Покупатель (новый)";
        }

        #endregion

        #region PersonId

        private string _PersonId;
        public string PersonId
        {
            get
            {
                return _PersonId;
            }
            set
            {
                _PersonId = value;
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
                IsPersonEdit = true;
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
                IsPersonEdit = true;
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
                IsPersonEdit = true;
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
                IsPersonEdit = true;
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
                IsPersonEdit = true;
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
                IsPersonEdit = true;
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
                IsPersonEdit = true;
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
                IsPersonEdit = true;
            }
        }

        #endregion

        #region Выбранные метки

        private ObservableCollection<string> _SelectedMarks;
        public ObservableCollection<string> SelectedMarks
        {
            get
            {
                return _SelectedMarks;
            }
            set
            {
                _SelectedMarks = value;
                OnPropertyChanged();
                IsPersonEdit = true;
            }
        }
        #endregion

        #region LeadId

        private string _LeadId;
        public string LeadId
        {
            get
            {
                return _LeadId;
            }
            set
            {
                _LeadId = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Дата создания

        private string _CreationDate;
        public string CreationDate
        {
            get
            {
                return _CreationDate;
            }
            set
            {
                _CreationDate = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Примечание

        private string _Note;
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                _Note = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Желаемая дата отправки

        private string _DateShipment;
        public string DateShipment
        {
            get
            {
                return _DateShipment;
            }
            set
            {
                _DateShipment = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Количество ночей

        private string _Nights;
        public string Nights
        {
            get
            {
                return _Nights;
            }
            set
            {
                _Nights = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Количество взрослых

        private string _Adults;
        public string Adults
        {
            get
            {
                return _Adults;
            }
            set
            {
                _Adults = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Количество детей

        private string _Сhildren;
        public string Сhildren
        {
            get
            {
                return _Сhildren;
            }
            set
            {
                _Сhildren = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Бюджет

        private string _Budget;
        public string Budget
        {
            get
            {
                return _Budget;
            }
            set
            {
                _Budget = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Выбранные страны

        private ObservableCollection<string> _SelectedCountries;
        public ObservableCollection<string> SelectedCountries
        {
            get
            {
                return _SelectedCountries;
            }
            set
            {
                _SelectedCountries = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Выбранные типы питания

        private ObservableCollection<string> _SelectedFood;
        public ObservableCollection<string> SelectedFood
        {
            get
            {
                return _SelectedFood;
            }
            set
            {
                _SelectedFood = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Выбранные типы размещения

        private ObservableCollection<string> _SelectedPlaces;
        public ObservableCollection<string> SelectedPlaces
        {
            get
            {
                return _SelectedPlaces;
            }
            set
            {
                _SelectedPlaces = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Выбранный менеджер

        private string _SelectedManager;
        public string SelectedManager
        {
            get
            {
                return _SelectedManager;
            }
            set
            {
                _SelectedManager = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Выбранный статус

        private string _SelectedStatus;
        public string SelectedStatus
        {
            get
            {
                return _SelectedStatus;
            }
            set
            {
                _SelectedStatus = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Выбранный тип заявки

        private string _SelectedType;
        public string SelectedType
        {
            get
            {
                return _SelectedType;
            }
            set
            {
                _SelectedType = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Выбранный источник

        private string _SelectedSource;
        public string SelectedSource
        {
            get
            {
                return _SelectedSource;
            }
            set
            {
                _SelectedSource = value;
                OnPropertyChanged();
            }
        }

        #endregion



        private string GetStringValue(string field, DataRow Row)
        {
            if (Row != null)
            {
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
            MainWindowViewModel.CloseLead();
        }

        #endregion

        #region SaveCommand
        public ICommand SaveCommand { get; }

        private bool CanSaveCommandExecuted(object p) => true;

        private void OnSaveCommandExecuted(object p)
        {
            if (PersonId != "0")
                DbHelper.UpdatePerson(Surname, Name, FatherName, Phone1, Phone2, Phone3, Mail, Birthday, PersonId);
            else
                DbHelper.CreatePerson(Surname, Name, FatherName, Phone1, Phone2, Phone3, Mail, Birthday);
            if (LeadId != "0")
                DbHelper.UpdateLead(SelectedSource, SelectedStatus, DateShipment, Nights, Adults, Сhildren, Budget, SelectedType, SelectedManager, Note, PersonId, LeadId);
            else
                DbHelper.CreateLead(SelectedSource, SelectedStatus, DateShipment, Nights, Adults, Сhildren, Budget, CreationDate, SelectedType, SelectedManager, Note, PersonId);
            DbHelper.SaveMarks(PersonId, SelectedMarks);
            DbHelper.SaveCountries(LeadId, SelectedCountries);
            DbHelper.SaveFood(LeadId, SelectedFood);
            DbHelper.SavePlaces(LeadId, SelectedPlaces);
            MainWindowViewModel.CloseLead();
        }

        #endregion

        #region DeleteCommand
        public ICommand DeleteCommand { get; }

        private bool CanDeleteCommandExecuted(object p) => true;

        private void OnDeleteCommandExecuted(object p)
        {
            MainWindowViewModel.CloseLead();
            DbHelper.DeleteLead(LeadId);
        }

        #endregion

        public void GetPerson()
        {
            Person = DbHelper.CurrentPerson(PersonId);
            PersonRow = Person.Rows[0];
            Surname = GetStringValue("Фамилия", PersonRow);
            Name = GetStringValue("Имя", PersonRow);
            FatherName = GetStringValue("Отчество", PersonRow);
            Phone1 = GetStringValue("Номер телефона 1", PersonRow);
            Phone2 = GetStringValue("Номер телефона 2", PersonRow);
            Phone3 = GetStringValue("Номер телефона 3", PersonRow);
            Mail = GetStringValue("E-mail", PersonRow);
            Birthday = GetStringValue("День рождения", PersonRow);
            if (Birthday != null)
            {
                DateTime birth = DateTime.ParseExact(Birthday, "dd.MM.yyyy h:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                Birthday = birth.Month.ToString() + "/" + birth.Day.ToString() + "/" + birth.Year.ToString() + " " + birth.Hour.ToString("00") + ":" + birth.Minute.ToString("00") + ":" + birth.Second.ToString("00") + " AM";
            }
            SelectedMarks = DbHelper.SelectedMarks(PersonId);
        }
    }
}
