using System;
using System.Text;

namespace DirWatcher.Models
{
    public class LineCount
    {
        public LineCount(int count)
        {
            Count = count;
        }

        public DateTime TimeStamp { get; } = DateTime.UtcNow;
        public int Count { get; }
    }
}
