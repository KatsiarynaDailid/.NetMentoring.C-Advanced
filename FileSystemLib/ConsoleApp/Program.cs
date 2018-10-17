using FileSystemLib;
using System;
using System.Configuration;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string startPoint = ConfigurationManager.AppSettings["StartPointPath"];
            string extention = ConfigurationManager.AppSettings["ExtensionToFilterBy"];
            string fileToSkip = ConfigurationManager.AppSettings["FileToSkip"];
            string directoryToStop = ConfigurationManager.AppSettings["DirectoryToStop"];

            FileSystemVisitor fsv = new FileSystemVisitor((f) =>
            {               
                return f.Extension != extention;
            });

            fsv.Start += (s, e) =>
            {
                Console.WriteLine($"Start of iteration: \nTime Stamp: {DateTime.Now.ToString("hh:mm:ss.fff")}.");
            };

            fsv.Finish += (s, e) =>
            {
                Console.WriteLine($"Finish of iteration: \nTime Stamp: {DateTime.Now.ToString("hh:mm:ss.fff")}.");
            };

            fsv.FileFinded += (s, eventArgs) =>
            {
                Console.WriteLine($"\tFounded file: {eventArgs.FileSystemInfoItem.Name}");
            };

            fsv.DirectoryFinded += (s, eventArgs) =>
            {
                Console.WriteLine($"\tFounded directory: {eventArgs.FileSystemInfoItem.Name}");
            };

            fsv.FilteredFileFinded += (s, eventArgs) =>
            {
                var name = eventArgs.FileSystemInfoItem.Name;
                Console.WriteLine($"\tFounded filtered file:  {name}");         
                if (name == fileToSkip)
                {
                    Console.WriteLine($"File '{name}' skipped.");
                    eventArgs.Skip = true;
                }
            };

            fsv.FilteredDirectoryFinded += (s, eventArgs) =>
            {
                var name = eventArgs.FileSystemInfoItem.Name;
                Console.WriteLine($"\tFounded filtered directory:  {name}");            
                if (name == directoryToStop)
                {
                    Console.WriteLine($"Stopped on directory '{name}'.");                  
                    ((FileSystemVisitor)s).Stop = true;
                }
            };

            foreach (var fileSystemInfo in fsv.GetFiles(startPoint))
            {
                Console.WriteLine(fileSystemInfo.Name);
            }
            Console.ReadLine();
        }
    }
}
