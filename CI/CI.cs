using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using BiomeLibrary;

namespace CI
{
    public class ConsoleInterface
    {
        //general usage variables
        public string root;

        //Bowls
        public Bowl variables = new Bowl(@"C:\\WinTools\Files\CI\BGDF\var.bgdf");
        public Bowl openFile = new Bowl(@"C:\WinTools\Files\CI\BGDF\openFile.bgdf");
        public Bowl settings = new Bowl(@"C:\WinTools\Files\CI\BGDF\settings.bgdf");
        public Bowl links = new Bowl(@"C:\WinTools\Files\CI\BGDF\linking.bgdf"); 


        //neccessary ToolBox Methods
        public void Setup()
        {
            if (!Directory.Exists(@"C:\WinTools\Files\CI"))
            {
                Directory.CreateDirectory(@"C:\WinTools\Files\CI");
                FileStream fs = File.Create(@"C:\WinTools\Files\CI\settings.bgdf");
                fs.Close();
                fs = File.Create(@"C:\WinTools\Files\CI\BGDF\lists.bgdf");
                fs.Close();
                fs = File.Create(@"C:\WinTools\Files\CI\BGDF\linking.bgdf");
                fs.Close();
                fs = File.Create(@"C:\WinTools\Files\CI\BGDF\var.bgdf");
                fs.Close();
                fs = File.Create(@"C:\WinTools\Files\CI\BGDF\settings.bgdf");
                fs.Close();
                fs = File.Create(@"C:\WinTools\Files\CI\BGDF\openFile.bgdf");
                fs.Close();
            }
        }
        public void Hub()
        {
            Next.Title("ConsoleInterface:");
            Next.Adv("type your command:");
            Main();
        }

        //method replacing the hub, just for ease of use idk (also, i just noticed that the root parameter is kinda unneccessary lmao)
        //primary function is file browser and opening other extensions

        public void Main()
        {
            Command cmd = new Command(Next.Cmd());

            switch (cmd.Array[0])
            {
                case "mount":
                    root = GetPhysicalPath(cmd.Array[1] + @"\");
                    Next.Adv("mounted Directory  " + root);
                    break;
                case "ci":
                    Handle(cmd.Array);
                    break;
                default:
                    //checks if the keyword is used for any extension
                    if (links.Exists(cmd.Array[0]))
                    {
                        SendHandle(cmd.Array, (string)links.Get(cmd.Array[0]));
                    }

                    switch(cmd.Array[1])
                    {
                        case "open":
                            if (File.Exists(GetPhysicalPath(cmd.Array[0], true)))
                            {
                                //makes the right extension handle the opening of the specified file after checking for its existence
                                if (openFile.Exists(GetFileExtension(GetPhysicalPath(cmd.Array[0], true))))
                                {
                                    SendHandle(cmd.Array, (string)openFile.Get(GetFileExtension(GetPhysicalPath(cmd.Array[0], true))));
                                }
                            }
                            break;
                    }
                    break;
            }
        }


        //General helping functions
        public string GetFileExtension(string path)
        {
            char[] chars = path.ToCharArray();
            int position = 0;
            for(int i = 0; i < chars.Length;i++)
            {
                if (chars[i] == '.')
                {
                    position = i;
                }
            }
            return path.Substring(position + 1);
        }

        //Extension Handling
        public void SendHandle(string[] cmd, string extensionName)
        {

        }

        //command handling
        public void Handle(string[] cmd)
        {
            switch (cmd[1])
            {
                case "print":
                    Next.Adv(cmd[2]);
                    break;
                case "link":
                    //LinkLibrary(cmd);
                    Next.Adv("created link");
                    break;
                case "try":
                    Console.WriteLine();
                    break;
                case "await":
                    StartExecutable(@"C:\WinTools", "await.exe");
                    Environment.Exit(0);
                    break;
                default:
                    Next.Err("command syntax seems to be wrong (error)");
                    break;
            }
        }
        //converts the command path into full length usable  physical path
        public string GetPhysicalPath(string input)
        {
            if (Directory.Exists(root + @"\" + input))
            {
                return root + @"\" + input;
            }
            else if (Directory.Exists(input))
            {
                return input;
            }
            else if (variables.Exists(input))
            {
                return (string)variables.Get(input);
            }
            return null;
        }
        public string GetPhysicalPath(string input, bool isFile)
        {
            if(isFile == false)
            {
                return GetPhysicalPath(input);
            }

            if(File.Exists(input))
            {
                return input;
            }
            else if(File.Exists(root + input))
            {
                return root + input;
            }
            else if(variables.Exists(input))
            {
                return (string)variables.Get(input);
            }
            return null;
        }
        public void ExecuteDLLMethod(string path, string methodName, string[] arg)
        {
            if (path == null)
            {
                Next.Err("extension could not be found");
            }
            Assembly lib = Assembly.LoadFile(path);
            System.Type type = lib.GetTypes()[0];
            var obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod(methodName);
            method.Invoke(obj, new object[] { arg, root });
        }

        //Executable handling
        public void StartExecutable(string path, string name)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = path + @"\" + name;
            startInfo.WorkingDirectory = path;
            Process process = Process.Start(startInfo);
        }

        //Tools and helping methods
        public string MergeStrings(string[] array, int startIndex)
        {
            string ret = "";
            for (int i = startIndex; i < array.Length; i++)
            {
                ret = ret + " " + array[i];
            }
            return ret;
        }
        public string[] GetCommand(string line)
        {
            char[] line_ch = line.ToCharArray();
            List<string> output = new List<string>();
            bool open = false;
            int last = 0;
            for (int i = 0; i < line_ch.Length; i++)
            {
                if (line_ch[i] == '"')
                {
                    if (open == true)
                    {
                        open = false;
                    }
                    else
                    {
                        open = true;
                    }
                }
                if (line_ch[i] == ' ' && !open)
                {
                    output.Add(Shorten(line.Substring(last, i - last)));
                    last = i;
                }
                else if (i == line_ch.Length - 1)
                {
                    output.Add(Shorten(line.Substring(last)));
                    last = i;
                }
            }
            return output.ToArray();
        }
        public string[] MadDirectories(string[] input, int refreshRate)
        {
            while (refreshRate > 0)
            {
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
            }
            return input;

        }
        public string[] MadJoin(string[] dir)
        {
            string[] all = dir;
            string[] temp;
            string[] w;
            for (int i = 0; i < dir.Length; i++)
            {
                w = Directory.GetFiles(dir[i]);
                temp = new string[all.Length + w.Length];
                all.CopyTo(temp, 0);
                w.CopyTo(temp, all.Length);
                all = temp;
            }
            return all;
        }
        public string[] ToArray(string input)
        {
            string[] output = new string[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = input.Substring(i, 1);
            }
            return output;
        }
        private string Shorten(string input)
        {
            while (input.Substring(0, 1) == " " || input.ToCharArray()[0] == '"')
            {
                input = input.Substring(1);
            }
            while (input.Substring(input.Length - 1) == " " || input.ToCharArray()[input.Length - 1] == '"')
            {
                input = input.Substring(0, input.Length - 1);
            }
            return input;
        }
        private string ShortenChar(string input, List<string> chars)
        {
            while (chars.Contains(input.Substring(0, 1)))
            {
                input = input.Substring(1);
            }
            while (chars.Contains(input.Substring(input.Length - 1)))
            {
                input = input.Substring(0, input.Length - 1);
            }
            return input;
        }
    }


        //Helping classes (not necessarily for this, for console commands in general) also, its on GitHub: 
        //Except for this one, this one is just for CI
        public enum SessionType { File, Directory, Default, List, External, CI }

        //these are not
        public class Next
        {
            public static string Cmd()
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("cmd>    ");
                Console.ForegroundColor = ConsoleColor.White;
                return Console.ReadLine();
            }
            public static string Arg()
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("arg>    ");
                Console.ForegroundColor = ConsoleColor.White;
                return Console.ReadLine();
            }
            public static void Title(string text)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(text);
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.White;
            }
            public static void Adv(string text)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(text);
                Console.ForegroundColor = ConsoleColor.White;
            }

            public static void Err(string msg)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.White;
            }

            public static void List(string[] input)
            {
                Console.WriteLine();
                if (input != null)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    foreach (string i in input)
                    {
                        Console.WriteLine(i);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("none");
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }
        }
        public class Command
        {
            public string Cmd { get; set; }
            public string Arg { get; set; }
            public string Post { get; set; }
            public string[] Array { get; set; }

            char sign = "'".ToCharArray()[0];


            public Command(string line)
            {
                char[] temp = line.ToCharArray();
                bool ext = false;
                bool done = false;
                int ext_start = 0;
                char sign = "'".ToCharArray()[0];
                int sign1 = -16;
                int sign2 = -16;
                int space = 0;
                ConsoleInterface ci = new ConsoleInterface();
                Array = ci.GetCommand(line);
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i] == sign)
                    {
                        if (sign1 != 0)
                        {
                            sign1 = i;
                        }
                        else if (sign2 != 0) //makes the program pick the second sign instead of the last one
                        {
                            sign2 = i;
                        }
                    }
                    else if (temp[i] == ' ' && done == false && sign1 == -16)//string has not been opened
                    {
                        space = i;
                        done = true;
                    }
                    else if (temp[i] == ' ' && done == false && sign2 != -16)//string has been closed
                    {
                        space = i;
                        done = true;
                    }
                }
                if (space != 0)
                {
                    Cmd = line.Substring(0, space);
                    if (ext == true)
                    {
                        Arg = line.Substring(space, ext_start - space);
                        Post = line.Substring(ext_start + 1);
                    }
                    else
                    {
                        Arg = line.Substring(space);
                        Post = null;
                    }
                    if (sign2 != -16)
                    {
                        Cmd = line.Substring(0, Cmd.Length - 1);
                    }
                    Cmd = Shorten(Cmd);
                    if (Arg != null)
                    {
                        Arg = Shorten(Arg);
                    }
                    if (Post != null)
                    {
                        Post = Shorten(Post);
                    }
                }
                else
                {
                    Cmd = Shorten(line);
                }
            }

            private string Shorten(string input)
            {
                while (input.Substring(0, 1) == " " || input.Substring(0, 1) == sign.ToString())
                {
                    input = input.Substring(1);
                }
                while (input.Substring(input.Length - 1) == " " || input.Substring(0, 1) == sign.ToString())
                {
                    input = input.Substring(0, input.Length - 1);
                }
                return input;
            }
        }
        public class Settings
        {
            public string PreRoot { get; set; }
            public string[] RootSaves { get; set; }
            public string[] PathMemory { get; set; }

            public Settings(string preRoot, string[] rootSaves, string[] pathMemory)
            {
                PreRoot = preRoot;
                RootSaves = rootSaves;
                PathMemory = pathMemory;
            }

            public Settings Default = new Settings(null, null, null);

            public Settings loadSettings()
            {
                return new Settings(null, null, null);
            }
        }
    }