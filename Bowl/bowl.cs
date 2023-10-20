using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace BiomeLibrary
{
    public class Bowl
    {
        //write methods
        public static void SaveString(string data, string name, string path)
        {
            Var var = new Var(name, data, Type.String, DataType.SingleValue);
            string previousFileContent = File.ReadAllText(path);
            File.WriteAllText(path, previousFileContent + VarToString(var));
        }
        public static void SaveInt(int data, string name, string path)
        {
            Var var = new Var(name, data.ToString(), Type.String, DataType.SingleValue);
            string previousFileContent = File.ReadAllText(path);
            File.WriteAllText(path, previousFileContent + VarToString(var));
        }
        public static void SaveDouble(double data, string name, string path)
        {
            Var var = new Var(name, data.ToString(), Type.String, DataType.SingleValue);
            string previousFileContent = File.ReadAllText(path);
            File.WriteAllText(path, previousFileContent + VarToString(var));
        }
        public static void SaveBool(bool data, string name, string path)
        {
            string data_string;
            if (data) { data_string = "true"; } else { data_string = "false"; }
            Var var = new Var(name, data_string, Type.String, DataType.SingleValue);
            string previousFileContent = File.ReadAllText(path);
            File.WriteAllText(path, previousFileContent + VarToString(var));
        }

        //array write methods
        public static void SaveStringArray(string[] data, string name, string path)
        {
            string data_string = ArrayToString(data);
            Var var = new Var(name, data_string, Type.String, DataType.Array);
            string previousFileContent = File.ReadAllText(path);
            File.WriteAllText(path, previousFileContent + VarToString(var));
        }
        public static void SaveIntArray(int[] data, string name, string path)
        {
            string data_string = ArrayToString(ToStringArray(data));
            Var var = new Var(name, data_string, Type.Integer, DataType.Array);
            string previousFileContent = File.ReadAllText(path);
            File.WriteAllText(path, previousFileContent + VarToString(var));
        }
        public static void SaveBoolArray(bool[] data, string name, string path)
        {
            string data_string = ArrayToString(ToStringArray(data));
            Var var = new Var(name, data_string, Type.Boolean, DataType.Array);
            string previousFileContent = File.ReadAllText(path);
            File.WriteAllText(path, previousFileContent + VarToString(var));
        }
        public static void SaveDoubleArray(double[] data, string name, string path)
        {
            string data_string = ArrayToString(ToStringArray(data));
            Var var = new Var(name, data_string, Type.Double, DataType.Array);
            string previousFileContent = File.ReadAllText(path);
            File.WriteAllText(path, previousFileContent + VarToString(var));
        }

        //read methods
        public static string ReadString(string name, string path)
        {
            Var var = GetVar(name, path);
            return var?.Value;
        }
        public static int ReadInt(string name, string path)
        {
            Var var = GetVar(name, path);
            return Convert.ToInt32(var?.Value);
        }
        public static bool ReadBool(string name, string path)
        {
            Var var = GetVar(name, path);
            if (var.Value == "true")
            {
                return true;
            }
            return false;
        }
        public static double ReadDouble(string name, string path)
        {
            Var var = GetVar(name, path);
            return Convert.ToDouble(var?.Value);
        }

        //array read methods
        public static int[] ReadIntArray(string name, string path)
        {
            Var var = GetVar(name, path);
            return GetIntArray(var?.Value);
        }
        public static double[] ReadDoubleArray(string name, string path)
        {
            Var var = GetVar(name, path);
            return GetDoubleArray(var?.Value);
        }
        public static bool[] ReadBoolArray(string name, string path)
        {
            Var var = GetVar(name, path);
            return GetBoolArray(var?.Value);
        }
        public static string[] ReadStringArray(string name, string path)
        {
            Var var = GetVar(name, path);
            return GetStringArray(var?.Value);
        }

        //replace/rewrite methods
        public static void RewriteString(string name, string data, string path)
        {
            Remove(name, path);
            SaveString(data, name, path);
        }
        public static void RewriteInt(string name, int data, string path)
        {
            Remove(name, path);
            SaveInt(data, name, path);
        }
        public static void RewriteDouble(string name, double data, string path)
        {
            Remove(name, path);
            SaveDouble(data, name, path);
        }
        public static void RewriteBool(string name, bool data, string path)
        {
            Remove(name, path);
            SaveBool(data, name, path);
        }
        public static void RewriteBoolArray(string name, bool[] data, string path)
        {
            Remove(name, path);
            SaveBoolArray(data, name, path);
        }
        public static void RewriteDoubleArray(string name, double[] data, string path)
        {
            Remove(name, path);
            SaveDoubleArray(data, name, path);
        }
        public static void RewriteIntArray(string name, int[] data, string path)
        {
            Remove(name, path);
            SaveIntArray(data, name, path);
        }
        public static void RewriteStringArray(string name, string[] data, string path)
        {
            Remove(name, path);
            SaveStringArray(data, name, path);
        }

        //remove methods
        public static void Remove(string name, string path)
        {
            string[] oldFile = ReadAllString(path);
            List<string> file = new List<string>();
            string result = "";
            for (int i = 0; i < oldFile.Length; i++)
            {
                if (GetName(oldFile[i]) != name)
                {
                    file.Add(oldFile[i]);
                }
            }
            for (int i = 0; i < file.Count; i++)
            {
                result = result + file[i];
            }
            File.WriteAllText(path, result);
        }

        //support methods
        private static int[] GetIntArray(string input)
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
        private static string[] GetStringArray(string input)
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
        private static double[] GetDoubleArray(string input)
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
        private static bool[] GetBoolArray(string input)
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
        private static string ArrayToString(string[] array)
        {
            string result = "";
            for (int i = 0; i < array.Length; i++)
            {
                result = result + "|" + array[i];
            }
            return result.Substring(1) + ".";
        }
        private static string[] ToStringArray(int[] array)
        {
            string[] result = new string[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i].ToString();
            }
            return result;
        }
        private static string[] ToStringArray(bool[] array)
        {
            string[] result = new string[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i].ToString();
            }
            return result;
        }
        private static string[] ToStringArray(double[] array)
        {
            string[] result = new string[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i].ToString();
            }
            return result;
        }
        private static string GetName(string line)
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
            Console.WriteLine(line.Substring(start + 1, end - start - 1));
            return line.Substring(start + 1, end - start - 1);
        }
        private static string VarToString(Var input)
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

        private static Var[] GetAsVarArray(string path)
        {
            string[] asString = File.ReadAllLines(path);
            Var[] output = new Var[asString.Length];
            for (int i = 0; i < asString.Length; i++)
            {
                output[i] = new Var(asString[i]);
            }
            return output;
        }
        private static Var GetVar(string name, string path)
        {
            string[] file = ReadAllString(path);
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
        private static string[] ReadAllString(string path)
        {
            List<string> fileList = new List<string>();
            string file = File.ReadAllText(path);
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
        public static string[] GetAllNames(string path)
        {
            string[] lines = ReadAllString(path);
            string[] names = new string[lines.Length];
            for(int i = 0; i < lines.Length; i++)
            {
                names[i] = GetName(lines[i]);
            }
            return names;
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
            string temp;
            switch (temp = line.Substring(points[0], points[1] - points[0]))
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
            switch (temp = line.Substring(1, points[0] - 2))
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
