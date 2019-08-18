using System;

namespace DirWatcher.Models
{
    public class LineCountUpdatedEventArgs : EventArgs
    {
        public LineCountUpdatedEventArgs(string filePath, LineCount current, LineCount previous)
        {
            FilePath = filePath;
            Current = current;
            Previous = previous;
        }
        public string FilePath { get; }
        public LineCount Current { get; }
        public LineCount Previous { get; }
        public DateTime TimeStamp { get; } = DateTime.UtcNow;
    }
}
