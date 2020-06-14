using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace SantaCRM.Models
{
    public static class DbHelper
    {
        private static MySqlConnection connection;
        private static MySqlCommand cmd = null;
        private static DataTable dt;
        private static MySqlDataAdapter da;

        public static int EstablishConnection(string Login, string Password)
        {
            try
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
                builder.Server = "127.0.0.1";
                builder.UserID = Login;
                builder.Password = Password;
                builder.Database = "mydb";
                builder.SslMode = MySqlSslMode.None;
                connection = new MySqlConnection(builder.ToString());
                connection.Open();
                return 0;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 1045:
                        return 1;
                    case 1042:
                        return 2;
                    default:
                        return 100;
                }
            }
        }

        private static string ToursSql = "SELECT заявки.`ID заявки` as `#`, заявки.`Дата создания`, заявки.`Дата отправления`, заявки.`Дата прибытия`, тип_заявки.`Тип заявки`, заявки.`Туроператор`, заявки.`Страна`, заявки.`Отель`, CONCAT_WS(' ', покупатели.`Фамилия`, покупатели.`Имя`, покупатели.`Отчество`) as `Покупатель`, CONCAT_WS(' ', покупатели.`Номер телефона 1`, покупатели.`Номер телефона 2`, покупатели.`Номер телефона 3`) as `Телефоны`, покупатели.`E-mail`, CONCAT_WS(' ', менеджеры.`Фамилия`, менеджеры.`Имя`) as `Менеджер`, статусы.`Статус`, источники_обращения.`Источник обращения`, заявки.`Стоимость путевки`, заявки.`Себестоимость`, обращения.`Примечание` FROM заявки LEFT JOIN обращения ON заявки.`ID заявки` = обращения.`ID заявки` LEFT JOIN желаемая_страна ON обращения.`ID обращения`= желаемая_страна.`ID обращения` LEFT JOIN словарь_стран ON словарь_стран.`ID страны`= желаемая_страна.`ID страны` LEFT JOIN источники_обращения ON обращения.`ID источника` = источники_обращения.`ID источника` LEFT JOIN статусы ON статусы.`ID статуса` = обращения.`ID статуса` LEFT JOIN тип_заявки ON тип_заявки.`ID типа` = обращения.`ID типа` LEFT JOIN покупатели ON покупатели.`ID` = обращения.`ID покупателя` LEFT JOIN менеджеры ON менеджеры.`ID менеджера` = обращения.`ID менеджера` GROUP BY заявки.`ID заявки` ORDER BY заявки.`Дата создания`;";

        public static DataTable Leads(string FindString)
        {
            cmd = new MySqlCommand("SELECT обращения.`ID обращения` as `#`, обращения.`Дата создания`, обращения.`Желаемая дата`, обращения.`Кол-во ночей`, тип_заявки.`Тип заявки`, GROUP_CONCAT(словарь_стран.`Название страны` ORDER BY словарь_стран.`Название страны` SEPARATOR ', ') as `Желаемые страны`, CONCAT_WS(' ', покупатели.`Фамилия`, покупатели.`Имя`, покупатели.`Отчество`) as `Покупатель`, CONCAT_WS(' ', покупатели.`Номер телефона 1`, покупатели.`Номер телефона 2`, покупатели.`Номер телефона 3`) as `Телефоны`, покупатели.`E-mail`, CONCAT_WS(' ', менеджеры.`Фамилия`, менеджеры.`Имя`) as `Менеджер`, статусы.`Статус`, источники_обращения.`Источник обращения`, обращения.`Примечание` FROM обращения LEFT JOIN желаемая_страна ON обращения.`ID обращения`= желаемая_страна.`ID обращения` LEFT JOIN словарь_стран ON словарь_стран.`ID страны`= желаемая_страна.`ID страны` LEFT JOIN источники_обращения ON обращения.`ID источника` = источники_обращения.`ID источника` LEFT JOIN статусы ON статусы.`ID статуса` = обращения.`ID статуса` LEFT JOIN тип_заявки ON тип_заявки.`ID типа` = обращения.`ID типа` LEFT JOIN покупатели ON покупатели.`ID` = обращения.`ID покупателя` LEFT JOIN менеджеры ON менеджеры.`ID менеджера` = обращения.`ID менеджера` WHERE CONCAT_WS(' ',обращения.`ID обращения`, обращения.`Дата создания`, обращения.`Кол-во ночей`, тип_заявки.`Тип заявки`, словарь_стран.`Название страны`, покупатели.`Фамилия`, покупатели.`Имя`, покупатели.`Отчество`, покупатели.`Номер телефона 1`, покупатели.`Номер телефона 2`, покупатели.`Номер телефона 3`, покупатели.`E-mail`, менеджеры.`Фамилия`, менеджеры.`Имя`, статусы.`Статус`, источники_обращения.`Источник обращения`, обращения.`Примечание`) LIKE '%" + FindString + "%' GROUP BY обращения.`ID обращения` ORDER BY обращения.`Дата создания`;", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            DataTable Person = dt.Clone();
            Person.Columns[1].DataType = typeof(string);
            Person.Columns[2].DataType = typeof(string);
            foreach (DataRow row in dt.Rows)
                Person.ImportRow(row);
            return Person;
        }

        public static DataTable Persons(string PersonsString)
        {
            cmd = new MySqlCommand("SELECT покупатели.`ID`, покупатели.`Фамилия`, покупатели.`Имя`, покупатели.`Отчество`, CONCAT_WS(' ', покупатели.`Номер телефона 1`, покупатели.`Номер телефона 2`, покупатели.`Номер телефона 3`) as `Телефоны`, покупатели.`E-mail`, покупатели.`День рождения`, GROUP_CONCAT(словарь_меток.`Название метки` ORDER BY словарь_меток.`Название метки` SEPARATOR ', ') as `Метки` FROM mydb.покупатели LEFT JOIN метки ON покупатели.`ID` = метки.`ID покупателя` LEFT JOIN словарь_меток ON метки.`ID метки` = словарь_меток.`ID метки` WHERE покупатели.`ID` LIKE '" + PersonsString + "%' OR покупатели.`Фамилия` LIKE '" + PersonsString + "%' OR покупатели.`Имя` LIKE '" + PersonsString + "%' OR покупатели.`Отчество` LIKE '" + PersonsString + "%' OR CONCAT_WS(' ', покупатели.`Номер телефона 1`, покупатели.`Номер телефона 2`, покупатели.`Номер телефона 3`) LIKE '" + PersonsString + "%' OR покупатели.`E-mail` LIKE '" + PersonsString + "%' OR покупатели.`День рождения` LIKE '%" + PersonsString + "%' OR словарь_меток.`Название метки` LIKE '" + PersonsString + "%' GROUP BY покупатели.`ID`;", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            DataTable Person = dt.Clone();
            Person.Columns[6].DataType = typeof(string);
            foreach (DataRow row in dt.Rows)
                Person.ImportRow(row);
            return Person;
        }

        public static DataTable Tours()
        {
            cmd = new MySqlCommand(ToursSql, connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        public static DataTable CurrentPerson(string ID)
        {
            cmd = new MySqlCommand("SELECT покупатели.`ID`, покупатели.`Фамилия`, покупатели.`Имя`, покупатели.`Отчество`, покупатели.`Номер телефона 1`, покупатели.`Номер телефона 2`, покупатели.`Номер телефона 3`, покупатели.`E-mail`, покупатели.`День рождения`, GROUP_CONCAT(словарь_меток.`Название метки` ORDER BY словарь_меток.`Название метки` SEPARATOR ', ') as `Метки` FROM mydb.покупатели LEFT JOIN метки ON покупатели.`ID` = метки.`ID покупателя` LEFT JOIN словарь_меток ON метки.`ID метки` = словарь_меток.`ID метки` WHERE покупатели.`ID` = " + ID.ToString() + " GROUP BY покупатели.`ID`;", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            DataTable Person = dt.Clone();
            Person.Columns[8].DataType = typeof(string);
            foreach (DataRow row in dt.Rows)
                Person.ImportRow(row);
            return Person;
        }

        public static DataTable CurrentLead(string LeadId)
        {
            cmd = new MySqlCommand("SELECT обращения.`ID обращения`, обращения.`ID покупателя`, обращения.`Дата создания`, обращения.`Желаемая дата`, обращения.`Кол-во ночей`, обращения.`Кол-во взрослых`, обращения.`Кол-во детей`, обращения.`Бюджет`, тип_заявки.`Тип заявки`, CONCAT_WS(' ', менеджеры.`Фамилия`, менеджеры.`Имя`) as `Менеджер`, статусы.`Статус`, источники_обращения.`Источник обращения`, обращения.`Примечание` FROM обращения LEFT JOIN источники_обращения ON обращения.`ID источника` = источники_обращения.`ID источника` LEFT JOIN статусы ON статусы.`ID статуса` = обращения.`ID статуса` LEFT JOIN тип_заявки ON тип_заявки.`ID типа` = обращения.`ID типа` LEFT JOIN менеджеры ON менеджеры.`ID менеджера` = обращения.`ID менеджера` WHERE обращения.`ID обращения` = " + LeadId.ToString() + ";", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            DataTable Lead = dt.Clone();
            foreach (DataColumn column in Lead.Columns)
                column.DataType = typeof(string);
            foreach (DataRow row in dt.Rows)
                Lead.ImportRow(row);
            return Lead;
        }

        public static void UpdatePerson(string Surname, string Name, string FatherName, string Phone1, string Phone2, string Phone3, string Mail, string Birthday, string Id)
        {
            if (Birthday == null)
                Birthday = ", `День рождения` = NULL";
            else
            {
                DateTime birth = DateTime.ParseExact(Birthday, "M/d/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                Birthday = ", `День рождения` = '" + birth.Year.ToString() + "-" + birth.Month.ToString() + "-" + birth.Day.ToString() + "'";
            }
            cmd = new MySqlCommand(@"UPDATE `mydb`.`покупатели` SET `Фамилия` = '" + Surname + "', `Имя` = '" + Name + "', `Отчество` = '" + FatherName + "', `Номер телефона 1` = '" + Phone1 + "', `Номер телефона 2` = '" + Phone2 + "', `Номер телефона 3` = '" + Phone3 + "', `E-mail` = '" + Mail + "'" + Birthday + " WHERE (`ID` = '" + Id + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static void UpdateLead(string Source, string Status, string DateShipment, string Nights, string Adults, string Children, string Budget, string Type, string Manager, string Note, string PersonId, string LeadId)
        {
            try
            {
                if (Source != null)
                {
                    cmd = new MySqlCommand("SELECT источники_обращения.`ID источника` FROM mydb.источники_обращения WHERE источники_обращения.`Источник обращения` = '" + Source + "';", connection);
                    dt = new DataTable();
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);
                    Source = "'" + dt.Rows[0].Field<int>("ID источника").ToString() + "'";
                }
                else Source = "NULL";

                if (Status != null)
                {
                    cmd = new MySqlCommand("SELECT статусы.`ID статуса` FROM mydb.статусы WHERE статусы.`Статус` = '" + Status + "';", connection);
                    dt = new DataTable();
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);
                    Status = "'" + dt.Rows[0].Field<int>("ID статуса").ToString() + "'";
                }
                else Status = "NULL";

                if (DateShipment == null)
                    DateShipment = "NULL";
                else
                {
                    DateTime date = DateTime.ParseExact(DateShipment, "M/d/yyyy h:m:s tt", System.Globalization.CultureInfo.InvariantCulture);
                    DateShipment = "'" + date.ToString("yyyy-MM-dd") + "'";
                }

                if (Nights != null)
                    Nights = "'" + Nights + "'";
                else Nights = "NULL";

                if (Adults != null)
                    Adults = "'" + Adults + "'";
                else Adults = "NULL";

                if (Children != null)
                    Children = "'" + Children + "'";
                else Children = "NULL";

                if (Budget != null)
                    Budget = "'" + Budget + "'";
                else Budget = "NULL";

                if (Type != null)
                {
                    cmd = new MySqlCommand("SELECT тип_заявки.`ID типа` FROM mydb.тип_заявки WHERE тип_заявки.`Тип заявки` = '" + Type + "';", connection);
                    dt = new DataTable();
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);
                    Type = "'" + dt.Rows[0].Field<int>("ID типа").ToString() + "'";
                }
                else Type = "NULL";

                if (Manager != null)
                {
                    cmd = new MySqlCommand("SELECT менеджеры.`ID менеджера` FROM mydb.менеджеры WHERE CONCAT_WS(' ', менеджеры.`Фамилия`, менеджеры.`Имя`) = '" + Manager + "';", connection);
                    dt = new DataTable();
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);
                    Manager = "'" + dt.Rows[0].Field<int>("ID менеджера").ToString() + "'";
                }
                else Manager = "NULL";

                if (PersonId == "0")
                    PersonId = GetLastPersonId();

                cmd = new MySqlCommand(@"UPDATE `mydb`.`обращения` SET `ID источника` = " + Source + ", `ID статуса` = " + Status + ", `Желаемая дата` = " + DateShipment + ", `Кол-во ночей` = " + Nights + ", `Кол-во взрослых` = " + Adults + ", `Кол-во детей` = " + Children + ", `Бюджет` = " + Budget + ", `ID типа` = " + Type + ", `ID менеджера` = " + Manager + ", `ID покупателя` = '" + PersonId + "', `Примечание` = '" + Note + "' WHERE (`ID обращения` = '" + LeadId + "');", connection);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Что-то пошло не так, вопросы к разрабу...");
            }
        }

        public static void CreatePerson(string Surname, string Name, string FatherName, string Phone1, string Phone2, string Phone3, string Mail, string Birthday)
        {
            if (Birthday == null)
                Birthday = "NULL";
            else
            {
                DateTime birth = DateTime.ParseExact(Birthday, "M/d/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                Birthday = "'" + birth.ToString("yyyy-MM-dd") + "'";
            }
            cmd = new MySqlCommand("INSERT INTO `mydb`.`покупатели` (`Фамилия`, `Имя`, `Отчество`, `Номер телефона 1`, `Номер телефона 2`, `Номер телефона 3`, `E-mail`, `День рождения`) VALUES('" + Surname + "', '" + Name + "', '" + FatherName + "', '" + Phone1 + "', '" + Phone2 + "', '" + Phone3 + "', '" + Mail + "', " + Birthday + ");", connection);
            cmd.ExecuteNonQuery();
        }

        public static void CreateLead(string Source, string Status, string DateShipment, string Nights, string Adults, string Children, string Budget, string CreationDate, string Type, string Manager, string Note, string PersonId)
        {
            try
            {
                if (Source != null)
                {
                    cmd = new MySqlCommand("SELECT источники_обращения.`ID источника` FROM mydb.источники_обращения WHERE источники_обращения.`Источник обращения` = '" + Source + "';", connection);
                    dt = new DataTable();
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);
                    Source = "'" + dt.Rows[0].Field<int>("ID источника").ToString() + "'";
                }
                else Source = "NULL";

                if (Status != null)
                {
                    cmd = new MySqlCommand("SELECT статусы.`ID статуса` FROM mydb.статусы WHERE статусы.`Статус` = '" + Status + "';", connection);
                    dt = new DataTable();
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);
                    Status = "'" + dt.Rows[0].Field<int>("ID статуса").ToString() + "'";
                }
                else Status = "NULL";

                if (DateShipment == null)
                    DateShipment = "NULL";
                else
                {
                    DateTime date = DateTime.ParseExact(DateShipment, "M/d/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                    DateShipment = "'" + date.ToString("yyyy-MM-dd") + "'";
                }

                if (Nights != null)
                    Nights = "'" + Nights + "'";
                else Nights = "NULL";

                if (Adults != null)
                    Adults = "'" + Adults + "'";
                else Adults = "NULL";

                if (Children != null)
                    Children = "'" + Children + "'";
                else Children = "NULL";

                if (Budget != null)
                    Budget = "'" + Budget + "'";
                else Budget = "NULL";

                if (Type != null)
                {
                    cmd = new MySqlCommand("SELECT тип_заявки.`ID типа` FROM mydb.тип_заявки WHERE тип_заявки.`Тип заявки` = '" + Type + "';", connection);
                    dt = new DataTable();
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);
                    Type = "'" + dt.Rows[0].Field<int>("ID типа").ToString() + "'";
                }
                else Type = "NULL";

                if (Manager != null)
                {
                    cmd = new MySqlCommand("SELECT менеджеры.`ID менеджера` FROM mydb.менеджеры WHERE CONCAT_WS(' ', менеджеры.`Фамилия`, менеджеры.`Имя`) = '" + Manager + "';", connection);
                    dt = new DataTable();
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);
                    Manager = "'" + dt.Rows[0].Field<int>("ID менеджера").ToString() + "'";
                }
                else Manager = "NULL";

                DateTime _date = DateTime.ParseExact(CreationDate, "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                CreationDate = _date.ToString("yyyy-MM-dd HH:mm:ss");

                if (PersonId == "0")
                    PersonId = GetLastPersonId();

                cmd = new MySqlCommand("INSERT INTO `mydb`.`обращения` (`ID источника`, `ID статуса`, `Желаемая дата`, `Кол-во ночей`, `Кол-во взрослых`, `Кол-во детей`, `Бюджет`, `Дата создания`, `ID типа`, `ID менеджера`, `Примечание`, `ID покупателя`) VALUES (" + Source + ", " + Status + ", " + DateShipment + ", " + Nights + ", " + Adults + ", " + Children + ", " + Budget + ", '" + CreationDate + "', " + Type + ", " + Manager + ", '" + Note + "', '" + PersonId + "');", connection);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Что-то пошло не так, вопросы к разрабу...");
            }
        }

        public static void DeletePerson(string Id)
        {
            cmd = new MySqlCommand("DELETE FROM `mydb`.`метки` WHERE (`ID покупателя` = '" + Id + "');", connection);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand("DELETE FROM `mydb`.`покупатели` WHERE (`ID` = '" + Id + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteLead(string Id)
        {
            cmd = new MySqlCommand("DELETE FROM `mydb`.`желаемая_страна` WHERE (`ID обращения` = '" + Id + "');", connection);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand("DELETE FROM `mydb`.`желаемое_размещение` WHERE (`ID обращения` = '" + Id + "');", connection);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand("DELETE FROM `mydb`.`желаемое_питание` WHERE (`ID обращения` = '" + Id + "');", connection);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand("DELETE FROM `mydb`.`обращения` WHERE (`ID обращения` = '" + Id + "');", connection);
            cmd.ExecuteNonQuery();
        }

        #region Словари
        public static ObservableCollection<string> AllMarks()
        {
            cmd = new MySqlCommand("SELECT * FROM mydb.словарь_меток;", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            ObservableCollection<string> AllMarksCol = new ObservableCollection<string>();
            foreach (DataRow row in dt.Rows)
            {
                AllMarksCol.Add(row["Название метки"].ToString());
            }
            return AllMarksCol;
        }

        public static ObservableCollection<string> AllCountries()
        {
            cmd = new MySqlCommand("SELECT * FROM mydb.словарь_стран;", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            ObservableCollection<string> Collection = new ObservableCollection<string>();
            foreach (DataRow row in dt.Rows)
            {
                Collection.Add(row["Название страны"].ToString());
            }
            return Collection;
        }

        public static ObservableCollection<string> AllFood()
        {
            cmd = new MySqlCommand("SELECT * FROM mydb.словарь_питания;", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            ObservableCollection<string> Collection = new ObservableCollection<string>();
            foreach (DataRow row in dt.Rows)
            {
                Collection.Add(row["Тип питания"].ToString());
            }
            return Collection;
        }

        public static ObservableCollection<string> AllPlaces()
        {
            cmd = new MySqlCommand("SELECT * FROM mydb.словарь_размещения;", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            ObservableCollection<string> Collection = new ObservableCollection<string>();
            foreach (DataRow row in dt.Rows)
            {
                Collection.Add(row["Тип размещения"].ToString());
            }
            return Collection;
        }

        public static ObservableCollection<string> AllStatus()
        {
            cmd = new MySqlCommand("SELECT * FROM mydb.статусы;", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            ObservableCollection<string> Collection = new ObservableCollection<string>();
            foreach (DataRow row in dt.Rows)
            {
                Collection.Add(row["Статус"].ToString());
            }
            return Collection;
        }

        public static ObservableCollection<string> AllManagers()
        {
            cmd = new MySqlCommand("SELECT CONCAT_WS(' ', менеджеры.`Фамилия`, менеджеры.`Имя`) as `Менеджер` FROM mydb.менеджеры;", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            ObservableCollection<string> Collection = new ObservableCollection<string>();
            foreach (DataRow row in dt.Rows)
            {
                Collection.Add(row["Менеджер"].ToString());
            }
            return Collection;
        }

        public static ObservableCollection<string> AllSources()
        {
            cmd = new MySqlCommand("SELECT * FROM mydb.источники_обращения;", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            ObservableCollection<string> Collection = new ObservableCollection<string>();
            foreach (DataRow row in dt.Rows)
            {
                Collection.Add(row["Источник обращения"].ToString());
            }
            return Collection;
        }

        public static ObservableCollection<string> AllTypes()
        {
            cmd = new MySqlCommand("SELECT * FROM mydb.тип_заявки;", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            ObservableCollection<string> Collection = new ObservableCollection<string>();
            foreach (DataRow row in dt.Rows)
            {
                Collection.Add(row["Тип заявки"].ToString());
            }
            return Collection;
        }

        #endregion

        public static void DeleteMark(string SelectedItem)
        {
            cmd = new MySqlCommand("SELECT словарь_меток.`ID метки` FROM mydb.словарь_меток WHERE словарь_меток.`Название метки` = '" + SelectedItem + "';", connection);
            DataTable dt1 = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt1);
            int markId = dt1.Rows[0].Field<int>("ID метки");
            cmd = new MySqlCommand("DELETE FROM `mydb`.`метки` WHERE (`ID метки` = '" + markId + "');", connection);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand("DELETE FROM `mydb`.`словарь_меток` WHERE (`ID метки` = '" + markId + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteCountry(string SelectedItem)
        {
            cmd = new MySqlCommand("SELECT словарь_стран.`ID страны` FROM mydb.словарь_стран WHERE словарь_стран.`Название страны` = '" + SelectedItem + "';", connection);
            DataTable dt1 = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt1);
            int markId = dt1.Rows[0].Field<int>("ID страны");
            cmd = new MySqlCommand("DELETE FROM `mydb`.`желаемая_страна` WHERE (`ID страны` = '" + markId + "');", connection);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand("DELETE FROM `mydb`.`словарь_стран` WHERE (`ID страны` = '" + markId + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteFood(string SelectedItem)
        {
            cmd = new MySqlCommand("SELECT словарь_питания.`ID питания` FROM mydb.словарь_питания WHERE словарь_питания.`Тип питания` = '" + SelectedItem + "';", connection);
            DataTable dt1 = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt1);
            int markId = dt1.Rows[0].Field<int>("ID питания");
            cmd = new MySqlCommand("DELETE FROM `mydb`.`желаемое_питание` WHERE (`ID питания` = '" + markId + "');", connection);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand("DELETE FROM `mydb`.`словарь_питания` WHERE (`ID питания` = '" + markId + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static void DeletePlace(string SelectedItem)
        {
            cmd = new MySqlCommand("SELECT словарь_размещения.`ID размещения` FROM mydb.словарь_размещения WHERE словарь_размещения.`Тип размещения` = '" + SelectedItem + "';", connection);
            DataTable dt1 = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt1);
            int markId = dt1.Rows[0].Field<int>("ID размещения");
            cmd = new MySqlCommand("DELETE FROM `mydb`.`желаемое_размещение` WHERE (`ID размещения` = '" + markId + "');", connection);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand("DELETE FROM `mydb`.`словарь_размещения` WHERE (`ID размещения` = '" + markId + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteStatus(string SelectedItem)
        {
            cmd = new MySqlCommand("SELECT статусы.`ID статуса` FROM mydb.статусы WHERE статусы.`Статус` = '" + SelectedItem + "';", connection);
            DataTable dt1 = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt1);
            int markId = dt1.Rows[0].Field<int>("ID статуса");
            cmd = new MySqlCommand("UPDATE `mydb`.`обращения` SET `ID статуса` = NULL WHERE (`ID статуса` = '" + markId + "');", connection);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand("DELETE FROM `mydb`.`статусы` WHERE (`ID статуса` = '" + markId + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteSource(string SelectedItem)
        {
            cmd = new MySqlCommand("SELECT источники_обращения.`ID источника` FROM mydb.источники_обращения WHERE источники_обращения.`Источник обращения` = '" + SelectedItem + "';", connection);
            DataTable dt1 = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt1);
            int markId = dt1.Rows[0].Field<int>("ID источника");
            cmd = new MySqlCommand("UPDATE `mydb`.`обращения` SET `ID источника` = NULL WHERE (`ID источника` = '" + markId + "');", connection);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand("DELETE FROM `mydb`.`источники_обращения` WHERE (`ID источника` = '" + markId + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteType(string SelectedItem)
        {
            cmd = new MySqlCommand("SELECT тип_заявки.`ID типа` FROM mydb.тип_заявки WHERE тип_заявки.`Тип заявки` = '" + SelectedItem + "';", connection);
            DataTable dt1 = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt1);
            int markId = dt1.Rows[0].Field<int>("ID типа");
            cmd = new MySqlCommand("UPDATE `mydb`.`обращения` SET `ID типа` = NULL WHERE (`ID типа` = '" + markId + "');", connection);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand("DELETE FROM `mydb`.`тип_заявки` WHERE (`ID типа` = '" + markId + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static void CreateMark(string Name)
        {
            cmd = new MySqlCommand("INSERT INTO `mydb`.`словарь_меток` (`Название метки`) VALUES('" + Name + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static void CreateCountry(string Name)
        {
            cmd = new MySqlCommand("INSERT INTO `mydb`.`словарь_стран` (`Название страны`) VALUES('" + Name + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static void CreateFood(string Name)
        {
            cmd = new MySqlCommand("INSERT INTO `mydb`.`словарь_питания` (`Тип питания`) VALUES('" + Name + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static void CreatePlace(string Name)
        {
            cmd = new MySqlCommand("INSERT INTO `mydb`.`словарь_размещения` (`Тип размещения`) VALUES('" + Name + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static void CreateStatus(string Name)
        {
            cmd = new MySqlCommand("INSERT INTO `mydb`.`статусы` (`Статус`) VALUES('" + Name + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static void CreateType(string Name)
        {
            cmd = new MySqlCommand("INSERT INTO `mydb`.`тип_заявки` (`Тип заявки`) VALUES('" + Name + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static void CreateSource(string Name)
        {
            cmd = new MySqlCommand("INSERT INTO `mydb`.`источники_обращения` (`Источник обращения`) VALUES('" + Name + "');", connection);
            cmd.ExecuteNonQuery();
        }

        public static ObservableCollection<string> SelectedMarks(string Id)
        {
            cmd = new MySqlCommand("SELECT словарь_меток.`Название метки` FROM mydb.метки LEFT JOIN словарь_меток ON словарь_меток.`ID метки` = метки.`ID метки` WHERE метки.`ID покупателя` = " + Id + ";", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            ObservableCollection<string> SelectedMarksCol = new ObservableCollection<string>();
            foreach (DataRow row in dt.Rows)
            {
                SelectedMarksCol.Add(row["Название метки"].ToString());
            }
            return SelectedMarksCol;
        }

        public static ObservableCollection<string> SelectedCountries(string LeadId)
        {
            cmd = new MySqlCommand("SELECT словарь_стран.`Название страны` FROM mydb.желаемая_страна LEFT JOIN словарь_стран ON словарь_стран.`ID страны` = желаемая_страна.`ID страны` WHERE желаемая_страна.`ID обращения` = " + LeadId + ";", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            ObservableCollection<string> SelectedMarksCol = new ObservableCollection<string>();
            foreach (DataRow row in dt.Rows)
            {
                SelectedMarksCol.Add(row["Название страны"].ToString());
            }
            return SelectedMarksCol;
        }

        public static ObservableCollection<string> SelectedFood(string LeadId)
        {
            cmd = new MySqlCommand("SELECT словарь_питания.`Тип питания` FROM mydb.желаемое_питание LEFT JOIN словарь_питания ON словарь_питания.`ID питания` = желаемое_питание.`ID питания` WHERE желаемое_питание.`ID обращения` = " + LeadId + ";", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            ObservableCollection<string> SelectedMarksCol = new ObservableCollection<string>();
            foreach (DataRow row in dt.Rows)
            {
                SelectedMarksCol.Add(row["Тип питания"].ToString());
            }
            return SelectedMarksCol;
        }

        public static ObservableCollection<string> SelectedPlaces(string LeadId)
        {
            cmd = new MySqlCommand("SELECT словарь_размещения.`Тип размещения` FROM mydb.желаемое_размещение LEFT JOIN словарь_размещения ON словарь_размещения.`ID размещения` = желаемое_размещение.`ID размещения` WHERE желаемое_размещение.`ID обращения` = " + LeadId + ";", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            ObservableCollection<string> SelectedMarksCol = new ObservableCollection<string>();
            foreach (DataRow row in dt.Rows)
            {
                SelectedMarksCol.Add(row["Тип размещения"].ToString());
            }
            return SelectedMarksCol;
        }

        public static void SaveMarks(string Id, ObservableCollection<string> Marks)
        {
            int markId;
            bool flag;
            if (Id == "0" && Marks != null)
            {
                string ID1 = GetLastPersonId();
                foreach (string Mark in Marks)
                {
                    cmd = new MySqlCommand("SELECT словарь_меток.`ID метки` FROM mydb.словарь_меток WHERE словарь_меток.`Название метки` = '" + Mark + "';", connection);
                    DataTable dt1 = new DataTable();
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(dt1);
                    markId = dt1.Rows[0].Field<int>("ID метки");
                    cmd = new MySqlCommand("INSERT INTO `mydb`.`метки` (`ID метки`, `ID покупателя`) VALUES ('" + markId + "', '" + ID1 + "');", connection);
                    cmd.ExecuteNonQuery();
                }
            }
            else if (Marks != null)
            {
                cmd = new MySqlCommand("SELECT словарь_меток.`Название метки` FROM mydb.метки LEFT JOIN словарь_меток ON словарь_меток.`ID метки` = метки.`ID метки` WHERE метки.`ID покупателя` = " + Id + ";", connection);
                dt = new DataTable();
                da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (string Mark in Marks)
                {
                    flag = true;
                    markId = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Название метки"].ToString() == Mark)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag == true)
                    {
                        cmd = new MySqlCommand("SELECT словарь_меток.`ID метки` FROM mydb.словарь_меток WHERE словарь_меток.`Название метки` = '" + Mark + "';", connection);
                        DataTable dt1 = new DataTable();
                        da = new MySqlDataAdapter(cmd);
                        da.Fill(dt1);
                        markId = dt1.Rows[0].Field<int>("ID метки");
                        cmd = new MySqlCommand("INSERT INTO `mydb`.`метки` (`ID метки`, `ID покупателя`) VALUES ('" + markId + "', '" + Id + "');", connection);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            if (Id != "0" && Marks == null)
            {
                cmd = new MySqlCommand("DELETE FROM `mydb`.`метки` WHERE (`ID` = '" + Id + "');", connection);
                cmd.ExecuteNonQuery();
            }
            else if (Id != "0")
            {
                cmd = new MySqlCommand("SELECT словарь_меток.`Название метки`, метки.`ID` FROM mydb.метки LEFT JOIN словарь_меток ON словарь_меток.`ID метки` = метки.`ID метки` WHERE метки.`ID покупателя` = " + Id + ";", connection);
                dt = new DataTable();
                da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    flag = true;
                    foreach (string Mark in Marks)
                    {
                        if (row["Название метки"].ToString() == Mark)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag == true)
                    {
                        cmd = new MySqlCommand("DELETE FROM `mydb`.`метки` WHERE (`ID` = '" + row["ID"].ToString() + "');", connection);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void SaveCountries(string Id, ObservableCollection<string> Marks)
        {
            int markId;
            bool flag;
            if (Id == "0" && Marks != null)
            {
                string ID1 = GetLastLeadId();
                foreach (string Mark in Marks)
                {
                    cmd = new MySqlCommand("SELECT словарь_стран.`ID страны` FROM mydb.словарь_стран WHERE словарь_стран.`Название страны` = '" + Mark + "';", connection);
                    DataTable dt1 = new DataTable();
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(dt1);
                    markId = dt1.Rows[0].Field<int>("ID страны");
                    cmd = new MySqlCommand("INSERT INTO `mydb`.`желаемая_страна` (`ID страны`, `ID обращения`) VALUES ('" + markId + "', '" + ID1 + "');", connection);
                    cmd.ExecuteNonQuery();
                }
            }
            else if (Marks != null)
            {
                cmd = new MySqlCommand("SELECT словарь_стран.`Название страны` FROM mydb.желаемая_страна LEFT JOIN словарь_стран ON словарь_стран.`ID страны` = желаемая_страна.`ID страны` WHERE желаемая_страна.`ID обращения` = " + Id + ";", connection);
                dt = new DataTable();
                da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (string Mark in Marks)
                {
                    flag = true;
                    markId = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Название страны"].ToString() == Mark)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag == true)
                    {
                        cmd = new MySqlCommand("SELECT словарь_стран.`ID страны` FROM mydb.словарь_стран WHERE словарь_стран.`Название страны` = '" + Mark + "';", connection);
                        DataTable dt1 = new DataTable();
                        da = new MySqlDataAdapter(cmd);
                        da.Fill(dt1);
                        markId = dt1.Rows[0].Field<int>("ID страны");
                        cmd = new MySqlCommand("INSERT INTO `mydb`.`желаемая_страна` (`ID страны`, `ID обращения`) VALUES ('" + markId + "', '" + Id + "');", connection);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            if (Id != "0" && Marks == null)
            {
                cmd = new MySqlCommand("DELETE FROM `mydb`.`желаемая_страна` WHERE (`ID` = '" + Id + "');", connection);
                cmd.ExecuteNonQuery();
            }
            else if (Id != "0")
            {
                cmd = new MySqlCommand("SELECT словарь_стран.`Название страны`, желаемая_страна.`ID` FROM mydb.желаемая_страна LEFT JOIN словарь_стран ON словарь_стран.`ID страны` = желаемая_страна.`ID страны` WHERE желаемая_страна.`ID обращения` = " + Id + ";", connection);
                dt = new DataTable();
                da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    flag = true;
                    foreach (string Mark in Marks)
                    {
                        if (row["Название страны"].ToString() == Mark)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag == true)
                    {
                        cmd = new MySqlCommand("DELETE FROM `mydb`.`желаемая_страна` WHERE (`ID` = '" + row["ID"].ToString() + "');", connection);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void SaveFood(string Id, ObservableCollection<string> Marks)
        {
            int markId;
            bool flag;
            if (Id == "0" && Marks != null)
            {
                string ID1 = GetLastLeadId();
                foreach (string Mark in Marks)
                {
                    cmd = new MySqlCommand("SELECT словарь_питания.`ID питания` FROM mydb.словарь_питания WHERE словарь_питания.`Тип питания` = '" + Mark + "';", connection);
                    DataTable dt1 = new DataTable();
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(dt1);
                    markId = dt1.Rows[0].Field<int>("ID питания");
                    cmd = new MySqlCommand("INSERT INTO `mydb`.`желаемое_питание` (`ID питания`, `ID обращения`) VALUES ('" + markId + "', '" + ID1 + "');", connection);
                    cmd.ExecuteNonQuery();
                }
            }
            else if (Marks != null)
            {
                cmd = new MySqlCommand("SELECT словарь_питания.`Тип питания` FROM mydb.желаемое_питание LEFT JOIN словарь_питания ON словарь_питания.`ID питания` = желаемое_питание.`ID питания` WHERE желаемое_питание.`ID питания` = " + Id + ";", connection);
                dt = new DataTable();
                da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (string Mark in Marks)
                {
                    flag = true;
                    markId = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Тип питания"].ToString() == Mark)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag == true)
                    {
                        cmd = new MySqlCommand("SELECT словарь_питания.`ID питания` FROM mydb.словарь_питания WHERE словарь_питания.`Тип питания` = '" + Mark + "';", connection);
                        DataTable dt1 = new DataTable();
                        da = new MySqlDataAdapter(cmd);
                        da.Fill(dt1);
                        markId = dt1.Rows[0].Field<int>("ID питания");
                        cmd = new MySqlCommand("INSERT INTO `mydb`.`желаемое_питание` (`ID питания`, `ID обращения`) VALUES ('" + markId + "', '" + Id + "');", connection);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            if (Id != "0" && Marks == null)
            {
                cmd = new MySqlCommand("DELETE FROM `mydb`.`желаемое_питание` WHERE (`ID` = '" + Id + "');", connection);
                cmd.ExecuteNonQuery();
            }
            else if (Id != "0")
            {
                cmd = new MySqlCommand("SELECT словарь_питания.`Тип питания`, желаемое_питание.`ID` FROM mydb.желаемое_питание LEFT JOIN словарь_питания ON словарь_питания.`ID питания` = желаемое_питание.`ID питания` WHERE желаемое_питание.`ID обращения` = " + Id + ";", connection);
                dt = new DataTable();
                da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    flag = true;
                    foreach (string Mark in Marks)
                    {
                        if (row["Тип питания"].ToString() == Mark)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag == true)
                    {
                        cmd = new MySqlCommand("DELETE FROM `mydb`.`желаемое_питание` WHERE (`ID` = '" + row["ID"].ToString() + "');", connection);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void SavePlaces(string Id, ObservableCollection<string> Marks)
        {
            int markId;
            bool flag;
            if (Id == "0" && Marks != null)
            {
                string ID1 = GetLastLeadId();
                foreach (string Mark in Marks)
                {
                    cmd = new MySqlCommand("SELECT словарь_размещения.`ID размещения` FROM mydb.словарь_размещения WHERE словарь_размещения.`Тип размещения` = '" + Mark + "';", connection);
                    DataTable dt1 = new DataTable();
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(dt1);
                    markId = dt1.Rows[0].Field<int>("ID размещения");
                    cmd = new MySqlCommand("INSERT INTO `mydb`.`желаемое_размещение` (`ID размещения`, `ID обращения`) VALUES ('" + markId + "', '" + ID1 + "');", connection);
                    cmd.ExecuteNonQuery();
                }
            }
            else if (Marks != null)
            {
                cmd = new MySqlCommand("SELECT словарь_размещения.`Тип размещения` FROM mydb.желаемое_размещение LEFT JOIN словарь_размещения ON словарь_размещения.`ID размещения` = желаемое_размещение.`ID размещения` WHERE желаемое_размещение.`ID размещения` = " + Id + ";", connection);
                dt = new DataTable();
                da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (string Mark in Marks)
                {
                    flag = true;
                    markId = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Тип размещения"].ToString() == Mark)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag == true)
                    {
                        cmd = new MySqlCommand("SELECT словарь_размещения.`ID размещения` FROM mydb.словарь_размещения WHERE словарь_размещения.`Тип размещения` = '" + Mark + "';", connection);
                        DataTable dt1 = new DataTable();
                        da = new MySqlDataAdapter(cmd);
                        da.Fill(dt1);
                        markId = dt1.Rows[0].Field<int>("ID размещения");
                        cmd = new MySqlCommand("INSERT INTO `mydb`.`желаемое_размещение` (`ID размещения`, `ID обращения`) VALUES ('" + markId + "', '" + Id + "');", connection);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            if (Id != "0" && Marks == null)
            {
                cmd = new MySqlCommand("DELETE FROM `mydb`.`желаемое_размещение` WHERE (`ID` = '" + Id + "');", connection);
                cmd.ExecuteNonQuery();
            }
            else if (Id != "0")
            {
                cmd = new MySqlCommand("SELECT словарь_размещения.`Тип размещения`, желаемое_размещение.`ID` FROM mydb.желаемое_размещение LEFT JOIN словарь_размещения ON словарь_размещения.`ID размещения` = желаемое_размещение.`ID размещения` WHERE желаемое_размещение.`ID обращения` = " + Id + ";", connection);
                dt = new DataTable();
                da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    flag = true;
                    foreach (string Mark in Marks)
                    {
                        if (row["Тип размещения"].ToString() == Mark)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag == true)
                    {
                        cmd = new MySqlCommand("DELETE FROM `mydb`.`желаемое_размещение` WHERE (`ID` = '" + row["ID"].ToString() + "');", connection);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static string GetLastPersonId()
        {
            cmd = new MySqlCommand("SELECT max(ID) FROM mydb.покупатели;", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            string Id = dt.Rows[0].Field<int>("max(ID)").ToString();
            return Id;
        }

        public static string GetLastLeadId()
        {
            cmd = new MySqlCommand("SELECT max(`ID обращения`) FROM mydb.обращения;", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            string Id = dt.Rows[0].Field<int>("max(`ID обращения`)").ToString();
            return Id;
        }

        public static ObservableCollection<string> FindPerson(string Search)
        {
            cmd = new MySqlCommand("SELECT CONCAT_WS(' ', покупатели.`Фамилия`, покупатели.`Имя`, покупатели.`Отчество`, покупатели.`Номер телефона 1`, покупатели.`Номер телефона 2`, покупатели.`Номер телефона 3`, покупатели.`E-mail`) as `Результат` FROM mydb.покупатели WHERE CONCAT_WS(' ', покупатели.`Фамилия`, покупатели.`Имя`, покупатели.`Отчество`, покупатели.`Номер телефона 1`, покупатели.`Номер телефона 2`, покупатели.`Номер телефона 3`, покупатели.`E-mail`) LIKE '%" + Search + "%';", connection);
            dt = new DataTable();
            da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            ObservableCollection<string> Result = new ObservableCollection<string>();
            foreach (DataRow row in dt.Rows)
                Result.Add(row["Результат"].ToString());
            return Result;
        }

        public static string FindPersonId(string PersonString)
        {
            try
            {
                cmd = new MySqlCommand("SELECT покупатели.`ID` FROM mydb.покупатели WHERE CONCAT_WS(' ', покупатели.`Фамилия`, покупатели.`Имя`, покупатели.`Отчество`, покупатели.`Номер телефона 1`, покупатели.`Номер телефона 2`, покупатели.`Номер телефона 3`, покупатели.`E-mail`) = '" + PersonString + "';", connection);
                dt = new DataTable();
                da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                string Id = dt.Rows[0].Field<int>("ID").ToString();
                return Id;
            }
            catch (System.IndexOutOfRangeException)
            {
                return null;
            }
        }
    }
}
