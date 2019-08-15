using System;

namespace DirWatcher.Models
{
    public class TrackedFileEventArgs : EventArgs
    {
        public TrackedFileEventArgs(string path)
        {
            Path = path;
        }

        public string Path { get; }
        public DateTime TimeStamp { get; } = DateTime.UtcNow;
    }
}
