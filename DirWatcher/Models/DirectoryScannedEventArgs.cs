using System;
using System.Collections.Generic;

namespace DirWatcher.Models
{
    public class DirectoryScannedEventArgs : EventArgs
    {
        public DirectoryScannedEventArgs(string directory, IDictionary<string, DateTime> filePathModificationTimes)
        {
            Directory = directory;
            FilePathModificationTimes = filePathModificationTimes;
        }

        public string Directory { get; }
        public IDictionary<string, DateTime> FilePathModificationTimes { get; }
        public DateTime TimeStamp { get; } = DateTime.UtcNow;
    }
}
