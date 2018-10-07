using EventArgs.FileSystemLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemLib
{
    public class FileSystemVisitor
    {
        public Func<FileSystemInfo, bool> filter;
        public FileSystemVisitor() { }
        public FileSystemVisitor(Func<FileSystemInfo, bool> Filter)
        {
            filter = Filter;
        }

        private ActionType CurrentAction { get; set; }

        #region Events

        public event Action Start;
        public event Action Finish;
        public event Action<FileSystemInfoEventArgs<FileInfo>> FileFinded;
        public event Action<FileSystemInfoEventArgs<DirectoryInfo>> DirectoryFinded;
        public event Action<FileSystemInfoEventArgs<FileInfo>> FilteredFileFinded;
        public event Action<FileSystemInfoEventArgs<DirectoryInfo>> FilteredDirectoryFinded;

        #endregion

        /// <summary>
        /// This method returns all finded directories and files in linear order.
        /// </summary>
        /// <param name="entryPoint">Start point in file system.</param>
        /// <returns>Collection of all finded directories and files.</returns>
        public IEnumerable<FileSystemInfo> GetFiles(string entryPoint)
        {
            Start?.Invoke();
            foreach (var fileSystemInfo in GetFilesRecursive(new DirectoryInfo(entryPoint)))
            {
                yield return fileSystemInfo;
            }
            Finish?.Invoke();
        }

        #region Private

        private IEnumerable<FileSystemInfo> GetFilesRecursive(DirectoryInfo entryDirectory)
        {
            var itemValidation = new ItemValidation();

            foreach (var current in entryDirectory.EnumerateFileSystemInfos())
            {
                if (current is FileInfo file)
                {
                    CurrentAction = itemValidation.Validate(file, filter, FileFinded, FilteredFileFinded);
                }

                if (current is DirectoryInfo directory)
                {
                    CurrentAction = itemValidation.Validate(directory, filter, DirectoryFinded, FilteredDirectoryFinded);
                    if (CurrentAction == ActionType.Continue)
                    {
                        yield return current;
                        foreach (var currentDir in GetFilesRecursive(new DirectoryInfo(current.FullName)))
                        {
                            yield return currentDir;
                        }
                    }
                }
                else if (CurrentAction == ActionType.Stop)
                {
                    yield break;
                }
                else
                {
                    yield return current;
                }
            }
        }

        #endregion
    }
}
