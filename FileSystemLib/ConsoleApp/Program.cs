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
            FileSystemVisitor fsv = new FileSystemVisitor((f) =>
            {
                string extention = ConfigurationManager.AppSettings["ExtensionToFilterBy"];
                return f.Extension != extention;
            });

            fsv.Start += () =>
            {
                Console.WriteLine($"Start of iteration: \nTime Stamp: {DateTime.Now.ToString("hh:mm:ss.fff")}.");
            };

            fsv.Finish += () =>
            {
                Console.WriteLine($"Finish of iteration: \nTime Stamp: {DateTime.Now.ToString("hh:mm:ss.fff")}.");
            };

            fsv.FileFinded += (eventArgs) =>
            {
                Console.WriteLine($"\tFounded file: {eventArgs.FileSystemInfoItem.Name}");
            };

            fsv.DirectoryFinded += (eventArgs) =>
            {
                Console.WriteLine($"\tFounded directory: {eventArgs.FileSystemInfoItem.Name}");
            };

            fsv.FilteredFileFinded += (eventArgs) =>
            {
                var name = eventArgs.FileSystemInfoItem.Name;
                Console.WriteLine($"\tFounded filtered file:  {name}");
                string fileToSkip = ConfigurationManager.AppSettings["FileToSkip"];
                if (name == fileToSkip)
                {
                    Console.WriteLine($"File '{name}' skiped.");
                    eventArgs.ActionType = ActionType.Skip;
                }
            };

            fsv.FilteredDirectoryFinded += (eventArgs) =>
            {
                var name = eventArgs.FileSystemInfoItem.Name;
                Console.WriteLine($"\tFounded filtered directory:  {name}");

                string directoryToStop = ConfigurationManager.AppSettings["DirectoryToStop"];
                if (name == directoryToStop)
                {
                    Console.WriteLine($"Stoped on {name}.");
                    eventArgs.ActionType = ActionType.Stop;
                }
            };

            foreach (var fileSystemInfo in fsv.GetFiles(startPoint));       
            Console.ReadLine();
        }
    }
}
