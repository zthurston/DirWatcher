using DirWatcher.Models;

namespace DirWatcher
{
    public interface ILineCountTracker
    {
        event LineCountCreated OnLineCountCreated;
        event LineCountUpdated OnLineCountUpdated;

        void OnFileChanged(TrackedFileEventArgs file);
        void OnFileRemoved(TrackedFileEventArgs file);
    }
}