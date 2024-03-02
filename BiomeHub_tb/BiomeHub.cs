using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;

namespace TestConsole
{

    public class BiomeHub
    {
        public static string memory = null;
        public string filePath = @"C:\\WinTools\Files\BiomeHub\Files\GameSave.bgdf";
        public static string Bowlpath;
        public static void Hub()
        {
            Console.WriteLine("BiomeHub Main Hub:");
            Console.WriteLine("type 'help' for a command list");
            string command = Console.ReadLine();
            string[] single = { @"D:\BiomeCo" };

            switch (command)
            {
                case "save-application":
                    SaveApplication();
                    break;
                case "save-app":
                    SaveApplication();
                    break;
                case "show-apps":
                    ShowGames();
                    break;
                case "start":
                    StartApplication();
                    break;
                case "help":
                    Console.WriteLine("commands:");
                    Console.WriteLine("'save-application' to save a new game or tool to your library");
                    Console.WriteLine("'show-apps' to show the library");
                    Console.WriteLine("'start' to start a saved application");
                    break;
                case "wipe-games":
                    Console.WriteLine("are you sure? (y/n)");
                    if (Console.ReadLine() == "y")
                    {
                        File.Delete(@"C:\\WinTools\Files\BiomeHub\Files\GameSave.bgdf");
                        FileStream fs = File.Create(@"C:\\WinTools\Files\BiomeHub\Files\GameSave.bgdf");
                        fs.Close();
                    }
                    else
                    {
                        Hub();
                    }
                    break;
                case "change-bowl-library":
                    Console.WriteLine("type name of new lib (with '.dll' at the end");
                    using (FileStream fileStream = new FileStream(@"C:\\WinTools\Files\BiomeHub\Files\bowl.txt", FileMode.Create))
                    {
                        using (StreamWriter writer = new StreamWriter(fileStream))
                        {
                            writer.Write(Console.ReadLine());
                        }
                    }
                    Console.WriteLine("");
                    break;
                default:
                    Hub();
                    break;

            }

            Hub();
        }

        public static void Setup()
        {
            Directory.CreateDirectory(@"C:\\WinTools\Files\BiomeHub");
            Directory.CreateDirectory(@"C:\\WinTools\Files\BiomeHub\Games");
            Directory.CreateDirectory(@"C:\\WinTools\Files\BiomeHub\Files");
            using (FileStream fileStream = new FileStream(@"C:\\WinTools\Files\BiomeHub\Files\GameSave.bgdf", FileMode.Create))
            {

            }
            using (FileStream fileStream = new FileStream(@"C:\\WinTools\Files\BiomeHub\Files\bowl.txt", FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    writer.Write("BowlWt");
                }
            }
            return;
        }

        public static void Handle(string[] input, string root)
        {
            string input_s = "";
            for(int i = 2; i < input.Length; i++)
            {
                input_s = input_s + " " + input[i];
            }
            input_s = input_s.Substring(1);

            switch (input[1])
            {
                case "save":
                    SaveApplication();
                    break;
                case "start":
                    if (input.Length == 2)
                    {
                        StartApplication();
                    }
                    else if (input.Length > 2)
                    {
                        try
                        {
                            ProcessStartInfo startInfo = new ProcessStartInfo();
                            startInfo.FileName = (ReadSave(input_s));
                            startInfo.WorkingDirectory = Path.GetDirectoryName(ReadSave(input_s));
                            Process process = Process.Start(startInfo);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("executable/application could not be found");
                        }
                    }
                    break;
                case "show":
                    ShowGames();
                    break;
                case "saven":
                    if (input[2].Substring(0,1) == "@")
                    {
                        //no naming
                        if (input_s.Substring(input_s.Length - 4) == ".zip")
                        {
                            GetGameFromZip(CombineStrings(input, 2).Substring(1));
                        }
                        else
                        {
                            if (!Directory.Exists(CombineStrings(input, 2).Substring(1))) { Console.WriteLine("directory not found."); return; }
                            GetGameFromDir(CombineStrings(input, 2).Substring(1));
                        }
                    }
                    else
                    {
                        WriteSave(input[2], CombineStrings(input, 3));
                    }
                    break;
                default:
                    Console.WriteLine("command not found in dll");
                    break;
            }
        }
        private static string CombineStrings(string[] array, int startIndex)
        {
            string output = " ";
            for(int i = startIndex; i < array.Length; i++)
            {
                output = output + array[i];
            }
            return output.Substring(1);
        }
        public static void RemoveSave(string name)
        {
            Assembly lib = Assembly.LoadFile(@"C:\\WinTools\Extensions\supp\Bowl.dll");
            Type type = lib.GetTypes().FirstOrDefault();

            if (type != null)
            {
                var obj = Activator.CreateInstance(type);
                MethodInfo method = type.GetMethod("Remove");
                method.Invoke(obj, new object[] { name, @"C:\\WinTools\Files\BiomeHub\Files\GameSave.bgdf" });
            }
            return;
        }
        public static void WriteSave(string name, string path)
        {
            Assembly lib = Assembly.LoadFile(@"C:\\WinTools\Extensions\supp\Bowl.dll");
            Type type = lib.GetTypes().FirstOrDefault();

            if (type != null)
            {
                var obj = Activator.CreateInstance(type);
                MethodInfo method = type.GetMethod("SaveString");
                method.Invoke(obj, new object[] { path, name, @"C:\\WinTools\Files\BiomeHub\Files\GameSave.bgdf" });
            }
            return;
        }
        public static string ReadSave(string name)
        {
            Assembly lib = Assembly.LoadFile(@"C:\\WinTools\Extensions\supp\Bowl.dll");
            Type type = lib.GetTypes().FirstOrDefault();
            var obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("ReadString");
            object h = method.Invoke(obj, new object[] { name, @"C:\\WinTools\Files\BiomeHub\Files\GameSave.bgdf" });
            return h.ToString();
        }
        public static string[] ShowAll(string path)
        {
            string[] u;
            Assembly lib = Assembly.LoadFile(@"C:\WinTools\Extensions\supp\Bowl.dll");
            Type type = lib.GetTypes()[0];

            if (type != null)
            {
                var obj = Activator.CreateInstance(type);
                MethodInfo method = type.GetMethod("GetAllNames");
                u = (string[])method.Invoke(obj, new object[] { path });
            }
            else
            {
                u = new string[0];
            }
            for (int i = 0; i < u.Length; i++)
            {
                Console.WriteLine("[" + i + "]   " + u[i]);
            }
            Console.WriteLine();
            return u;

        }
        public static void SaveApplication()
        {
            Console.WriteLine("Type zip to save from zip, dir to do from directory");
            Console.WriteLine("type command:");
            Console.WriteLine("");
            string command = Console.ReadLine();
            Console.WriteLine("type path:");
            Console.WriteLine("");
            string path = Console.ReadLine();
            switch (command)
            {
                case "zip":
                    GetGameFromZip(path);
                    break;
                case "dir":
                    GetGameFromDir(path);
                    break;
                default:
                    Console.WriteLine("going to main now. Unexpected command.");
                    Hub();
                    break;
            }
            return;
        }

        public static void GetGameFromDir(string path)
        {
            string exe = null;
            string directoryName = GetFileSystemEntryName(path);
            string[] lone = { path };
            string[] directories = lone.Concat(Directory.GetDirectories(path)).ToArray();
            directories = Directories(directories, 10);
            for (int i = 0; i < directories.Length; i++)
            {
                string directory = directories[i];
                string[] files = Directory.GetFiles(directory);

                for (int j = 0; j < files.Length; j++)
                {
                    string file = files[j];
                    string file_type = GetFileType(file);
                    if (file_type == "exe" && GetFileSystemEntryName(file) != "UnityCrashHandler64.exe" && GetFileSystemEntryName(file) != "UE4PrereqSetup_x64.exe")
                    {
                        exe = file;
                        break;
                    }
                }
                if (exe != null) { break; }
            }
            memory = exe;
            Console.WriteLine(exe);
            if (exe == null || !File.Exists(exe)) { Console.WriteLine("fail"); }
            WriteSave(directoryName, exe);
            Console.WriteLine("done. Added to you library");
            return;
        }

        public static void GetGameFromZip(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("file not found");
                return;
            }
            string directoryName = GetFileSystemEntryName(path).Substring(0, GetFileSystemEntryName(path).Length - 4);
            Console.WriteLine(directoryName);
            string directoryPath = @"C:\\WinTools\Files\BiomeHub\Games\" + directoryName;
            string exe = String.Empty;
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
            Console.WriteLine("extracting");
            ZipFile.ExtractToDirectory(path, directoryPath);
            Console.WriteLine("done");
            string[] lone = { directoryPath };
            string[] directories = lone.Concat(Directory.GetDirectories(directoryPath)).ToArray();
            directories = Directories(directories, 10);
            for (int i = 0; i < directories.Length; i++)
            {
                string directory = directories[i];
                string[] files = Directory.GetFiles(directory);

                for (int j = 0; j < files.Length; j++)
                {
                    string file = files[j];
                    string file_type = GetFileType(file);
                    if (file_type == "exe" && GetFileSystemEntryName(file) != "UnityCrashHandler64.exe" && GetFileSystemEntryName(file) != "UnityCrashHandler32.exe" && GetFileSystemEntryName(file) != "UE4PrereqSetup_x64.exe")
                    {
                        exe = file;
                        break;
                    }
                }
                if (exe != String.Empty) { break; }
            }
            memory = exe;
            Console.WriteLine(exe);
            if (exe == null || !File.Exists(exe)) { Console.WriteLine("fail"); }
            WriteSave(directoryName, exe);
            return;
        }

        public static void DeleteGame()
        {
            string[] w = ShowAll(@"C:\\WinTools\Files\BiomeHub\Files\GameSave.bgdf");
            Console.WriteLine("Type the index of the application you want to delete:");
            int index = Convert.ToInt32(Console.ReadLine());
            if (index < 0) { Hub(); }
            Assembly lib = Assembly.LoadFile(@"C:\\WinTools\Extensions\supp\Bowl.dll");
            Type type = lib.GetTypes().FirstOrDefault();

            if (type != null)
            {
                var obj = Activator.CreateInstance(type);
                MethodInfo method = type.GetMethod("Remove");
                method.Invoke(obj, new object[] { w[index], @"C:\\WinTools\Files\BiomeHub\Files\GameSave.bgdf" });
            }
            Console.WriteLine("done");
            Hub();

        }

        public static void ShowGames()
        {
            string[] gamesArray = ShowAll(@"C:\\WinTools\Files\BiomeHub\Files\GameSave.bgdf");
            Console.WriteLine("Type rename-app if you would like to rename an app. Type delete-app to delete an app. Type -1 to exit to the Main Hub.");
            string action = Console.ReadLine();
            switch (action)
            {
                case "rename-app":
                    Console.WriteLine("type the index of the Game you want to change the name of:");
                    int index = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("type a new name for the game:");
                    string newName = Console.ReadLine();
                    string path = ReadSave(gamesArray[index]);
                    Console.WriteLine("saving...");
                    Console.WriteLine(gamesArray[index]);
                    RemoveSave(gamesArray[index]);
                    WriteSave(newName, path);
                    Console.WriteLine("saved");
                    break;
                case "delete-app":
                    DeleteGame();
                    break;
                case "-1":
                    Hub();
                    break;
                default:
                    Hub();
                    break;
            }
        }

        public static void StartApplication()
        {
            Console.WriteLine("start-app menu");
            string[] w = ShowAll(@"C:\\WinTools\Files\BiomeHub\Files\GameSave.bgdf");
            Console.WriteLine("type the index of the application you want to start:");
            int index = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("starting...");
            if (index < 0 || index >= w.Length)
            {
                Hub();
            }
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = (ReadSave(w[index]));
            startInfo.WorkingDirectory = Path.GetDirectoryName(ReadSave(w[index]));
            Process bioshock2 = Process.Start(startInfo);
            Environment.Exit(0);
        }
        public static string GetFileSystemEntryName(string path)
        {
            int pos = 0;
            string sub;
            for (int i = 0; i < path.Length; i++)
            {
                sub = path.Substring(i, 1);
                if (sub == @"\" || sub == @"/")
                {
                    pos = i;
                }
            }
            pos++;
            string output = path.Substring(pos);
            BiomeHub.memory = output;
            return output;
        }
        public static string[] Directories(string[] input, int refreshRate)
        {
            if (refreshRate == 0)
            {
                return input;
            }
            string[] memory;
            string[] input_bu;
            for (int i = 0; i < input.Length; i++)
            {
                memory = Directory.GetDirectories(input[i]);

                for (int j = 0; j < memory.Length; j++)
                {
                    if (!input.Contains(memory[j]) && input != null)
                    {
                        input_bu = input;
                        string[] lsd = { memory[j] };
                        input = new string[input.Length + 1];
                        input = input_bu.Concat(lsd).ToArray();
                    }
                }
            }
            refreshRate--;
            if (refreshRate != 0)
            {
                return Directories(input, refreshRate);
            }
            else
            {
                return input;
            }

        }
        public static string GetFileType(string fileName)
        {
            string output;
            output = fileName.Substring(GetLastIndexOfSign(fileName, "."), 3);
            BiomeHub.memory = output;
            return output;
        }
        public static int GetLastIndexOfSign(string input, string sign)
        {
            int index = -1;
            for (int i = 0; i < input.Length; i++)
            {
                if (input.Substring(i, 1) == sign)
                {
                    index = i;
                }
            }
            index++;
            BiomeHub.memory = index.ToString();
            return index;
        }

    }
}
