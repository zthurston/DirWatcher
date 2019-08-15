using DirWatcher.Models;

namespace DirWatcher
{
    public interface IDirectoryStateTracker
    {
        event NewFileScanned OnNewFileScanned;
        event TrackedFileMissing OnTrackedFileMissing;
        event TrackedFileModified OnTrackedFileModified;

        void OnDirectoryScanned(DirectoryScannedEventArgs eventArgs);
    }
}