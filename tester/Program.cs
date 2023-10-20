using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;

namespace Testing
{
    class Program
    {
        static void Main()
        {
            Test.Hub();
        }
    }
    public class Test
    {
        public static void Hub()
        {
            Assembly assembly = Assembly.LoadFrom(@"C:\WinTools\Extensions\supp\Bowl.dll");
            Type type = assembly.GetTypes()[0];
            var obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("SaveString");
            method.Invoke(obj, new object[] { "testString", "testVar", @"D:\Main\test.bgdf" });
            MethodInfo read = type.GetMethod("ReadString");
            string result = (string)read.Invoke(obj, new object[] { "testVar", @"D:\Main\test.bgdf" });
            return;
        }
        public void Setup()
        {
            return;
        }
    }
}


