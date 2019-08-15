using System;
using System.Collections.Generic;

namespace DirWatcher.Models
{
    public class DirectoryScannedEventArgs : EventArgs
    {
        public DirectoryScannedEventArgs(string directory, ICollection<string> filePaths)
        {
            Directory = directory;
            FilePaths = filePaths;
        }

        public string Directory { get; }
        public ICollection<string> FilePaths { get; }
        public DateTime TimeStamp { get; } = DateTime.UtcNow;
    }
}
