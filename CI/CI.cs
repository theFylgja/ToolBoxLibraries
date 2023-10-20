using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.Xml.XPath;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Messaging;
using System.Linq.Expressions;

namespace CI
{
    public class ConsoleInterface
    {
        //general usage variables
        public Next next = new Next();
        public Command cmd;
        public string root;
        public string currentPath;
        public List<string> pathMem;
        public SessionType current;
        public string bowlPath = @"C:\WinTools\Extensions\supp\Bowl.dll";
        public static string bowlPath_static = @"C:\WinTools\Extensions\supp\Bowl.dll";
        public static string listPath = @"C:\WinTools\Files\CI\lists.bgdf";
        public static string linkPath = @"C:\WinTools\Files\CI\linking.bgdf";
        public static string varPath = @"C:\WinTools\Files\CI\var.bgdf";
        public static int commandIndex = -1;
        public static Command[] extCommands = null;
        public List currentList;
        //public Settings currentSettings = new Settings(null, null, null);
        public string preMount = null;


        //neccessary ToolBox Methods
        public void Setup()
        {
            if(!Directory.Exists(@"C:\WinTools\Files\CI"))
            {
                Directory.CreateDirectory(@"C:\WinTools\Files\CI");
                FileStream fs = File.Create(@"C:\WinTools\Files\CI\settings.bgdf");
                fs.Close();
                fs = File.Create(@"C:\WinTools\Files\CI\lists.bgdf");
                fs.Close();
                fs = File.Create(@"C:\WinTools\Files\CI\linking.bgdf");
                fs.Close();
                fs = File.Create(@"C:\WinTools\Files\CI\var.bgdf");
                fs.Close();
            }
        }
        public void Hub()
        {
            next.Title("ConsoleInterface:");
            next.Adv("type your command:");
            Main(null, SessionType.Default);
        }

        //method replacing the hub, just for ease of use idk (also, i just noticed that the root parameter is kinda unneccessary lmao)
        public void Main(string rootDir, SessionType currentSession)
        {
            Command cmd;
            cmd = extCommands?[commandIndex] ?? new Command(next.Cmd());
            next.Err(commandIndex.ToString());
            current = currentSession;
            root = rootDir;
            next.Adv(ReadSave(cmd.Cmd, linkPath));
            if (ReadSave(cmd.Cmd, linkPath) != null && cmd.Cmd != "ci" && cmd.Cmd != "do")
            {
                string headerHandle = ReadSave(cmd.Cmd, linkPath);
                ExecuteDLLMethod(headerHandle, "Handle", cmd.asStringArray);
                Main(rootDir, currentSession);
            }
            else if(cmd.Cmd == "ci" || cmd.Cmd == "do")
            {
                Handle(cmd.asStringArray);
                Main(rootDir, currentSession);
            }
            switch (currentSession)
            {
                case SessionType.Default:
                    switch (cmd.Cmd)
                    {
                        case "mount":
                            MountDirectory(cmd);
                            break;
                        case "using":
                            ExecuteExt(cmd.Arg);
                            break;
                        case "esc":
                            Main(rootDir, SessionType.Default);
                            break;
                        default:

                            switch (cmd.Arg)
                            {
                                case "esc":
                                    Main(root, SessionType.Default);
                                    break;
                                case "install":
                                    InstallExt(cmd);
                                    break;
                                case "execute":
                                    ExecuteExt(cmd.Cmd);
                                    break;
                                case "use":
                                    ExecuteExt(cmd.Cmd);
                                    break;
                                default:
                                    Main(root, currentSession); 
                                    break;

                            }
                            break;

                    }
                    break;
                case SessionType.Directory:
                    currentPath = root;
                    switch (cmd.Cmd)
                    {
                        case "dcreate":
                            try
                            {
                                Directory.CreateDirectory(root + @"\" + cmd.Arg);
                            }
                            catch(Exception)
                            {
                                next.Err("an error has occured. please try again.");
                            }
                            break;
                        case "obj":
                            switch(cmd.Arg)
                            {
                                case "print":
                                    next.Adv("current object: Directory :  " + root);
                                    break;
                                default:
                                    next.Err("command not found");
                                    break;

                            }
                            break;
                        case "mount":
                            MountDirectory(cmd);
                            break;
                        case "using":
                            if(cmd.Arg.Substring(cmd.Arg.Length - 4) == ".exe")
                            {
                                if(!File.Exists(root + @"\" + cmd.Arg))
                                {
                                    next.Err("file not found");
                                    Main(root, SessionType.Directory);
                                }
                                StartExecutable(root, cmd.Arg);
                                
                            }
                            else if(cmd.Arg.Substring(cmd.Arg.Length - 4) == ".dll")
                            {
                                ExecuteExt(cmd.Arg);
                            }
                            break;
                        case "esc":
                            return;
                        case "this":
                            switch(cmd.Arg)
                            {
                                case "list":
                                    next.Adv("all files and subdirectories located in the current directory");
                                    next.List(ExtractName(Directory.GetFileSystemEntries(root)));
                                    break;
                                case "flist":
                                    next.Adv("Files located in the current Directory");
                                    next.List(ExtractName(Directory.GetFiles(root)));
                                    break;
                                case "dlist":
                                    next.Adv("Subdirectories located in the current Directory");
                                    next.List(ExtractName(Directory.GetDirectories(root)));
                                    break;
                                case "fselist":
                                    next.Adv("File System Entries (files and directories) located in the current Directory");
                                    next.List(ExtractName(Directory.GetFileSystemEntries(root)));
                                    break;
                                case "madlist":
                                    next.Adv("literally everything in the current directory (20 layers)");
                                    next.Adv("could take a couple seconds if it is a big directory, please let it take its time aight");
                                    next.List(ExtractName(MadJoin(MadDirectories(new string[] { root }, 20))));
                                    break;

                                case "install":
                                    InstallExt(cmd);
                                    break;
                                case "execute":
                                    ExecuteExt(cmd.Cmd);
                                    break;
                                case "use":
                                    ExecuteExt(cmd.Cmd);
                                    break;

                                case "delete":
                                    next.Adv("u sure? (y/n)");
                                    string ans = next.Arg();
                                    if(ans == "y" || ans == "yes")
                                    {
                                        root = Directory.GetParent(root).FullName;
                                        Directory.Delete(root);
                                        next.Adv("directory deleted");
                                        break;
                                    }
                                    next.Adv("action interrupted");
                                    break;
                                    
                            }
                            break;
                        default:
                            switch (cmd.Arg)
                            {
                                case "open":
                                    if (File.Exists(rootDir + @"\" + cmd.Cmd)) 
                                    {
                                        currentPath = root + @"\" + cmd.Cmd;
                                        next.Adv("opened file:  " + currentPath);
                                    }
                                    else { next.Err("file does not exist"); break; }
                                    Main(root, SessionType.File);
                                    break;
                                case "create":
                                    bool isFile = false;
                                    char[] asArray = cmd.Cmd.ToCharArray();
                                    foreach(char c in asArray)
                                    {
                                        if(c == '.')
                                        {
                                            isFile = true;
                                        }
                                    }
                                    if (isFile)
                                    {
                                        if (File.Exists(root + @"\" + cmd.Cmd))
                                        {
                                            next.Err("file already exists");
                                            break;
                                        }
                                        FileStream fs = File.Create(rootDir + @"\" + cmd.Cmd);
                                        fs.Close();
                                        next.Adv("created file at " + root + @"\" + cmd.Cmd);
                                        break;
                                    }
                                    else
                                    {
                                        if(Directory.Exists(root + @"\" + cmd.Cmd))
                                        {
                                            next.Err("file exists already.");
                                            break;
                                        }
                                        Directory.CreateDirectory(root + @"\" + cmd.Cmd);
                                        next.Adv("created directory at " + root + @"\" + cmd.Cmd);
                                        break;
                                    }
                                case "delete":
                                    if(!File.Exists(root + @"\" + cmd.Cmd))
                                    {
                                        next.Err("file does not exist");
                                        Main(root, SessionType.Directory);
                                    }
                                    File.Delete(root + @"\" + cmd.Cmd);
                                    next.Adv("deleted the file");
                                    Main(root, SessionType.Directory);
                                    break;
                                case "include":
                                    if(!File.Exists(root + @"\" + cmd.Cmd))
                                    {
                                        next.Err("file not found");
                                        break;
                                    }
                                    string[] file = File.ReadAllLines(root + @"\" + cmd.Cmd);
                                    Command[] commands = new Command[file.Length];
                                    for(int i = 0; i < file.Length; i++)
                                    {
                                        commands[i] = new Command(file[i]);
                                    }
                                    extCommands = commands;
                                    commandIndex = 0;
                                    break;
                                default:
                                    break;

                            }
                            break;

                    }
                    break;
                case SessionType.File:
                    switch(cmd.Cmd)
                    {
                        case "close":
                            Main(root, SessionType.Directory);
                            break;
                        case "this":
                            switch(cmd.Arg)
                            {
                                case "writex":
                                    if (!File.Exists(currentPath)) { next.Err("File does not exist"); Main(root, SessionType.File); break; }
                                    List<string> txt = new List<string>();
                                    next.Adv("this will overwrite the current content");
                                    next.Adv("type your text into the console, enter for a new line and 'esc' to finish");
                                    next.Adv("directly type 'esc' to stop the process. The current file content will not be overwritten then.");
                                    do
                                    {
                                        txt.Add(Console.ReadLine());
                                    }while(!txt.Contains("esc"));
                                    txt.Remove("esc");
                                    if (txt != null)
                                    {
                                        WriteFile(currentPath, txt.ToArray());
                                    }
                                    break;
                                case "write":
                                    if (!File.Exists(currentPath)) { next.Err("File does not exist"); Main(root, SessionType.File); break; }
                                    List<string> text = File.ReadAllLines(currentPath).ToList<string>();
                                    next.Adv("this will not overwrite the current content, use 'writex' for that");
                                    next.Adv("type your text into the console, enter for a new line and 'esc' to finish");
                                    next.Adv("directly type 'esc' to stop the process");
                                    next.List(text.ToArray()); 
                                    do
                                    {
                                        text.Add(Console.ReadLine());
                                    } while (!text.Contains("esc"));
                                    text.Remove("esc");
                                    if (text != null)
                                    {
                                        WriteFile(currentPath, text.ToArray());
                                    }
                                    break;
                                case "print":
                                    if (cmd.Cmd == "this")
                                    {
                                        next.Adv("text of file " + currentPath);
                                        Console.WriteLine(ReadFile(currentPath));
                                    }
                                    else if (cmd.Cmd == "obj")
                                    {
                                        //Shows currently active object(file, directory etc)
                                        next.Adv("current active object: File (" + currentPath + ")");
                                    }
                                    else { next.Err("command not found"); }
                                    break;
                                default:
                                    next.Err("command not found");
                                    break;
                            }
                            break;
                        default:
                            switch(cmd.Arg)
                            {
                                case "print":
                                    if(cmd.Cmd == "this")
                                    {
                                        next.Adv("text of file " + currentPath);
                                        Console.WriteLine(ReadFile(currentPath));
                                    }
                                    else if(cmd.Cmd == "obj")
                                    {
                                        //Shows currently active object(file, directory etc)
                                        next.Adv("current active object: File (" + currentPath + ")");
                                    }
                                    else { next.Err("command not found");}
                                    break;

                                default:
                                    break;
                            }
                            break;
                    }
                    break;
                case SessionType.List:
                    switch(cmd.Cmd)
                    {
                        case "this":
                            switch(cmd.Arg)
                            {

                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case SessionType.CI:
                    try
                    {
                        string header = cmd.asStringArray[0];
                        string headerHandle = ReadSave(header, linkPath);
                        if (header == "ci" || header == "do")
                        {
                            this.Handle(cmd.asStringArray);
                            break;
                        }
                        ExecuteDLLMethod(headerHandle, "Handle", cmd.asStringArray);
                    }
                    catch(Exception exc)
                    {
                        next.Err("command not found (an error was thrown and i protected you)");
                        next.Err(exc.Message);
                    }
                    break;
            }
            //general commands
            switch(cmd.Cmd)
            {
                case "var":
                    next.Adv("creating var");
                    string path = string.Empty;
                    if (ReadSave(cmd.Arg, varPath) != null)
                    {
                        if(cmd.Post != null)
                        {
                            switch(cmd.Post)
                            {
                                case "obj":
                                    path = currentPath;
                                    break;
                                default:
                                    path = cmd.Post;
                                    break;
                            }
                        }
                        SetString(cmd.Arg, path, varPath);
                    }
                    else
                    {
                        if (cmd.Post != null)
                        {
                            switch (cmd.Post)
                            {
                                case "obj":
                                    path = currentPath;
                                    break;
                                default:
                                    path = cmd.Post;
                                    break;
                            }
                        }
                        WriteSave(cmd.Arg, path, varPath);
                    }
                    break;
            }
            commandIndex++;
            if(commandIndex == 0)
            {
                commandIndex = -1;
                extCommands = null;
            }
            Main(rootDir, currentSession);
            
        }

        //command handling
        public void Handle(string[] cmd)
        {
            switch(cmd[1])
            {
                case "print":
                    next.Adv(cmd[2]);
                    break;
                case "link":
                    LinkLibrary(cmd);
                    next.Adv("created link");
                    break;
                default:
                    next.Err("command syntax seems to be wrong (error)");
                    break;
            }
            Main(root, SessionType.CI);
        }
        public void LinkLibrary(string[] cmd)
        {
            string link = ShortenChar(cmd[2], new List<string> { ">", "<" });
            string lib = cmd[3];
            string path = null;
            if (lib.Substring(0, 1) == "@")
            {
                path = lib.Substring(1);
            }
            else
            {
                try
                {
                    path = ReadExtSave(lib);

                }
                catch (Exception)
                {
                    next.Err("extension or dll file could not be found");
                    Main(root, current);
                }
            }
            WriteSave(link, path, linkPath);
            return;
        }
        public void ExecuteExternalMethod(string extName, string methodName, string[] cmd, string root)
        {
            string path = ReadExtSave(extName);
            if(path == null) 
            {
                next.Err("extension could not be found");
            }
            Assembly lib = Assembly.LoadFile(path);
            Type type = lib.GetTypes()[0]; 
            var obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod(methodName);
            method.Invoke(obj, new object[] { cmd, root});
        }
        public void ExecuteDLLMethod(string path, string methodName, string[] arg)
        {
            if (path == null)
            {
                next.Err("extension could not be found");
            }
            Assembly lib = Assembly.LoadFile(path);
            Type type = lib.GetTypes()[0];
            var obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod(methodName);
            method.Invoke(obj, new object[] { arg, root });
        }
        //Extension Handling
        public void InstallExt(Command cmd_in)
        {
            string filePath = @"C:\WinTools\Extensions\" + cmd_in.Cmd;
            if (!File.Exists(filePath))
            {
                next.Err("File not found. Make sure you typed the path correctly.");
                Main(root, current);
            }
            next.Adv("installing...");
            Assembly lib = Assembly.LoadFrom(filePath);
            Type type = lib.GetTypes()[0];
            var obj = Activator.CreateInstance(type);
            var method = type.GetMethod("Setup");
            method.Invoke(obj, new object[] { });
            WriteExtSave(cmd_in.Cmd.Substring(0, cmd_in.Cmd.Length - 4), @"C:\WinTools\Extensions\" + cmd_in.Cmd);
            next.Adv("install successful");
            Main(root, current);
        }
        public void ExecuteExt(string name)
        {
            string path = ReadExtSave(name);
            if(path == null) 
            {
                next.Err("extension could not be found");
            }
            Assembly lib = Assembly.LoadFile(path);
            Type type = lib.GetTypes()[0]; 
            var obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("Hub");
            method.Invoke(obj, new object[] { });
            Main(root, current);
        }
        public void ExecuteExternalMethod(string extName, string methodName)
        {
            string path = ReadExtSave(extName);
            if(path == null) 
            {
                next.Err("extension could not be found");
            }
            Assembly lib = Assembly.LoadFile(path);
            Type type = lib.GetTypes()[0]; 
            var obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod(methodName);
            method.Invoke(obj, new object[] { });
            return;
        }

        //Executable handling
        public void StartExecutable(string path, string name)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = path + @"\" + name;
            startInfo.WorkingDirectory = path;
            Process process = Process.Start(startInfo);
        }

        //native ToolBox Extension read/write handling
        public void WriteExtSave(string name, string path)
        {
            string newLine = "$Ext<" + name + ">('" + path + "');";
            string[] oldFile = File.ReadAllLines(@"C:\WinTools\Files\Intern\Ext.bgdf");
            string[] newFile = new string[oldFile.Length + 1];
            for (int i = 0; i < oldFile.Length; i++) { newFile[i] = oldFile[i]; }
            newFile[newFile.Length - 1] = newLine;
            File.WriteAllLines(@"C:\WinTools\Files\Intern\Ext.bgdf", newFile);
            return;
        }
        public string ReadExtSave(string name)
        {
            string[] file = File.ReadAllLines(@"C:\WinTools\Files\Intern\Ext.bgdf");
            string line;
            for (int i = 0; i < file.Length; i++)
            {
                line = file[i];
                if (GetName(line) == name)
                {
                    return GetPath(line);
                }
            }
            return null;
        }
        public string GetPath(string line)
        {
            string[] aline = ToArray(line);
            int first = -12;
            int last = -12;
            for (int i = 0; i < aline.Length; i++)
            {
                if (aline[i] == "'")
                {
                    if (first == -12)
                    {
                        first = i + 1;
                    }
                    else if (last == -12)
                    {
                        last = i;
                    }
                }
            }
            if (first == -12 || last == -12)
            {
                return null;
            }
            return line.Substring(first, last - first);
        }
        private string GetName(string line)
        {
            string[] aline = ToArray(line);
            int first = -12;
            int last = -12;
            for (int i = 0; i < aline.Length; i++)
            {
                if (aline[i] == "<")
                {
                    first = i + 1;
                }
                if (aline[i] == ">")
                {
                    last = i;
                }
            }
            if (first == -12 || last == -12)
            {
                return null;
            }
            return line.Substring(first, last - first);
        }

        //Tools and helping methods
        public string[] GetCommand(string line)
        {
            char[] line_ch = line.ToCharArray();
            List<string> output = new List<string>();
            bool open = false;
            int last = 0;
            for(int i = 0; i < line_ch.Length; i++)
            {
                if(line_ch[i] == '"')
                {
                    if(open == true)
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
                else if(i == line_ch.Length -1)
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
        private string[] ExtractName(string[] input)
        {
            if(input == null)
            {
                return null;
            }
            string[] temp = ToArray(input[0]);
            int index = 0;
            for(int i = 0; i < temp.Length; i++)
            {
                if(temp[i] == @"\")
                {
                    index = i;
                }
            }
            
            for(int i = 0; i < input.Length; i++)
            {
                input[i] = input[i].Substring(index + 1);
            }
            return input;
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
        public void MountDirectory(Command cmd_in)
        {
            string mount;
            mount = IsVar(cmd_in.Arg) ?? root + @"\" + cmd_in.Arg;
            if (cmd_in.Arg.Length == 1)
            {
                mount = cmd_in.Arg + @":\";
                if (cmd_in.Post != null)
                {
                    mount = cmd_in.Arg + @":\" + cmd_in.Post;
                }
            }
            if (cmd_in.Arg == "parent")
            {
                if (root.Length > 3)
                {
                    mount = Directory.GetParent(root).FullName;
                }
                else { mount = root; }
            }
            if (!Directory.Exists(mount)) { next.Err("directory not found"); Main(root, SessionType.Default); }
            next.Adv("mounted Directory:   " + mount);
            root = mount;
            Main(mount, SessionType.Directory);
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

        //Bowl Operation methods
        public string IsVar(string name)
        {
            Assembly asm = Assembly.LoadFile(bowlPath_static);
            Type type = asm.GetTypes()[0];
            var obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("ReadString");
            return (string)method.Invoke(obj, new object[] { name, varPath });
        }
        public static void SetString(string name, string data, string path)
        {
            Assembly asm = Assembly.LoadFile(bowlPath_static);
            Type type = asm.GetTypes()[0];
            var obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("RewriteString");
            method.Invoke(obj, new object[] { name, data, path});
        }
        public static void WriteSave(string name, string content, string path)
        {
            Assembly asm = Assembly.LoadFile(bowlPath_static);
            Type type = asm.GetTypes()[0];
            var obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("SaveString");
            method.Invoke(obj, new object[] {content, name, path});
        }
        public static string ReadSave(string name, string path)
        {
            Assembly asm = Assembly.LoadFile(bowlPath_static);
            Type type = asm.GetTypes()[0];
            var obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("ReadString");
            return (string)method.Invoke(obj, new object[] { name, path });
        }
        public static string[] ReadStringArray(string name, string path)
        {
            Assembly asm = Assembly.LoadFile(bowlPath_static);
            Type type = asm.GetTypes()[0];
            var obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("ReadStringArray");
            return (string[])method.Invoke(obj, new object[] { name, path });
        }
        public static void WriteStringArray(string name, string[] value, string path)
        {
            Assembly asm = Assembly.LoadFile(bowlPath_static);
            Type type = asm.GetTypes()[0];
            var obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("SaveStringArray");
            method.Invoke(obj, new object[] { value, name, path });
        }
        //File System methods
        public void OpenFile(Command cmd_in)
        {
            next.Adv("text of file " + root + @"\" + cmd_in.Cmd);
            Console.WriteLine(ReadFile(root + @"\" + cmd_in.Cmd));
            currentPath = root + @"\" + cmd_in.Cmd;
            Main(root, SessionType.File);
        }
        public void WriteFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }
        public void WriteFile(string path, string[] content)
        {
            File.WriteAllLines(path, content);
        }
        public string ReadFile(string path)
        {
            using(FileStream stream = new FileStream(path, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }


    //Helping classes (not necessarily for this, for console commands in general) also, its on GitHub: 
    //Except for this one, this one is just for CI
    public enum SessionType { File, Directory, Default, List, External, CI}

    //these are not
    public class Next
    {
        public string Cmd()
        {
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Blue;
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
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(text);
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void Adv(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Err(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void List(string[] input)
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
                Console.ForegroundColor= ConsoleColor.Red;
                Console.WriteLine("none");
            }
            Console.ForegroundColor= ConsoleColor.White;
            Console.WriteLine();
        }
    }
    public class Command
    {
        public string Cmd { get; set; }
        public string Arg { get; set; }
        public string Post { get; set; }
        public string[] asStringArray { get; set; }

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
            asStringArray = ci.GetCommand(line);
            for(int i = 0; i < temp.Length; i++)
            {
                if(temp[i] == ':' && !ext)
                {
                    ext = true;
                    ext_start = i;
                }
                else if(temp[i] == sign)
                {
                    if(sign1 != 0)
                    {
                        sign1 = i;
                    }
                    else if(sign2 != 0) //makes the program pick the second sign instead of the last one
                    {
                        sign2 = i;
                    }
                }
                else if(temp[i] == ' '&& done == false && sign1 == -16)//string has not been opened
                {
                    space = i;
                    done = true;
                }
                else if(temp[i] == ' ' && done == false && sign2 != -16)//string has been closed
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
            while(input.Substring(0, 1) == " " || input.Substring(0, 1) == sign.ToString())
            {
                input = input.Substring(1);
            }
            while(input.Substring(input.Length - 1) == " " || input.Substring(0, 1) == sign.ToString())
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
    public class List
    {
        public string Name { get; set; }
        public string[] Items { get; set; }

        public List(string name)
        {
            Name = name;
            Items = new string[]  { "placehldr"};
            ConsoleInterface.WriteStringArray(Name, Items, ConsoleInterface.listPath);
        }
        public void Add(string item)
        {
            List<string> items = Items.ToList<string>();
            items.Add(item);
            Items = items.ToArray();
        }
        public void Load()
        {
            Items = ConsoleInterface.ReadStringArray(Name, ConsoleInterface.listPath);
        }
        public void Save()
        {
            ConsoleInterface.WriteStringArray(Name, Items, ConsoleInterface.listPath);
        }
    }
}