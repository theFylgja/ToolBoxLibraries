using System;
using System.IO;
using Microsoft.CSharp;
using System.Net;
using System.CodeDom.Compiler;

namespace CC
{
    public class CCExecuter
    {
        public Next next = new Next();
        public void Setup()
        {
            if(!Directory.Exists(@"C:\\WinTools\Files\CCExecuter"))
            {
                Directory.CreateDirectory(@"C:\\WinTools\Files\CCExecuter");
                File.Create(@"C:\\WinTools\Files\CCExecuter\ScriptDump.txt");
                Directory.CreateDirectory(@"C:\\WinTools\Files\CCExecuter\Scripts");
                File.Create(@"C:\\WinTools\Files\CCExecuter\dump.txt");
            }
            return;
        }
        public void Hub(string cmd_in)
        {
            next.Title("CCExecutor");
            next.Adv("type your command. type '?' for list of commands");
            Command cmd = new Command(cmd_in ?? next.Cmd());
            switch(cmd.Cmd)
            {
                case "?":
                    next.Adv("type 'open-file' to open a code file and execute it. (only .cs files) ");
                    next.Adv("type 'direct-compile' to directly execute code typed into the console");
                    next.Adv("type 'esc' or '-1' to return to the ToolBox");
                    Console.WriteLine();
                    Hub();
                    break;
                case "open":
                    Console.WriteLine("path:");
                    OpenFile(Console.ReadLine());
                    break;
                    
            }
        }
        public void OpenFile(string path)
        {
            Console.WriteLine(path);
            if(!File.Exists(path) || path.Substring(path.Length - 3) != ".cs")
            {
                Console.WriteLine(path.Substring(path.Length - 3));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("file does not exist or is not a C# file");
            }
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters par = new CompilerParameters { GenerateInMemory = true, GenerateExecutable = false };
            CompilerResults results = provider.CompileAssemblyFromFile(par, path);
        }

        public void WriteFile(string input)
        {
            using(FileStream stream = new FileStream(input, FileAccess.))
        }
    }

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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.WriteLine("");
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
                if (temp[i] == ' ' && done == false)
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

        public string _ToString()
        {
            if(Post == null)
            {
                return Cmd + " " + Arg;
            }
            return Cmd + " " + Arg + ":" + Post;
            
        }
    }

}
