using DirWatcher.Models;

namespace DirWatcher
{
    public interface ILineCountTracker
    {
        event LineCountCreated OnLineCountCreated;
        event LineCountUpdated OnLineCountUpdated;
        event LineCountFailed OnLineCountFailed;

        void OnFileChanged(TrackedFileEventArgs file);
        void OnFileRemoved(TrackedFileEventArgs file);
    }
}