using SQLite;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace newTheatre
{
    class Program
    {
        static void Main(string[] args)
        {
            var TheatreDB = new TheatreDB();
            TheatreDB.createTable();

            string url = @"./config.json";

            string json = File.ReadAllText(url);

            var config = JsonConvert.DeserializeObject<ConfigurationFile>(json);

            var TheatreCheck = new TheatreCheck();

            var TheatreComponent = new TheatreComponent();

            bool res = true;
            while (res)
            {
                Console.Clear();
                int menuPicker = TheatreCheck.MenuPicker();
                Console.WriteLine("[1] Zobrazení rezervací \n [2] Vytvořit rezervaci \n [3] Zobrazení uživatelských rezervací \n [4] Zobrazení Konfiguračního souboru");
                switch (menuPicker)
                {
                    case 1:
                        TheatreComponent.ShowReservationTable();
                        Console.WriteLine("Pro navrácení do menu. Stikněte jakoukoli klávesu");
                        Console.ReadKey();
                        break;
                    case 2:
                        TheatreComponent.ShowReservationTable();
                        TheatreComponent.CreateReservation();
                        Console.WriteLine("Pro navrácení do menu. Stikněte jakoukoli klávesu");
                        Console.ReadKey();
                        break;
                    case 3:
                        TheatreComponent.ShowReservation();
                        Console.WriteLine("Pro navrácení do menu. Stikněte jakoukoli klávesu");
                        Console.ReadKey();
                        break;
                    case 4:
                        TheatreComponent.showConfig();
                        Console.WriteLine("Pro navrácení do menu. Stikněte jakoukoli klávesu");
                        Console.ReadKey();
                        break;

                }
            }
        }


        public class TheatreComponent
        {
            public void ShowReservationTable()
            {
                string url = @"./config.json";

                string json = File.ReadAllText(url);

                var conf = JsonConvert.DeserializeObject<ConfigurationFile>(json);
                var TheatreDB = new TheatreDB();
                string table = "";
                bool isTheSame = false;

                for (int i = 1; i < conf.x+1; i++)
                {
                    
                    table += "O";
                    
                    for (int j = 1; j < conf.y+1; j++)
                    {
                        isTheSame = false;
                        foreach (var item in TheatreDB.getAll())
                        {
                            
                            if (item.x == j && item.y == i)
                            {
                                table += "X";
                                isTheSame = true;
                                
                            }
                        }
                        if (isTheSame == false)
                        {
                            table += "O";
                        }
                        
                    }
                    table += "\n";
                }
                Console.WriteLine(table);

            }

            public void ShowReservation()
            {
                var theatreDB = new TheatreDB();
                foreach (var item in theatreDB.getAll())
                {
                    Console.WriteLine("Rezervace");
                    Console.WriteLine("-------------");
                    Console.Write("Email | Jméno | Sloupec | Řada\n");
                    Console.WriteLine("-------------");
                    Console.Write(item.email + " | " + item.name + " | " + item.x + " | " + item.y + "\n");
                    Console.WriteLine("-------------");
                }
            }

            public void CreateReservation()
            {
                var theatreCheck = new TheatreCheck();
                var theatreDB = new TheatreDB();

                int x = theatreCheck.numberCheckByLimitX();

                int y = theatreCheck.numberCheckByLimitY();

                string email = theatreCheck.emailCheck();

                string name = theatreCheck.nameCheck();

                if (theatreDB.InsertNewReservation(x, y, name, email))
                {
                    Console.WriteLine("Vaše rezervace byla úspěšně přidána!");
                } else
                {
                    Console.WriteLine("Někde se stala chyba");
                }
            }
            public void showConfig()
            {
                string url = @"./config.json";

                string json = File.ReadAllText(url);

                var conf = JsonConvert.DeserializeObject<ConfigurationFile>(json);

                Console.WriteLine("Počet sloupců: " + conf.x);
                Console.WriteLine("Počet řad: " + conf.y);
                Console.WriteLine("Rezervace");
                Console.WriteLine("------------");
                Console.WriteLine("řada | sloupec | Text");
                Console.WriteLine("------------");
                foreach (var item in conf.issue)
                {
                    Console.WriteLine(item.y + " | " + item.x + " | " + item.text);
                }
            }
        }
        public class TheatreCheck
        {
            public int MenuPicker()
            {
                string input;
                int result;
                do
                {
                    Console.Write("Zvolte si z menu: ");
                    input = Console.ReadLine();

                } while (!int.TryParse(input, out result));

                return result;
            }

            public string nameCheck()
            {
                string input;

                Console.Write("Napište jméno: ");
                input = Console.ReadLine();

                return input;
            }

            public int numberCheckByLimitX()
            {
                string input;
                int result = 5;
                bool ok = true;

                string url = @"./config.json";

                string json = File.ReadAllText(url);

                var conf = JsonConvert.DeserializeObject<ConfigurationFile>(json);

                while (ok == true)
                {
                    do
                    {
                        Console.WriteLine("Sloupec");
                        Console.WriteLine("Rozmezí 1-" + conf.x);
                        Console.Write("Vyberte číslo: ");
                        input = Console.ReadLine();

                    } while (!int.TryParse(input, out result));

                    if (result <= conf.x)
                    {
                        ok = false;
                        return result;
                    }

                }
                return result;
                
            }

            bool IsValidEmail(string strIn)
            {
                // Return true if strIn is in valid e-mail format.
                return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            }
            public string emailCheck()
            {
                string input;
                do
                {
                    Console.Clear();
                    Console.WriteLine("Email: ");
                    input = Console.ReadLine();

                } while (!IsValidEmail(input));

                return input;
            }
            public int numberCheckByLimitY()
            {
                string input;
                int result = 0;

                string url = @"./config.json";

                string json = File.ReadAllText(url);

                var conf = JsonConvert.DeserializeObject<ConfigurationFile>(json);

                bool ok = true;
                while (ok == true)
                {
                    do
                    {
                        Console.WriteLine("Řada");
                        Console.WriteLine("Rozmezí 1-" + conf.y);
                        Console.Write("Vyberte číslo: ");
                        input = Console.ReadLine();

                    } while (!int.TryParse(input, out result));

                    if (result <= conf.x)
                    {
                        ok = false;
                        return result;
                    }
                }
                return result;
            }
        }
        public class TheatreDB
        {
            public List<Theatre> getAll()
                                
            {
                var db = new SQLiteConnection("./data.db");
                var result = db.Query<Theatre>("SELECT * FROM Theatre");
                return result;
            }



            public void createTable()
            {
                var db = new SQLiteConnection("./data.db");

                db.CreateTable<Theatre>();
            }


            public bool InsertNewReservation(int x, int y,string name ,string email)
            {
                var db = new SQLiteConnection("./data.db");
                var _Theatre = new Theatre();
                _Theatre.x = x;
                _Theatre.y = y;
                _Theatre.name = name;
                _Theatre.email = email;
                var result = db.Insert(_Theatre);
                if (result != 0)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }

        public class Theatre
        {
            [PrimaryKey, AutoIncrement]
            public int id { get; set; }

            public int x { get; set; }
          
            public int y { get; set; }

            public string name { get; set; }

            public string email { get; set; }
        }

        public class ConfigurationFile
        {
            public int x { get; set; }

            public int y { get; set; }

            public List<issues> issue { get; set; }

            public class issues
            {
                public int x { get; set; }

                public int y { get; set; }

                public string text { get; set; }
            }
        }
    }

   

}
