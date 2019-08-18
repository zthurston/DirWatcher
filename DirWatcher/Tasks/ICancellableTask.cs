using System.Threading.Tasks;

namespace DirWatcher.Tasks
{
    public interface ICancellableTask
    {
        Task Task { get; }
        bool IsCancelled { get; }

        void Cancel();
    }
}
