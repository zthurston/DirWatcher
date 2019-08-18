using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DirWatcher.Tasks
{
    /// <summary>
    /// Handles managing lifecycle of CancellationTokenSources
    /// </summary>
    public class CancellableTaskRunner : IDisposable, ICancellableTaskRunner
    {
        public CancellableTaskRunner(
            Func<Task, Action, ICancellableTask> cancellableTaskFactory
        )
        {
            CancellableTaskFactory = cancellableTaskFactory;
            Disposables = new List<IDisposable>();
        }
        protected List<IDisposable> Disposables { get; }

        protected Func<Task, Action, ICancellableTask> CancellableTaskFactory { get; }

        public ICancellableTask Run(Func<CancellationToken, Task> func, bool onThreadPoolThread = false)
        {
            var cts = CreateCancellationTokenSource();
            var runningTask = onThreadPoolThread
                ? Task.Run(() => func.Invoke(cts.Token))
                : func.Invoke(cts.Token);

            return CancellableTaskFactory.Invoke(runningTask, () =>
            {
                using (cts)
                    cts.Cancel();

                lock (Disposables)
                    Disposables.Remove(cts);
            });
        }

        protected CancellationTokenSource CreateCancellationTokenSource()
        {
            var cts = new CancellationTokenSource();
            lock (Disposables)
                Disposables.Add(cts);
            return cts;
        }

        public void Dispose()
        {
            foreach (var d in Disposables)
                d?.Dispose();

            Disposables.Clear();
        }
    }
}
