using EventArgs.FileSystemLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemLib
{
    public class ItemValidation
    {
        /// <summary>
        /// Identicate what should be done with current file system item.
        /// </summary>
        /// <typeparam name="T">FileSystemInfo type.</typeparam>
        /// <param name="fileSystemInfoItem">File System Info object that needs to be validated.</param>
        /// <param name="filter">Filter Delegate.</param>
        /// <param name="itemFound">Event on File System Item Finded.</param>
        /// <param name="itemFiltered">Event on File System Item Filtered.</param>
        /// <returns>Action that needs to be performed.</returns>
        public ActionType Validate<T>(T fileSystemInfoItem, 
            Func<T, bool> filter, 
            Action<FileSystemInfoEventArgs<T>> itemFound, 
            Action<FileSystemInfoEventArgs<T>> itemFiltered) where T : FileSystemInfo
        {
            FileSystemInfoEventArgs<T> eventArgs = new FileSystemInfoEventArgs<T>(fileSystemInfoItem, ActionType.Continue);

            itemFound?.Invoke(eventArgs);

            if(filter == null || eventArgs.ActionType != ActionType.Continue)
            {
                return eventArgs.ActionType;
            }

            if (filter(fileSystemInfoItem))
            {
                eventArgs = new FileSystemInfoEventArgs<T>(fileSystemInfoItem, ActionType.Continue);
                itemFiltered?.Invoke(eventArgs);
                return eventArgs.ActionType;
            }

            return ActionType.Continue;
        }
    }
}
