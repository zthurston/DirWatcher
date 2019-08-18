using System;
using System.Threading;
using System.Threading.Tasks;

namespace DirWatcher.Tasks
{
    public interface ICancellableTaskRunner
    {
        ICancellableTask Run(Func<CancellationToken, Task> func, bool onThreadPoolThread = false);
    }
}
