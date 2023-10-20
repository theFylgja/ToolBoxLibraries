using System;
using System.IO;
using System.Collections.Generic;

namespace BiomeLibrary
{
    public class Bowl
    {
        public ToolBox toolBox = new ToolBox();
        public enum _Type { String, Integer, Boolean, Null }
        public static void Setup()
        {
            if(!Directory.Exists(@"C:\WinTools\Files\BGDF"))
            {
                Directory.CreateDirectory(@"C:\WinTools\Files\BGDF");
            }
            return;
        }
        public static void Hub()
        {
            Console.WriteLine("this is not meant for execution");
            return;
        }

        public string ReadString(string nameOfVariable, string path)
        {
            Var variable = GetVar(nameOfVariable, path);
            return (variable.Value);
        }

        public int ReadInt(string nameOfVariable, string path)
        {
            Var variable = GetVar(nameOfVariable, path);
            return Convert.ToInt32(variable.Value);
        }

        public bool ReadBool(string nameOfVariable, string path)
        {
            Var variable = GetVar(nameOfVariable, path);
            if (variable.Value == "true") { return true; }
            else { return false; }
        }
        public string[] ReadAllString(string path)
        {
            Var[] vr = GetAsVarArray(path);
            string[] output = new string[vr.Length];
            for(int i =0; i < vr.Length; i++)
            {
                output[i] = vr[i].Name;
            }
            return output;
        }
        public void Save(Var variable, string path)
        {
            string newLine = VarToString(variable);
            string[] oldFile = File.ReadAllLines(path);
            string[] newFile = new string[oldFile.Length + 1];
            for (int i = 0; i < oldFile.Length; i++) { newFile[i] = oldFile[i]; }
            newFile[newFile.Length - 1] = newLine;
            File.WriteAllLines(path, newFile);
            return;
        }
        public void SaveString(string data, string nameOfVariable, string path)
        {
            string newLine = "$var<str>'" + nameOfVariable + "'('" + data + "');";
            string[] oldFile = File.ReadAllLines(path);
            string[] newFile = new string[oldFile.Length + 1];
            for (int i = 0; i < oldFile.Length; i++) { newFile[i] = oldFile[i]; }
            newFile[newFile.Length - 1] = newLine;
            File.WriteAllLines(path, newFile);
            return;
        }

        public void SaveInt(int data, string NameOfVariable, string path)
        {
            string data_string = data.ToString();
            string newLine = "$var<int>'" + NameOfVariable + "'('" + data_string + "');";
            string[] oldFile = File.ReadAllLines(path);
            string[] newFile = new string[oldFile.Length + 1];
            for (int i = 0; i < oldFile.Length; i++) { newFile[i] = oldFile[i]; }
            newFile[newFile.Length - 1] = newLine;
            File.WriteAllLines(path, newFile);
            return;
        }

        public void SaveFloat(float data, string NameOfVariable, string path)
        {
            string data_string = data.ToString();
            string newLine = "$var<flt>'" + NameOfVariable + "'('" + data_string + "');";
            string[] oldFile = File.ReadAllLines(path);
            string[] newFile = new string[oldFile.Length + 1];
            for (int i = 0; i < oldFile.Length; i++) { newFile[i] = oldFile[i]; }
            newFile[newFile.Length - 1] = newLine;
            File.WriteAllLines(path, newFile);
            return;
        }

        public void SaveBool(bool data, string nameOfVariable, string path)
        {
            string data_string = "0";
            if (data == true) { data_string = "true"; }
            else if (data == false) { data_string = "false"; }
            string newLine = "$var<bol>'" + nameOfVariable + "'('" + data_string + "');";
            string[] oldFile = File.ReadAllLines(path);
            string[] newFile = new string[oldFile.Length + 1];
            for (int i = 0; i < oldFile.Length; i++) { newFile[i] = oldFile[i]; }
            newFile[newFile.Length - 1] = newLine;
            File.WriteAllLines(path, newFile);
            return;
        }

        public void ReplaceString(string data, string nameOfVariable, string path)
        {
            Remove(nameOfVariable, path);
            SaveString(data, nameOfVariable, path);
            return;
        }

        public void ReplaceInt(int data, string nameOfVariable, string path)
        {
            Remove(nameOfVariable, path);
            SaveInt(data, nameOfVariable, path);
            return;
        }

        public void ReplaceFloat(float data, string nameOfVariable, string path)
        {
            Remove(nameOfVariable, path);
            SaveFloat(data, nameOfVariable, path);
            return;
        }

        public void ReplaceBool(bool data, string nameOfVariable, string path)
        {
            Remove(nameOfVariable, path);
            SaveBool(data, nameOfVariable, path);
            return;
        }

        public void Remove(string nameOfVariable, string path)
        {
            Var[] asVar = GetAsVarArray(path);
            List<string> file = new List<string>();
            for (int i = 0; i < asVar.Length; i++)
            {
                if (asVar[i].Name != nameOfVariable)
                {
                    file.Add(VarToString(asVar[i]));
                }
            }
            File.WriteAllLines(path, file.ToArray());
        }
        public void WipeFile(string path)
        {
            string[] newFile = new string[] { };
            File.WriteAllLines(path, newFile);
        }

        public Var[] GetAsVarArray(string path)
        {
            string[] asString = File.ReadAllLines(path);
            Var[] output = new Var[asString.Length];
            for (int i = 0; i < asString.Length; i++)
            {
                output[i] = GetVar(GetName(asString[i]), path);
            }
            return output;
        }

        public string VarToString(Var varObj)
        {
            return "$var<" + toolBox.TypeToString(varObj.Type) + ">'" + varObj.Name + "'('" + varObj.Value + "');";
        }

        public Var GetVar(string name, string path)
        {
            string[] file = File.ReadAllLines(path);
            string line;
            for (int i = 0; i < file.Length; i++)
            {
                line = file[i];
                Var lineVar = new Var(GetName(line), GetValue(line), i, GetType(line));
                if (lineVar.Name == name)
                {
                    return lineVar;
                }
            }
            return null;

        }
        private string GetName(string line)
        {
            string[] aline = toolBox.ToArray(line);
            int first = -12;
            int last = -12;
            int length;
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
        private string GetValue(string line)
        {
            string[] aline = toolBox.ToArray(line);
            int first = -12;
            int last = -12;
            int length;
            for (int i = 0; i < aline.Length; i++)
            {
                if (aline[i] == "'")
                {
                    if (first == -12)
                    {
                        first = -11;
                    }
                    else if (first == -11)
                    {
                        first = -10;
                    }
                    else if (first == -10)
                    {
                        first = i + 1;
                    }
                    else
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

        private _Type GetType(string line)
        {
            string[] aline = toolBox.ToArray(line);
            int first = -12;
            for (int i = 0; i < aline.Length; i++)
            {
                if (aline[i] == "<")
                {
                    first = i + 1;
                }
            }
            if (first == -12)
            {
                return _Type.Null;
            }
            switch (line.Substring(first, 3))
            {
                case "str":
                    return _Type.String;
                case "int":
                    return _Type.Integer;
                case "bol":
                    return _Type.Boolean;
                default:
                    return _Type.Null;
            }
        }

        public class Var
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public int Line { get; set; }
            public Bowl._Type Type { get; set; }

            public Var(string name, string value, int line, _Type type)
            {
                Name = name;
                Value = value;
                Line = line;
                Type = type;
            }
            public Var(string name, string value, Bowl._Type type)
            {
                Name = name;
                Value = value;
                Type = type;
            }
        }

        public class Error
        {
            public string Msg;
        }
        public class ToolBox
        {
            public string TypeToString(Bowl._Type type)
            {
                switch (type)
                {
                    case Bowl._Type.String:
                        return "str";
                    case Bowl._Type.Integer:
                        return "int";
                    case Bowl._Type.Boolean:
                        return "bol";
                    default:
                        return "nul";
                }
            }
            public string[] ToArray(string Input)
            {
                string[] output = new string[Input.Length];
                for (int i = 0; i < Input.Length; i++)
                {
                    output[i] = Input.Substring(i, 1);
                }
                return output;
            }

            public void LogError(Error error)
            {
                throw new Exception();
            }
        }
    }
}
