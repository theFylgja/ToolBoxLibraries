using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace BiomeLibrary
{
    public class Bowl
    {
        //Attributes
        public string Path { get; set; }

        //accessible methods(non private plus all non static)
        public bool Exists(string name)
        {
            List<string> names = GetAllNames().ToList<string>();
            return names.Contains(name);
        }
        public void Set(string name, string data)
        {
            if (!Exists(name))
            {
                Create(name, data, Type.String, DataType.SingleValue);
            }
            Var[] vars = GetAsVarArray();
            for (int i = 0; i < vars.Length; i++)
            {
                if (vars[i].Name == name)
                {
                    vars[i].Value = data;
                }
            }
            SaveVarArray(vars);
        }
        public void Set(string name, int data)
        {
            if (!Exists(name))
            {
                Create(name, data.ToString(), Type.Integer, DataType.SingleValue);
            }
            Var[] vars = GetAsVarArray();
            for (int i = 0; i < vars.Length; i++)
            {
                if (vars[i].Name == name)
                {
                    vars[i].Value = data.ToString();
                    vars[i].ValueType = Type.Integer;
                    vars[i].KindOfData = DataType.SingleValue;
                }
            }
            SaveVarArray(vars);
        }
        public void Set(string name, double data)
        {
            if (!Exists(name))
            {
                Create(name, data.ToString(), Type.Double, DataType.SingleValue);
            }
            Var[] vars = GetAsVarArray();
            for (int i = 0; i < vars.Length; i++)
            {
                if (vars[i].Name == name)
                {
                    vars[i].Value = data.ToString();
                }
            }
            SaveVarArray(vars);
        }
        public void Set(string name, bool data)
        {
            if (!Exists(name))
            {
                Create(name, BoolToString(data), Type.Boolean, DataType.SingleValue);
            }
            Var[] vars = GetAsVarArray();
            for (int i = 0; i < vars.Length; i++)
            {
                if (vars[i].Name == name)
                {
                    vars[i].Value = BoolToString(data);
                    vars[i].ValueType = Type.Boolean;
                    vars[i].KindOfData = DataType.SingleValue;
                }
            }
            SaveVarArray(vars);
        }
        public void Set(string name, string[] data)
        {
            if (!Exists(name))
            {
                Create(name, ArrayToString(data), Type.String, DataType.Array);
                return;
            }
            Var[] vars = GetAsVarArray();
            for (int i = 0; i < vars.Length; i++)
            {
                if (vars[i].Name == name)
                {
                    vars[i].Value = ArrayToString(data);
                }
            }
            SaveVarArray(vars);
        }
        public void Set(string name, bool[] data)
        {
            string[] strings = new string[data.Length];
            for (int j = 0; j < strings.Length; j++)
            {
                strings[j] = BoolToString(data[j]);
            }

            if (!Exists(name))
            {
                Create(name, ArrayToString(strings), Type.Boolean, DataType.Array);
            }
            Var[] vars = GetAsVarArray();
            for (int i = 0; i < vars.Length; i++)
            {
                if (vars[i].Name == name)
                {
                    vars[i].Value = ArrayToString(strings);
                }
            }
            SaveVarArray(vars);
        }
        public void Set(string name, int[] data)
        {
            object[] objs = new object[data.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i] = (object)data[i];
            }

            if (!Exists(name))
            {
                Create(name, ArrayToString(ToStringArray(objs)), Type.Integer, DataType.Array);
            }
            Var[] vars = GetAsVarArray();
            for (int i = 0; i < vars.Length; i++)
            {
                if (vars[i].Name == name)
                {
                    vars[i].Value = ArrayToString(ToStringArray(objs));
                }
            }
            SaveVarArray(vars);
        }
        public void Set(string name, double[] data)
        {
            object[] objs = new object[data.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i] = (object)data[i];
            }

            if (!Exists(name))
            {
                Create(name, ArrayToString(ToStringArray(objs)), Type.Double, DataType.Array);
            }
            Var[] vars = GetAsVarArray();

            for (int i = 0; i < vars.Length; i++)
            {
                if (vars[i].Name == name)
                {
                    vars[i].Value = ArrayToString(ToStringArray(objs));
                }
            }
            SaveVarArray(vars);
        }

        //Get-methods
        public object Get(string name)
        {
            Var[] vars = GetAsVarArray();
            Var var = null;
            bool isArray = false;
            for (int i = 0; i < vars.Length; i++)
            {
                if (vars[i].Name == name)
                {
                    var = vars[i];
                    break;
                }
            }
            if (var == null)
            { return null; }

            switch (var.KindOfData)
            {
                case DataType.SingleValue:

                    switch (var.ValueType)
                    {
                        case Type.String:
                            return (object)var.Value;
                        case Type.Boolean:
                            return (object)ToBool(var.Value);
                        case Type.Integer:
                            return (object)Convert.ToInt32(var.Value);
                        case Type.Double:
                            return (object)Convert.ToDouble(var.Value);
                    }
                    break;
                case DataType.Array:

                    switch (var.ValueType)
                    {
                        case Type.String:
                            return (object)GetStringArray(var.Value);
                        case Type.Boolean:
                            return (object)GetBoolArray(var.Value);
                        case Type.Integer:
                            return (object)GetIntArray(var.Value);
                        case Type.Double:
                            return (object)GetDoubleArray(var.Value);
                    }
                    break;
            }
            return null;
        }
        //private read-write methods
        private void Create(string name, string data, Type type, DataType dataType)
        {
            List<Var> file = GetAsVarArray().ToList<Var>();
            file.Add(new Var(name, data, type, dataType));
            string[] file_s = new string[file.Count];
            for(int i = 0; i < file.Count; i++)
            {
                file_s[i] = VarToString(file[i]);
            }
            File.WriteAllLines(Path, file_s);
        }

        //constructor
        public Bowl(string path)
        {
            if (!File.Exists(path)) 
            {
                File.Create(path).Close();
            }

            Path = path;
        }



        //support methods
        private bool ToBool(string input)
        {
            if (input == "true")
            {
                return true;
            }
            return false;
        }
        private string BoolToString(bool input)
        {
            if (input)
            {
                return "true";
            }
            return "false";
        }
        private int[] GetIntArray(string input)
        {
            List<int> output = new List<int>();
            char[] asChar = input.ToCharArray();
            for (int i = 0; i < asChar.Length; i++)
            {
                if (asChar[i] == '|')
                {
                    output.Add(Convert.ToInt32(input.Substring(0, i)));
                    input = input.Substring(i + 1);
                    asChar = input.ToCharArray();
                    i = 0;
                }
                else if (asChar[i] == '.')
                {
                    output.Add(Convert.ToInt32(input.Substring(0, input.Length - 1)));
                    break;
                }
            }
            return output.ToArray();
        }
        private string[] GetStringArray(string input)
        {
            List<string> output = new List<string>();
            char[] asChar = input.ToCharArray();
            for (int i = 0; i < asChar.Length; i++)
            {
                if (asChar[i] == '|')
                {
                    output.Add(input.Substring(0, i));
                    input = input.Substring(i + 1);
                    asChar = input.ToCharArray();
                    i = 0;
                }
                else if (asChar[i] == '.')
                {
                    output.Add(input.Substring(0, input.Length - 1));
                    break;
                }
            }
            return output.ToArray();
        }
        private double[] GetDoubleArray(string input)
        {
            List<double> output = new List<double>();
            char[] asChar = input.ToCharArray();
            for (int i = 0; i < asChar.Length; i++)
            {
                if (asChar[i] == '|')
                {
                    Console.WriteLine(Convert.ToDouble(input.Substring(0, i)));
                    output.Add(Convert.ToDouble(input.Substring(0, i)));
                    input = input.Substring(i + 1);
                    asChar = input.ToCharArray();
                    i = 0;
                }
                else if (asChar[i] == '.')
                {
                    output.Add(Convert.ToDouble(input.Substring(0, input.Length - 1)));
                    break;
                }
            }
            return output.ToArray();
        }
        private bool[] GetBoolArray(string input)
        {
            List<bool> output = new List<bool>();
            char[] asChar = input.ToCharArray();
            for (int i = 0; i < asChar.Length; i++)
            {
                if (asChar[i] == '|')
                {
                    if (input.Substring(0, i) == "True")
                    {
                        output.Add(true);
                    }
                    else
                    {
                        output.Add(false);
                    }
                    input = input.Substring(i + 1);
                    asChar = input.ToCharArray();
                    i = 0;
                }
                else if (asChar[i] == '.')
                {
                    if (input.Substring(0, input.Length - 1) == "True")
                    {
                        output.Add(true);
                    }
                    else
                    {
                        output.Add(false);
                    }
                    break;
                }
            }
            return output.ToArray();
        }
        private string ArrayToString(string[] array)
        {
            string result = "";
            for (int i = 0; i < array.Length; i++)
            {
                result = result + "|" + array[i];
            }
            return result.Substring(1) + ".";
        }
        private string[] ToStringArray(object[] array)
        {
            string[] result = new string[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i].ToString();
            }
            return result;
        }
        private string GetName(string line)
        {
            string str = "'"; char sign = str.ToCharArray()[0];
            char[] asChar = line.ToCharArray();
            int start = 0;
            int end = 0;
            for (int i = 0; i < asChar.Length; i++)
            {
                if (asChar[i] == sign)
                {
                    if (start != 0)
                    {
                        end = i;
                        break;
                    }
                    else
                    {
                        start = i;
                    }
                }
            }
            return line.Substring(start + 1, end - start - 1);
        }
        private string VarToString(Var input)
        {
            string valueType_string;
            string dataType_string;
            switch (input.ValueType)
            {
                case Type.String:
                    valueType_string = "string";
                    break;
                case Type.Integer:
                    valueType_string = "int";
                    break;
                case Type.Boolean:
                    valueType_string = "bool";
                    break;
                case Type.Double:
                    valueType_string = "double";
                    break;
                default:
                    valueType_string = "string";
                    break;
            }
            switch (input.KindOfData)
            {
                case DataType.SingleValue:
                    dataType_string = "var";
                    break;
                case DataType.Array:
                    dataType_string = "arr";
                    break;
                case DataType.Object:
                    dataType_string = "obj";
                    break;
                default:
                    dataType_string = "var";
                    break;
            }
            return "$" + dataType_string + "<" + valueType_string + ">'" + input.Name + "'('" + input.Value + "');";
        }

        private Var[] GetAsVarArray()
        {
            string[] asString = File.ReadAllLines(Path);
            Var[] output = new Var[asString.Length];
            for (int i = 0; i < asString.Length; i++)
            {
                output[i] = new Var(asString[i]);
            }
            return output;
        }
        private Var GetVar(string name)
        {
            string[] file = ReadAllString();
            string line;
            for (int i = 0; i < file.Length; i++)
            {
                line = file[i];
                Var lineVar = new Var(line);
                if (lineVar.Name == name)
                {
                    return lineVar;
                }
            }
            return null;

        }
        private string[] ReadAllString()
        {
            List<string> fileList = new List<string>();
            string file = File.ReadAllText(Path);
            char[] file_char = file.ToCharArray();
            int last = 0;
            for (int i = 0; i < file_char.Length; i++)
            {
                if (file_char[i] == ';')
                {
                    fileList.Add(file.Substring(last, i - last + 1));
                    last = i + 1;
                }
            }
            return fileList.ToArray();
        }
        public string[] GetAllNames()
        {
            string[] lines = ReadAllString();
            string[] names = new string[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                names[i] = GetName(lines[i]);
            }
            return names;
        }
        public void SaveVarArray(Var[] input)
        {
            string[] file = new string[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                file[i] = VarToString(input[i]);
            }
            File.WriteAllLines(Path, file);
        }
    }


    public class Var
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public Type ValueType { get; set; }
        public DataType KindOfData { get; set; }

        public Var(string name, string value, Type valueType, DataType dataType)
        {
            Name = name;
            Value = value;
            ValueType = valueType;
            KindOfData = dataType;
        }
        public Var(string line)
        {
            string str = "'"; char sign = str.ToCharArray()[0];
            int[] points = new int[6]; //marks important points in the line 0 = "<", 1 = ">", 2 = "'", 3 = "'"(the second one), 4 = "(", 5 = ")"
            char[] asCharArray = line.ToCharArray();
            //get all points
            points[3] = 0;
            for (int i = 0; i < line.Length; i++)
            {
                switch (asCharArray[i])
                {
                    case '<':
                        points[0] = i;
                        break;
                    case '>':
                        points[1] = i;
                        break;
                    case '(':
                        points[4] = i;
                        break;
                    case ')':
                        points[5] = i;
                        break;
                    default:
                        if (asCharArray[i] == sign)
                        {
                            if (points[2] > 0 && points[3] == 0)
                            {
                                points[3] = i;
                                break;
                            }
                            else if (points[2] == 0)
                            {
                                points[2] = i;
                            }
                        }
                        break;

                }
            }
            Name = line.Substring(points[2] + 1, points[3] - points[2] - 1);
            Value = line.Substring(points[4] + 2, points[5] - points[4] - 3);
            string temp = line.Substring(points[0] + 1, points[1] - points[0] - 1);
            switch (temp)
            {
                case "string":
                    ValueType = Type.String;
                    break;
                case "int":
                    ValueType = Type.Integer;
                    break;
                case "bool":
                    ValueType = Type.Boolean;
                    break;
                case "double":
                    ValueType = Type.Double;
                    break;
                default:
                    ValueType = Type.String;
                    break;
            }
            temp = line.Substring(1, points[0] - 1);
            switch (temp)
            {
                case "var":
                    KindOfData = DataType.SingleValue;
                    break;
                case "arr":
                    KindOfData = DataType.Array;
                    break;
                case "obj":
                    KindOfData = DataType.Object;
                    break;
                default:
                    KindOfData = DataType.SingleValue;
                    break;
            }
        }

        public string[] ValuetoArray()
        {
            List<string> valueArr = new List<string>();
            List<int> commas = new List<int>();
            char[] valueCharArr = Value.ToCharArray();
            for (int i = 0; i < valueCharArr.Length; i++)
            {
                if (valueCharArr[i] == ',') { commas.Add(i); }
            }
            int[] aCommas = commas.ToArray();
            for (int i = 0; i < aCommas.Length; i++)
            {
                if (i == 0)
                {
                    valueArr.Add(Value.Substring(1, aCommas[0] - 2));
                }
                else if (i == aCommas.Length - 1)
                {
                    valueArr.Add(Value.Substring(aCommas[i] + 1));
                }
                else
                {
                    valueArr.Add(Value.Substring(aCommas[i], aCommas[i + 1] - aCommas[i] - 1));
                }
            }
            return valueArr.ToArray();
        }
    }

    public enum Type { String, Integer, Boolean, Double }
    public enum DataType { SingleValue, Array, Object }
}