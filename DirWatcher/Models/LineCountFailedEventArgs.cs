using System;

namespace DirWatcher.Models
{
    public class LineCountFailedEventArgs : EventArgs
    {
        public LineCountFailedEventArgs(string filePath, Exception exception)
        {
            FilePath = filePath;
            Exception = exception;
        }

        public string FilePath { get; }
        public Exception Exception { get; }
    }
}
