using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DirWatcher.Models;
using DirWatcher.Tasks;

namespace DirWatcher
{
    public delegate void LineCountCreated(LineCountCreatedEventArgs args);
    public delegate void LineCountUpdated(LineCountUpdatedEventArgs args);
    public delegate void LineCountFailed(LineCountFailedEventArgs args);
    public class LineCountTracker : ILineCountTracker
    {
        public LineCountTracker(
            CancellationToken cancellationToken,
            ICancellableTaskRunner cancellableTaskRunner,
            ILineCounter lineCounter
        )
        {
            LineCounter = lineCounter;
            CancellationToken = cancellationToken;
            CancellableTaskRunner = cancellableTaskRunner;
            OutstandingLineCounts = new Dictionary<string, ICancellableTask>(StringComparer.OrdinalIgnoreCase);
            PathLineCounts = new Dictionary<string, LineCount>();
        }

        public event LineCountCreated OnLineCountCreated;
        public event LineCountUpdated OnLineCountUpdated;
        public event LineCountFailed OnLineCountFailed;

        protected Dictionary<string, ICancellableTask> OutstandingLineCounts { get; }
        protected Dictionary<string, LineCount> PathLineCounts { get; }
        protected ILineCounter LineCounter { get; }
        protected CancellationToken CancellationToken { get; }
        protected ICancellableTaskRunner CancellableTaskRunner { get; }

        public void OnFileChanged(TrackedFileEventArgs file)
        {
            if (CancellationToken.IsCancellationRequested)
                return;

            var filePath = file.Path;

            ICancellableTask task = null;
            // Check if we have an outstanding line count task. If we don't, kick one off
            lock (OutstandingLineCounts)
            {
                if (!OutstandingLineCounts.ContainsKey(filePath))
                {
                    // kick off and add line count task
                    task = CancellableTaskRunner.Run(async token =>
                    {
                        await CalculateLineCount(filePath, token);
                    }, onThreadPoolThread: true);
                 
                    OutstandingLineCounts.Add(filePath, task);
                }
            }

            task?.Task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    OnLineCountFailed?.Invoke(new LineCountFailedEventArgs(filePath, t.Exception));
                }
            });
        }

        public void OnFileRemoved(TrackedFileEventArgs file)
        {
            var filePath = file.Path;
            // Cancel an outstanding line count task for this file, if one exists
            lock (OutstandingLineCounts)
            {
                if (OutstandingLineCounts.TryGetValue(filePath, out var task))
                    task.Cancel();
            }

            lock (PathLineCounts)
            {
                // NOTE: if the key doesn't exist, the method will return null,
                // not throw an exception, per method documentation
                PathLineCounts.Remove(filePath);
            }
        }

        protected async Task CalculateLineCount(string filePath, CancellationToken taskCancellationToken)
        {
            if (IsCancellationRequested())
                return;

            try
            {
                var lineCount = await LineCounter.CountLinesAsync(filePath, taskCancellationToken);
                if (IsCancellationRequested())
                    return;

                var result = AddOrUpdateLineCount(filePath, lineCount);
                if (result.previous == null)
                {
                    OnLineCountCreated?.Invoke(new LineCountCreatedEventArgs(filePath, result.current));
                }
                else
                {
                    OnLineCountUpdated?.Invoke(new LineCountUpdatedEventArgs(filePath, result.current, result.previous));
                }
            }
            finally
            {
                lock (OutstandingLineCounts)
                {
                    OutstandingLineCounts.Remove(filePath);
                }
            }

            bool IsCancellationRequested() => CancellationToken.IsCancellationRequested
                || taskCancellationToken.IsCancellationRequested;
        }

        private (LineCount current, LineCount previous) AddOrUpdateLineCount(string filePath, int count)
        {
            var current = new LineCount(count);
            LineCount previous;

            lock (PathLineCounts)
            {
                if (PathLineCounts.TryGetValue(filePath, out previous))
                {
                    PathLineCounts.Remove(filePath);
                }

                PathLineCounts.Add(filePath, current);
            }

            return (current, previous);
        }
    }
}
