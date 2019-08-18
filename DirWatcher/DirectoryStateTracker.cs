using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DirWatcher.Models;

namespace DirWatcher
{
    public delegate void NewFileScanned(TrackedFileEventArgs file);
    public delegate void TrackedFileModified(TrackedFileEventArgs file);
    public delegate void TrackedFileMissing(TrackedFileEventArgs file);

    public class DirectoryStateTracker : IDirectoryStateTracker
    {
        public DirectoryStateTracker(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }

        public event NewFileScanned OnNewFileScanned;
        public event TrackedFileModified OnTrackedFileModified;
        public event TrackedFileMissing OnTrackedFileMissing;

        protected CancellationToken CancellationToken { get; }
        protected IDictionary<string, DateTime> TrackedPaths = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        public void OnDirectoryScanned(DirectoryScannedEventArgs eventArgs)
        {
            eventArgs = eventArgs ?? throw new ArgumentNullException(nameof(eventArgs));

            if (CancellationToken.IsCancellationRequested)
                return;

            CheckForNewOrModifiedFiles(eventArgs);

            CheckForDeletedFiles(eventArgs);
        }

        protected void CheckForNewOrModifiedFiles(DirectoryScannedEventArgs eventArgs)
        {
            foreach (var filePathModificationTime in eventArgs.FilePathModificationTimes)
            {
                var trackedFileEventArgs = new TrackedFileEventArgs(filePathModificationTime.Key);
                if (TrackedPaths.TryGetValue(filePathModificationTime.Key, out var value))
                {
                    if (value < filePathModificationTime.Value)
                        OnTrackedFileModified?.Invoke(trackedFileEventArgs);
                    TrackedPaths[filePathModificationTime.Key] = filePathModificationTime.Value;
                }
                else
                {
                    TrackedPaths.Add(filePathModificationTime);
                    OnNewFileScanned?.Invoke(trackedFileEventArgs);
                }
            }
        }

        protected void CheckForDeletedFiles(DirectoryScannedEventArgs eventArgs)
        {
            foreach (var missingFilePath in TrackedPaths.Keys.Except(eventArgs.FilePathModificationTimes.Keys).ToList())
            {
                var missingFileEventArgs = new TrackedFileEventArgs(missingFilePath);
                TrackedPaths.Remove(missingFilePath);
                OnTrackedFileMissing?.Invoke(missingFileEventArgs);
            }
        }
    }
}
