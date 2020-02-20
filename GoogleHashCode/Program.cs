using System;
using System.IO;

namespace GoogleHashCode
{
    public static class Program
    {
        public static void Main()
        {
            string strExeFilePath = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;
            string inputFolderPath = $"{strExeFilePath}{Path.DirectorySeparatorChar}Inputs";
            Console.WriteLine($"Reading input files from {inputFolderPath}");

            foreach (var file in Directory.GetFiles(inputFolderPath))
            {
                Console.WriteLine($"Processing {file}");
                var mng = new BookManager(file);
                mng.Manage();
            }
        }
    }
}
