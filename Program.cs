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
            int num;
            while (res)
            {
                Console.Clear();
                int scene_id = SceneConstant.scene_id;

                if (scene_id != 0)
                {
                    var scene = TheatreDB.getBySceneId(scene_id);
                    Console.WriteLine("Vybrané představení: " + scene[0].name);
                } else
                {
                    Console.WriteLine("Představení není vybrané"); 
                }


                Console.WriteLine("[1] Zobrazení rezervací \n [2] Vytvořit rezervaci \n [3] Zobrazení uživatelských rezervací \n [4] Zobrazení Konfiguračního souboru \n [5] Výběr představení \n [6] Změnit počet řad \n [7] Změnit počet sedadel");
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
                        scene_id = TheatreCheck.numberCheckByScene();
                        SceneConstant.scene_id = scene_id;
                        break;
                    case 6:
                        num = TheatreCheck.numberCheck();
                        c.changeTheatreConfigY(num);
                        c.save();
                        Console.WriteLine("Pro navrácení do menu. Stikněte jakoukoli klávesu");
                        Console.ReadKey();
                        break;
                    case 7:
                        num = TheatreCheck.numberCheck();
                        c.changeTheatreConfigX(num);
                        c.save();
                        Console.WriteLine("Pro navrácení do menu. Stikněte jakoukoli klávesu");
                        Console.ReadKey();
                        break;

                        /*case 6:
                            TheatreDB.insertScene("Kocka a dum");
                            Console.WriteLine("Pro navrácení do menu. Stikněte jakoukoli klávesu");
                            Console.ReadKey();
                            break;*/

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
            public void getBasicPicker(object[] Arr)
            {
                int x = 0;
                foreach (var item in Arr)
                {
                    Console.WriteLine("[" + x + "] " + item);
                    x++;
                }
            }

            public void getScenesTable()
            {
                var db = new TheatreDB();
                var scenes = db.getAllScenes();

                foreach (var item in scenes)
                {
                    Console.WriteLine("[" + item.id + "] " + item.name);
                }
            } 
            public void ShowReservationTable()
            {

                var c = new TheatreConfig();
                var conf = c.conf;

                var TheatreDB = new TheatreDB();
                string table = "";
                bool isTheSame = false;

                for (int i = 1; i < conf.x+1; i++)
                {
                   
                    
                    for (int j = 1; j < conf.y+1; j++)
                    {
                        isTheSame = false;


                        foreach (var a in conf.issue)
                        {
                            if (a.x == j && a.y == i)
                            {
                                table += "T";
                                isTheSame = true;
                            }
                        }

                        if (isTheSame == false)
                        {
                            foreach (var item in TheatreDB.getAllBySceneId(SceneConstant.scene_id))
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
                        
                    }
                    table += "\n";
                }
                Console.WriteLine(table);

            }

            public void ShowReservation()
            {
                var theatreDB = new TheatreDB();
                Console.WriteLine("Rezervace");
                Console.WriteLine("-------------");
                Console.Write("Email | Jméno | Sloupec | Řada | Představení\n");
                Console.WriteLine("-------------");
                foreach (var item in theatreDB.getAllBySceneId(SceneConstant.scene_id))
                {
   
                    var scene = theatreDB.getBySceneId(item.scene_id)[0].name;
                    Console.Write(item.email + " | " + item.name + " | " + item.x + " | " + item.y +" | "+ scene + "\n");
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


                if (theatreDB.InsertNewReservation(x, y, name, email, SceneConstant.scene_id))
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


        /* 
         Input check

         */
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

            public int numberCheckByScene()
            {
                string input;
                bool ok = true;
                int result = 0;

                var c = new TheatreConfig();
                var component = new TheatreComponent();
                var db = new TheatreDB();
                var conf = c.conf;

                while (ok == true)
                {
                    do
                    {

                        component.getScenesTable();
                        Console.Write("Vyberte číslo přestavení: ");
                        input = Console.ReadLine();

                    } while (!int.TryParse(input, out result));

                    var scene = db.getBySceneId(result);
                    if (scene.Count != 0)
                    {
                        ok = false;
                    }

                }
                return result;
            }

            public string nameCheck()
            {
                string input;

                Console.Write("Napište jméno: ");
                input = Console.ReadLine();

                return input;
            }

          
            public int numberCheck()
            {
                int result = 0;
                string input;
                do
                {
                    Console.Write("Vyberte číslo: ");
                    input = Console.ReadLine();

                } while (!int.TryParse(input, out result));
                return result;
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
                        Console.WriteLine("Rozmezí 0-" + conf.x);
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
                        Console.WriteLine("Rozmezí 0-" + conf.y);
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

        /*
         Database methods
         */
        public class TheatreDB
        {

            /* DB */
            public SQLiteConnection db;


            /* Constructor */
            public TheatreDB()
            {
                db = new SQLiteConnection("./data.db");
            }

            /* Get all from theatre */
            public List<Theatre> getAll()
                                
            {
                var result = db.Query<Theatre>("SELECT * FROM Theatre");
                return result;
            }

            /* getAllBySceneId */
            public List<Theatre> getAllBySceneId(int scene_id)

            {
                var result = db.Query<Theatre>("SELECT * FROM Theatre WHERE scene_id = ?", scene_id);
                return result;
            }

            /* getBySceneId */
            public List<Scene> getBySceneId(int scene_id)

            {
                var result = db.Query<Scene>("SELECT * FROM Scene WHERE id = ?", scene_id);
                return result;
            }


            /* getAllScenes */
            public List<Scene> getAllScenes()

            {
                var result = db.Query<Scene>("SELECT * FROM Scene");
                return result;
            }


            /* insertScene */
            public bool insertScene(string name)

            {
                var _Scene = new Scene();
                _Scene.name = name;
                var result = db.Insert(_Scene);
                if (result != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void createTable()
            {

                db.CreateTable<Theatre>();
                db.CreateTable<Scene>();
            }


            public bool InsertNewReservation(int x, int y,string name ,string email, int scene_id)
            {
                var _Theatre = new Theatre();
                _Theatre.x = x;
                _Theatre.y = y;
                _Theatre.name = name;
                _Theatre.email = email;
                _Theatre.scene_id = scene_id;
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


        /* Database Objects*/
        public class Theatre
        {
            [PrimaryKey, AutoIncrement]
            public int id { get; set; }

            public int x { get; set; }
          
            public int y { get; set; }

            public string name { get; set; }

            public string email { get; set; }

            public int scene_id { get; set; }
        }


        public class Scene
        {
            [PrimaryKey, AutoIncrement]
            public int id { get; set; }

            public string name { get; set; }
        }
    

        /* Config.json Objects */

        public class ConfigurationFile
        {
            public int x;

            public int y;

            public object[] error;

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

        static class SceneConstant
        {
            public static int scene_id;
        }


    }

   

}
