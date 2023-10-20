using System;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Lib
{
    class VersionControl
    {
        public string bgdfController = @"C:\WinTools\Extensions\Bowl_tb.dll";
        public string projFolder = @"C:\WinTools\Files\VersionControl\proj\";
        public const string projBowl = @"C:\\WinTools\Files\VersionControl\Table\proj.bgdf";
        string root = @"C:\\WinTools\Files\VersionControl\proj\";
        string currentDir = null;
        public Next next = new Next();
        public void Setup()
        {
            next.Title("VersionControl Setup");
            if(!Directory.Exists(@"C:\\WinTools\Files\VersionControl"))
            {
                Directory.CreateDirectory(@"C:\\WinTools\Files\VersionControl");
                Directory.CreateDirectory(@"C:\\WinTools\Files\VersionControl\proj");
                Directory.CreateDirectory(@"C:\\WinTools\Files\VersionControl\Table");
                FileStream fs = File.Create(@"C:\\WinTools\Files\VersionControl\Table\proj.bgdf");
                fs.Close();
                fs = File.Create(@"C:\\WinTools\Files\VersionControl\Table\set.bgdf");
                fs.Close();
            }
            if(!File.Exists(bgdfController))
            {
                next.Err("Bowl-Extension not found. If you changed the name, the library cannot be accessed, if it does not exist, please download from the official download site");
                throw new FileNotFoundException();
            }
            next.Adv("type the location of the default location for project folders. Type '/def' to use the default Directory which is:");
            next.Adv(@"C:\WinTools\Files\VersionControll\proj");
            string path = Shorten(next.Arg());
            if(path == "/def")
            {
                projFolder = @"C:\WinTools\Files\VersionControl\proj";
                next.Adv("folder set");
            }
            else if(Directory.Exists(path))
            {
                projFolder = path;
                next.Adv("folder set");
            }
            else
            {
                next.Err("the specified folder does not exist");
                Setup();
                return;
            }
            WriteSave("projectRoot", path, @"C:\\WinTools\Files\VersionControl\Table\set.bgdf");
            next.Adv("all set");

        }
        public void Hub()
        {
            next.Title("VersionControl Main Hub:");
            next.Adv("type command. type '?' for help (a list of commands)");
            Console.WriteLine();
            Command cmd = new Command(next.Cmd());
            switch(cmd.Cmd)
            {
                case "?":
                    Console.WriteLine("type 'new' to create a new project, type 'projectName' to open a project(so you can load different versions of it)");
                    Hub();
                    break;
                case "new":
                    if (cmd.Arg == "proj")
                    {
                        CreateProject();
                    }
                    break;
                default:
                    next.Err("command not found");
                    Hub();
                    break;
            }
        }

        public void _Project(Project project, string branchPath)
        {
            next.Adv("project opened");
            Thread.Sleep(10000);
            Command cmd = new Command(next.Cmd());
            Thread.Sleep(10000);
            switch(cmd.Cmd)
            {
                case "new":
                    switch(cmd.Arg)
                    {
                        case "branch":
                            string branchName = next.Arg();
                            Directory.CreateDirectory(projFolder + branchName);
                            next.Adv("created and opened branch:  " + branchName);
                            _Project(project, branchName);
                            break;
                        case "save":
                            next.Adv("type name of the new save");
                            string name = next.Arg();
                            ZipFile.CreateFromDirectory(project.Path_op, project.Path + @"\" + name);
                            next.Adv("created save");
                            break;

                    }
                    break;
                default:
                    next.Err("command not found.");
                    _Project(project, "");
                    break;
            }
        }

        public void CreateProject()
        {
            string projDir = "balz";
            next.Adv("what do you want to name your project?");
            string name = next.Arg();
            if(name == null)
            {
                next.Err("project is invalid");
                CreateProject();
            }
            next.Adv("type the name of the folder your project will be saved into. Type '/other' to use a physical path leading to a different location. Type /def to name it after the project.");
            string path = next.Arg();
            next.Adv("type the full path of the folder your project is currently in. This can be changed later");
            string projPath = next.Arg();
            if (!Directory.Exists(projPath)) { next.Err("invalid path"); CreateProject(); }
            if(path == "/other")
            {
                next.Adv("type the path you want. Type 'esc' to use default");
                string ans = next.Arg();
                if(ans != "esc")
                {
                    if (!Directory.Exists(ans)) { next.Err("folder not found"); CreateProject(); }
                    projDir = ans;
                }
                else { CreateProject(); }
            }
            else if(path == "/def" || path == null)
            {
                projDir = projFolder + name;
            }
            else
            {
                projDir = projFolder + path;
            }
            next.Adv("creating project");
            next.Prog();
            if(!Directory.Exists(projDir))
            {
                Directory.CreateDirectory(projDir);
                Directory.CreateDirectory(projDir);
            }
            next.Prog();
            WriteSave(name, projDir, @"C:\\WinTools\Files\VersionControl\Table\proj.bgdf");
            next.Prog();
            WriteSave(name + "_op", projPath, projBowl);
            next.Prog();
            FileStream fs = File.Create(projDir + @"\proj.bgdf");
            next.Prog();
            fs.Close();
            next.Prog();
            Console.WriteLine();
            next.Adv("project created and opened");
            currentDir = projDir;
            next.Adv("till here");
            Thread.Sleep(10000);
            _Project(new Project(name), "");

        }

        public string ReadSave(string name, string path)
        {
            Assembly asm = Assembly.LoadFile(bgdfController);
            Type type = asm.GetTypes()[0];
            var obj = Activator.CreateInstance(type);
            MethodInfo c = type.GetMethod("ReadString");
            return c.Invoke(obj, new object[] { name, path }).ToString();
        }
        public void WriteSave(string nameOfVar, string data, string filePath)
        {
            Assembly a = Assembly.LoadFile(bgdfController);
            Type b = a.GetTypes()[0];
            var obj = Activator.CreateInstance(b);
            MethodInfo c = b.GetMethod("SaveString");
            c.Invoke(obj, new object[] { data, nameOfVar, filePath });
        }
        private string Shorten(string input)
        {
            while (input.Substring(0, 1) == " ")
            {
                input = input.Substring(1);
            }
            while (input.Substring(input.Length - 1) == " ")
            {
                input = input.Substring(0, input.Length - 1);
            }
            return input;
        }
        public void SaveProjectSettings(Project input)
        {
            //File.WriteAllLines(ReadSave("projectRoot", @"C:\\WinTools\Files\VersionControl\Table\set.bgdf") + @"\" + "proj.bgdf", new string[] { input.Name, input.Path, input.Path_op });
        }
    }

    public class Next
    {
        public string Cmd()
        {
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("cmd>    ");
            Console.ForegroundColor = ConsoleColor.White;
            return Console.ReadLine();
        }
        public string Arg()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("arg>    ");
            Console.ForegroundColor = ConsoleColor.White;
            return Console.ReadLine();
        }
        public void Title(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void Adv(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Err(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Prog()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write('.');
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void List(string[] input)
        {
            Console.WriteLine();
            if (input != null)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
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
        public Command(string line)
        {
            char[] temp = line.ToCharArray();
            bool ext = false;
            bool done = false;
            int ext_start = 0;
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == ':')
                {
                    ext = true;
                    ext_start = i;
                }
            }
            for (int i = 0; i < temp.Length; i++)
            {
                if ((temp[i] == ' ') && done == false)
                {
                    Cmd = line.Substring(0, i);
                    if (ext == true)
                    {
                        Arg = line.Substring(i, ext_start - i);
                        Post = line.Substring(ext_start + 1);
                    }
                    else
                    {
                        Arg = line.Substring(i);
                        Post = null;
                    }
                    done = true;
                }
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

        private string Shorten(string input)
        {
            while (input.Substring(0, 1) == " ")
            {
                input = input.Substring(1);
            }
            while (input.Substring(input.Length - 1) == " ")
            {
                input = input.Substring(0, input.Length - 1);
            }
            return input;
        }
    }

    public class Project
    {
        public string Name { get; set; }
        //path for the folder where the actual files are
        public string Path_op { get; set; }
        //Path where backups are being saved
        public string Path { get; set; }

        public Project(string name)
        {
            VersionControl versionControl= new VersionControl();
            Next next = new Next();
            Name = name;
            next.Adv("name got");
            next.Adv(name);
            Thread.Sleep(5000);
            Path = versionControl.ReadSave(name, @"C:\\WinTools\Files\VersionControl\Table\proj.bgdf");
            next.Adv("path got");
            Thread.Sleep(2000);
            Path_op = versionControl.ReadSave(name + "_op", @"C:\\WinTools\Files\VersionControl\Table\proj.bgdf");
            next.Adv("path_op got");
            Thread.Sleep(2000);
        }
    }
}
