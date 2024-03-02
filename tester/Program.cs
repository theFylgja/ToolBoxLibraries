using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Security.Cryptography;

namespace Testing
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine(Test.Hub(@"C:\Main\Cs\SCP\Tests\Project\Assets\random.svg"));
            Console.WriteLine(Test.Hub_s(@"C:\Main\Cs\SCP\Tests\Project\Assets\random.svg"));
        }
    }
    public class Test
    {
        public static string Hub(string filePath)
        {
            byte[] tmpSource;
            byte[] tmpHash;
            tmpSource = File.ReadAllBytes(filePath);
            tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tmpHash.Length; i++)
            {
                sb.Append(tmpHash[i].ToString("x2"));
            }

            return sb.ToString();
        }
        public static string Hub_s(string filePath)
        {
            byte[] tmpSource;
            byte[] tmpHash;
            tmpSource = ASCIIEncoding.ASCII.GetBytes(File.ReadAllText(filePath));
            tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tmpHash.Length; i++)
            {
                sb.Append(tmpHash[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}


