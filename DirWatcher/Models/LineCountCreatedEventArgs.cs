using System;

namespace DirWatcher.Models
{
    public class LineCountCreatedEventArgs: EventArgs
    {
        public LineCountCreatedEventArgs(string filePath, LineCount current)
        {
            FilePath = filePath;
            Current = current;
        }
        public string FilePath { get; }
        public LineCount Current { get; }
        public DateTime TimeStamp { get; } = DateTime.UtcNow;
    }
}
