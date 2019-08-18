using System.Threading;
using System.Threading.Tasks;

namespace DirWatcher
{
    public interface ILineCounter
    {
        Task<int> CountLinesAsync(string filePath, CancellationToken token);
    }
}
