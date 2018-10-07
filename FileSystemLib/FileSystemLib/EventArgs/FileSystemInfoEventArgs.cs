using FileSystemLib;
using System.IO;

namespace EventArgs.FileSystemLib
{
    public class FileSystemInfoEventArgs<T> where T : FileSystemInfo
    {
        public T FileSystemInfoItem { get; }
        public ActionType ActionType { get; set; }

        public FileSystemInfoEventArgs(T fileSystemInfoItem, ActionType actionType)
        {
            FileSystemInfoItem = fileSystemInfoItem;
            ActionType = actionType;
        }
    }
}
