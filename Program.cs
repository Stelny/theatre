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

            var c = new TheatreConfig();
            var conf = c.conf;

            var TheatreCheck = new TheatreCheck();

            var TheatreComponent = new TheatreComponent();

            bool res = true;
            while (res)
            {
                Console.Clear();

                Console.WriteLine("[1] Zobrazení rezervací \n [2] Vytvořit rezervaci \n [3] Zobrazení uživatelských rezervací \n [4] Zobrazení Konfiguračního souboru");
                int menuPicker = TheatreCheck.MenuPicker();
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
                    case 5:
                        var testik = new TheatreConfig();

                        Console.WriteLine("Pro navrácení do menu. Stikněte jakoukoli klávesu");
                        testik.insertTheatreConfigIssue(12,7,"Nema nic");
                        testik.save();
                        Console.ReadKey();
                        
                        break;
                }
            }
        }

        public class TheatreConfig 
        {
            public ConfigurationFile conf;
            public string url;
            public TheatreConfig()
            {
                url = @"./config.json";

                string json = File.ReadAllText(url);

                conf = JsonConvert.DeserializeObject<ConfigurationFile>(json);


            }

            public void changeTheatreConfig()
            {

                Console.WriteLine(conf.x);
                
                /* ConfigurationFile a = new ConfigurationFile(10, 10);
                a.x = 10;
                Console.WriteLine(a.x);

                Issues b = new Issues();
                b.text = "testik";
                a.issue.Add(b);

                Console.WriteLine(a.issue[0].text);*/
                
            }
            /*
            public void insertTheatreConfig()
            {

                Issues b = new Issues();
                b.text = "Test";
                conf.issue.Add(b);

                foreach (var item in conf.issue)
                {
                    Console.WriteLine(item.x);
                }
            } */
            public void changeTheatreConfigX(int x)
            {
                conf.x = x;
            }
            public void changeTheatreConfigY(int y)
            {
                conf.y = y;
            }

            protected int getLastid()
            {
                int countOfColumns  = conf.issue.Count;

                return conf.issue[countOfColumns-1].id;
                
            }
            public void insertTheatreConfigIssue(int x, int y, string text)
            {
                int id = getLastid();
                Issues b = new Issues();
                b.id = id+1;
                b.text = text;
                b.x = x;
                b.y = y;
                conf.issue.Add(b);
            }

            public void save()
            {

                string json = JsonConvert.SerializeObject(conf);

                File.WriteAllText(url, json);
                

                
            }
            public bool deleteTheatreConfigById(int id)
            {
                if (id == 0)
                {
                    return false;
                } else
                {
                    return true;
                }
            }


        }
        public class TheatreComponent
        {
            public void ShowReservationTable()
            {

                var c = new TheatreConfig();
                var conf = c.conf;

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
                var c = new TheatreConfig();
                var conf = c.conf;

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

                var c = new TheatreConfig();
                var conf = c.conf;

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

                var c = new TheatreConfig();
                var conf = c.conf;

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
            public SQLiteConnection db;

            public TheatreDB()
            {
                db = new SQLiteConnection("./data.db");
            }
            public List<Theatre> getAll()
                                
            {
                var result = db.Query<Theatre>("SELECT * FROM Theatre");
                return result;
            }



            public void createTable()
            {

                db.CreateTable<Theatre>();
            }


            public bool InsertNewReservation(int x, int y,string name ,string email)
            {
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
            public int x;

            public int y;

            public List<Issues> issue;

            public ConfigurationFile(int cX, int cY)
            {
                x = cX;
                y = cY;
                issue = new List<Issues>();
            }

           
        } 
        public class Issues
            {
                
                public int x { get; set; }

                public int y { get; set; }

                public int id;
                public string text { get; set; }
            }
    }

   

}
