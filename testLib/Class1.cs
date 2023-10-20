using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace testLib
{
    public class Test
    {
        public void Hub()
        {
            return;
        }
        public static void Handle(string[] input, string root)
        {
            if(root == null)
            {
                return;
            }
            Console.WriteLine("lib exec");
            Console.WriteLine(root);
            string[] files = Directory.GetFiles(root);
            foreach (string file in files)
            {
                Console.WriteLine(file);
            }

        }
        public void Setup()
        {
            return;
        }
    }
}
