using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace Pozdravlator
{

    struct Human
    {
        public string name;
        public int year;
        public int month;
        public int day;
    }

    class Note
    {

        List<Human> bDays = new List<Human>();
        int tableWidth = 72;
        string path = "";


        private void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        private void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        private string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        private int BdLen()
        {
            int count = 0;
            using (SQLiteConnection DB = new SQLiteConnection($"Data Source = {path}"))
            {
                DB.Open();
                SQLiteCommand CMD = DB.CreateCommand();

                CMD.CommandText = "SELECT COUNT(*) FROM Birthdays";
                count = Convert.ToInt32(CMD.ExecuteScalar());
                DB.Close();
            }

            return count;
        }

        private void Sql(string text)
        {

            using (SQLiteConnection DB = new SQLiteConnection($"Data Source = {path}"))
            {
                DB.Open();
                SQLiteCommand CMD = DB.CreateCommand();
                CMD.CommandText = text;
                CMD.ExecuteNonQuery();
                DB.Close();
            }

        }

        private void ReRight()
        {


            Sql("DELETE FROM Birthdays");
            //Console.Write(bDays.Count());
            Console.ReadKey(true);
            for (int i = 0; i < bDays.Count(); ++i)
            {
                AddHuman(bDays[i].name, bDays[i].year, bDays[i].month, bDays[i].day, -2);
            }

        }

        public bool BdPath()
        {

            path = $@"{System.AppDomain.CurrentDomain.BaseDirectory}info.db";
            bool dirExists = File.Exists(path);
            if (dirExists)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"База данных не найдена.\nПожалуйста поместите вашу базу данных по адресу {System.AppDomain.CurrentDomain.BaseDirectory}\nи назавите её info.db");
                return false;
            }


        }

        public int Len()
        {

            return bDays.Count();
        }

        public void AddHuman(string Name, int Year, int Month, int Day, int edit = -1)
        {

            Human men = new Human();
            men.name = Name;
            men.year = Year;
            men.month = Month;
            men.day = Day;
            if (edit == -1)
            {
                bDays.Add(men);
                int count = BdLen();
                Sql($"INSERT INTO Birthdays (Id, Name, Day, Month, Year) VALUES ({count + 1}, '{men.name}', {men.day}, {men.month}, {men.year})");
            }
            else if (edit == -2)
            {
                int count = BdLen();
                Sql($"INSERT INTO Birthdays (Id, Name, Day, Month, Year) VALUES ({count + 1}, '{men.name}', {men.day}, {men.month}, {men.year})");

            }
            else
            {



                bDays.Insert(edit, men);
                Sql($"UPDATE Birthdays SET Name = '{men.name}', Day = {men.day}, Month = {men.month}, Year = {men.year} WHERE Id = {edit + 1}");
            }

        }

        public void LoadFromDB()
        {

            using (SQLiteConnection DB = new SQLiteConnection($"Data Source = {path}"))
            {
                DB.Open();
                SQLiteCommand CMD = DB.CreateCommand();

                CMD.CommandText = "SELECT * FROM Birthdays";
                SQLiteDataReader reader = CMD.ExecuteReader();

                Human men = new Human();


                while (reader.Read())
                {

                    men.name = reader.GetString(reader.GetOrdinal("Name"));
                    men.year = reader.GetInt32(reader.GetOrdinal("Year"));
                    men.month = reader.GetInt32(reader.GetOrdinal("Month"));
                    men.day = reader.GetInt32(reader.GetOrdinal("Day"));
                    bDays.Add(men);

                }
                DB.Close();
            }



        }

        public void SortPrint()
        {
            string date = "";
            PrintLine();
            PrintRow("Ближайшие");
            PrintLine();
            PrintRow("Имя", "Дата Рождения", "До дня рождения");
            PrintLine();
            for (int i = 0; i < bDays.Count; ++i)
            {
                date = bDays[i].day + "." + bDays[i].month + "." + bDays[i].year;

                DateTime today = DateTime.Now;
                DateTime bDate = new DateTime(bDays[i].year, bDays[i].month, bDays[i].day);
                TimeSpan leftDays;
                int leftMonth = 0;

                if ((today.Month >= bDate.Month) && (today.Day > bDate.Day))
                {
                    DateTime newBTime = new DateTime((today.Year + 1), bDate.Month, bDate.Day);
                    leftMonth = Math.Abs(Convert.ToInt32(((today.Year - newBTime.Year) * 12) + today.Month - newBTime.Month));
                    if (leftMonth <= 1)
                    {
                        leftDays = newBTime - today;
                        PrintRow(bDays[i].name, date, Convert.ToString(leftDays.Days + 1) + " дней");
                    }



                }
                else
                {
                    DateTime newBTime = new DateTime((today.Year), bDate.Month, bDate.Day);

                    leftMonth = Math.Abs(Convert.ToInt32(((today.Year - newBTime.Year) * 12) + today.Month - newBTime.Month));
                    if (leftMonth <= 1)
                    {
                        leftDays = newBTime - today;
                        PrintRow(bDays[i].name, date, Convert.ToString(leftDays.Days + 1) + " дней");
                    }


                }
            }
            ////////////////////////////////////////

            PrintLine();
            PrintRow("Предстоящие");
            PrintLine();
            PrintRow("Имя", "Дата Рождения", "До дня рождения");
            PrintLine();

            for (int i = 0; i < bDays.Count; ++i)
            {
                date = bDays[i].day + "." + bDays[i].month + "." + bDays[i].year;

                DateTime today = DateTime.Now;
                DateTime bDate = new DateTime(bDays[i].year, bDays[i].month, bDays[i].day);

                int leftMonth = 0;


                if ((today.Month >= bDate.Month) && (today.Day > bDate.Day))
                {
                    DateTime newBTime = new DateTime((today.Year + 1), bDate.Month, bDate.Day);
                    leftMonth = Math.Abs(Convert.ToInt32(((today.Year - newBTime.Year) * 12) + today.Month - newBTime.Month));
                    if (leftMonth > 1)
                    {
                        PrintRow(bDays[i].name, date, Convert.ToString(leftMonth) + " месяцев");
                    }

                }
                else
                {
                    DateTime newBTime = new DateTime((today.Year), bDate.Month, bDate.Day);

                    leftMonth = Math.Abs(Convert.ToInt32(((today.Year - newBTime.Year) * 12) + today.Month - newBTime.Month));
                    if (leftMonth > 1)
                    {
                        PrintRow(bDays[i].name, date, Convert.ToString(leftMonth) + " месяцев");
                    }

                }
            }
            PrintLine();

        }

        public void Print()
        {
            string date = "";
            PrintLine();
            PrintRow("Номер", "Имя", "Дата Рождения", "До дня рождения");
            PrintLine();
            for (int i = 0; i < bDays.Count; ++i)
            {
                date = bDays[i].day + "." + bDays[i].month + "." + bDays[i].year;

                DateTime today = DateTime.Now;
                DateTime bDate = new DateTime(bDays[i].year, bDays[i].month, bDays[i].day);
                TimeSpan leftDays;
                int leftMonth = 0;

                if ((today.Month >= bDate.Month) && (today.Day > bDate.Day))
                {
                    DateTime newBTime = new DateTime((today.Year + 1), bDate.Month, bDate.Day);
                    leftMonth = Math.Abs(Convert.ToInt32(((today.Year - newBTime.Year) * 12) + today.Month - newBTime.Month));
                    if (leftMonth <= 1)
                    {
                        leftDays = newBTime - today;
                        PrintRow(Convert.ToString(i + 1), bDays[i].name, date, Convert.ToString(leftDays.Days + 1) + " дней");
                    }
                    else
                    {
                        PrintRow(Convert.ToString(i + 1), bDays[i].name, date, Convert.ToString(leftMonth) + " месяцев");
                    }


                }
                else
                {
                    DateTime newBTime = new DateTime((today.Year), bDate.Month, bDate.Day);

                    leftMonth = Math.Abs(Convert.ToInt32(((today.Year - newBTime.Year) * 12) + today.Month - newBTime.Month));
                    if (leftMonth <= 1)
                    {
                        leftDays = newBTime - today;
                        PrintRow(Convert.ToString(i + 1), bDays[i].name, date, Convert.ToString(leftDays.Days + 1) + " дней");
                    }
                    else
                    {
                        PrintRow(Convert.ToString(i + 1), bDays[i].name, date, Convert.ToString(leftMonth) + " месяцев");
                    }

                }


            }
            PrintLine();
        }

        public bool DeleteDate(int count, bool db = false)
        {


            count = count - 1;
            if (count > bDays.Count - 1 || count < 0 || (bDays.Count - 1 == 0 && count == 0))
            {
                Console.Clear();
                Console.WriteLine("\nОшибка ввода");

                return false;
            }
            else
            {
                bDays.RemoveAt(count);
                if (db)
                {

                    ReRight();

                }

                return true;
            }

        }

    }
}

