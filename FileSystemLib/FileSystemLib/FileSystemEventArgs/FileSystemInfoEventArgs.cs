using System;
using System.IO;

namespace FileSystemEventArgs.FileSystemLib
{
    public class FileSystemInfoEventArgs<T> : EventArgs where T : FileSystemInfo
    {
        public T FileSystemInfoItem { get; }
        public bool Skip { get; set; }

        public FileSystemInfoEventArgs(T fileSystemInfoItem, bool skip)
        {
            FileSystemInfoItem = fileSystemInfoItem;
            Skip = skip;
        }
    }
}
