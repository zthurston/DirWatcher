using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DirWatcher
{
    public class LineCounter : ILineCounter
    {
        public LineCounter(CancellationToken token)
        {
            Token = token;
        }

        public CancellationToken Token { get; }

        public Task<int> CountLinesAsync(string filePath, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            // This implies that File.ReadLines() will work with any sort of line ending:
            // https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader.readline
            // NOTE: If the file has really wide, or no lines, this code will end up allocating the file's or line's width
            // in memory as it reads in until it hides a line ending
            // We would need to implement an algorithm that iterates over the file in chunks, like done in the following
            // post, to avoid this problem: https://nima-ara-blog.azurewebsites.net/counting-lines-of-a-text-file/
            int result = File.ReadLines(filePath).Count();
            return Task.FromResult(result);
        }
    }
}
