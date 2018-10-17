using FileSystemEventArgs.FileSystemLib;
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
        private IFakeClass _fakeClass;

        public bool Stop { get; set; }

        public Func<FileSystemInfo, bool> filter;

        public IFakeClass FakeClass
        {
            get
            {
                if (_fakeClass == null)
                {
                    _fakeClass = new FakeClass();
                }
                return _fakeClass;
            }
            set { }
        }

        public FileSystemVisitor() { }
        public FileSystemVisitor(Func<FileSystemInfo, bool> Filter)
        {
            filter = Filter;
        }
        public FileSystemVisitor(IFakeClass fakeClass)
        {
            _fakeClass = fakeClass;
        }

        #region Events

        public event EventHandler Start;
        public event EventHandler Finish;
        public event EventHandler<FileSystemInfoEventArgs<FileInfo>> FileFinded;
        public event EventHandler<FileSystemInfoEventArgs<DirectoryInfo>> DirectoryFinded;
        public event EventHandler<FileSystemInfoEventArgs<FileInfo>> FilteredFileFinded;
        public event EventHandler<FileSystemInfoEventArgs<DirectoryInfo>> FilteredDirectoryFinded;

        #endregion

        /// <summary>
        /// This method returns all finded directories and files in linear order.
        /// </summary>
        /// <param name="entryPoint">Start point in file system.</param>
        /// <returns>Collection of all finded directories and files.</returns>
        public IEnumerable<FileSystemInfo> GetFiles(string entryPoint)
        {
            Stop = false;
            OnStartEvent(Start);
            foreach (var fileSystemInfo in GetFilesRecursive(new DirectoryInfo(entryPoint)))
            {
                if (Stop)
                {
                    break;
                }
                yield return fileSystemInfo;
            }
            OnFinishEvent(Finish);
        }

        public IEnumerable<FileSystemInfo> GetFilesRecursive(DirectoryInfo entryDirectory)
        {
            if (FakeClass.ThrowException)
            {
                throw new ArgumentException("GetFilesRecursive method thrown an exception.");
            }

            bool skip = false;

            foreach (var current in entryDirectory.EnumerateFileSystemInfos())
            {
                if (current is FileInfo file)
                {
                    skip = Validate(file, filter, FileFinded, FilteredFileFinded);
                    if (!skip)
                    {
                        yield return current;
                    }
                }

                if (current is DirectoryInfo directory)
                {
                    skip = Validate(directory, filter, DirectoryFinded, FilteredDirectoryFinded);
                    if (!skip)
                    {
                        yield return current;
                        foreach (var currentDir in GetFilesRecursive(new DirectoryInfo(current.FullName)))
                        {
                            yield return currentDir;
                        }
                    }

                }
            }
        }

        #region OnEvents

        protected virtual void OnStartEvent(EventHandler eventHandler)
        {
            var x = eventHandler;
            if (x != null)
            {
                x(this, new EventArgs());
            }
        }

        protected virtual void OnFinishEvent(EventHandler eventHandler)
        {
            var x = eventHandler;
            if (x != null)
            {
                x(this, new EventArgs());
            }
        }

        protected virtual void OnItemFindedEvent<T>(EventHandler<FileSystemInfoEventArgs<T>> eventHandler, FileSystemInfoEventArgs<T> eventArgs) where T : FileSystemInfo
        {
            var x = eventHandler;
            if (x != null)
            {
                x(this, eventArgs);
            }
        }

        protected virtual void OnFilteredItemFindedEvent<T>(EventHandler<FileSystemInfoEventArgs<T>> eventHandler, FileSystemInfoEventArgs<T> eventArgs) where T : FileSystemInfo
        {
            var x = eventHandler;
            if (x != null)
            {
                x(this, eventArgs);
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Identicate what should be done with current file system item.
        /// </summary>
        /// <typeparam name="T">FileSystemInfo type.</typeparam>
        /// <param name="fileSystemInfoItem">File System Info object that needs to be validated.</param>
        /// <param name="filter">Filter Delegate.</param>
        /// <param name="itemFound">Event on File System Item Finded.</param>
        /// <param name="itemFiltered">Event on File System Item Filtered.</param>
        /// <returns>Action that needs to be performed.</returns>
        private bool Validate<T>(T fileSystemInfoItem,
            Func<T, bool> filter,
            EventHandler<FileSystemInfoEventArgs<T>> itemFound,
            EventHandler<FileSystemInfoEventArgs<T>> itemFiltered) where T : FileSystemInfo
        {
            FileSystemInfoEventArgs<T> eventArgs = new FileSystemInfoEventArgs<T>(fileSystemInfoItem, false);

            OnItemFindedEvent(itemFound, eventArgs);

            if (filter == null || eventArgs.Skip)
            {
                return eventArgs.Skip;
            }

            if (filter(fileSystemInfoItem))
            {
                eventArgs = new FileSystemInfoEventArgs<T>(fileSystemInfoItem, false);
                OnItemFindedEvent(itemFiltered, eventArgs);
                return eventArgs.Skip;
            }

            return false;
        }

        #endregion
    }
}
