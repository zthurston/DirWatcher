using System.Threading;
using System.Threading.Tasks;

namespace DirWatcher
{
    public interface IDirWatcher
    {
        event DirectoryScanned OnDirectoryScanned;

        Task Run(CancellationToken token);
    }
}