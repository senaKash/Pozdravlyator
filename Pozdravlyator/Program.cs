using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace Pozdravlator
{
    class Program
    {


        static void test(ref Note human)
        {
            human.AddHuman("Игорь", 1999, 12, 1);
            human.AddHuman("Михаил", 1999, 03, 4);
            human.AddHuman("Геннадий", 1999, 9, 12);
            human.AddHuman("Евгений", 1999, 1, 1);
            human.AddHuman("Андрей", 1999, 4, 23);

        }

        public static void Parser(ref Note human, int edit = -1)
        {
            bool works = true;

            while (works)
            {

                Console.WriteLine("\nВведите в формате [имя], [дата рождения] (Сергей, 10.09.2000), для выхода введите 0");
                string str = Console.ReadLine();
                try
                {
                    if (Convert.ToInt32(str) == 0)
                    {
                        break;
                    }
                }
                catch
                {

                    int count = str.ToCharArray().Where(c => c == ',').Count();
                    if (count != 1)
                    {
                        Console.Clear();
                        Console.WriteLine("\nНекорректный ввод по шаблону, для выхода введите 0");
                    }
                    else
                    {
                        string name = str.Split(',')[0];

                        if (name != "")
                        {
                            string date = str.Substring(str.IndexOf(' ') + 1);

                            try
                            {
                                DateTime convertDate = DateTime.Parse(date), nowDate = DateTime.Now;
                                if (nowDate.Year - convertDate.Year < 0)
                                {
                                    Console.Clear();
                                    Console.WriteLine("\nНекорректный ввод даты, для выхода введите 0");
                                }
                                else
                                {
                                    human.AddHuman(name, convertDate.Year, convertDate.Month, convertDate.Day, edit);
                                    works = false;
                                }

                            }
                            catch
                            {
                                Console.Clear();
                                Console.WriteLine("\nНекорректный ввод даты, для выхода введите 0");
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("\nНекорректный ввод имени");
                        }
                    }
                }
            }
        }

        static int isInt()
        {
            var count = Console.ReadLine();
            if (int.TryParse(count, out int intCount))
                return intCount;
            else
                return -1;
        }

        static void Menu()
        {
            bool works = true;




            Note human = new Note();

            if (!human.BdPath())
                return;

            human.LoadFromDB();
            human.SortPrint();


            while (works)
            {

                Console.WriteLine("\nВыберите пункт меню:\n1 - Ввести дату\n2 - Удалить элемент\n3 - Редактировать элемент\n4 - Посмотреть ближайшие дни рождения\n5 - Ввести тестовый набор данных\n0 - выйти");

                int intCount = isInt();
                if (intCount != -1)
                {
                    switch (intCount)
                    {
                        case 1:
                            Parser(ref human);
                            Console.Clear();
                            human.Print();
                            break;
                        case 2:
                            if (human.Len() != 0)
                            {
                                Console.Clear();
                                human.Print();
                                Console.WriteLine("\nВведите номер элемента, для выхода введите 0");
                                intCount = isInt();
                                if (intCount == 0)
                                {
                                    break;
                                }
                                else if (intCount != -1)
                                {

                                    human.DeleteDate(intCount, true);
                                    Console.Clear();
                                    human.Print();
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("\nошибка ввода");
                                }
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("\nНет элементов");
                            }
                            break;
                        case 3:
                            if (human.Len() != 0)
                            {
                                Console.Clear();
                                human.Print();
                                Console.WriteLine("\nВведите номер элемента, для выхода введите 0");
                                intCount = isInt();
                                if (intCount == 0)
                                {
                                    break;
                                }
                                else if (intCount != -1)
                                {

                                    if (human.DeleteDate(intCount))
                                    {
                                        Parser(ref human, intCount - 1);
                                        Console.Clear();
                                        human.Print();
                                    }


                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("\nОшибка ввода");
                                }
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("\nНет элементов");
                            }
                            break;
                        case 4:
                            Console.Clear();
                            human.SortPrint();
                            break;

                        case 5:
                            test(ref human);
                            Console.Clear();
                            human.Print();
                            break;
                        case 0:
                            works = false;
                            break;
                        default:
                            Console.Clear();

                            human.Print();

                            Console.WriteLine("\nПожалуйста проверьте вводимые данные");
                            break;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("\nПожалуйста проверьте вводимые данные");

                }
            }

        }

        static void Main(string[] args)
        {

            Menu();




            Console.Write("\nДля продолжения нажмите любую кнопку . . . ");
            Console.ReadKey(true);
        }

    }
}
