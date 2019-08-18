using System;
using System.Threading.Tasks;

namespace DirWatcher.Tasks
{
    /// <summary>
    /// Wrapper around Task for making cancellation of tasks simpler
    /// </summary>
    public class CancellableTask : ICancellableTask
    {
        public CancellableTask(Task task, Action onCancel)
        {
            OnCancel = onCancel;
            Task = task;
        }

        protected Action OnCancel { get; }
        public Task Task { get; }

        public bool IsCancelled { get; protected set; }

        public void Cancel()
        {
            if (!IsCancelled)
            {
                OnCancel?.Invoke();
                IsCancelled = true;
            }
        }
    }
}
