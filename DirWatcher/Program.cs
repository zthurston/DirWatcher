using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using DirWatcher.Models;
using DirWatcher.Tasks;

namespace DirWatcher
{
    class Program
    {
#if DEBUG
        const int ScanIntervalMs = 1_000;
#else
        const int ScanIntervalMs = 10_000_000;
#endif
        static CancellationTokenSource CancellationTokenSource;
        static CancellableTaskRunner CancellableTaskRunner;
        static IDirWatcher Watcher;
        static IDirectoryStateTracker StateTracker;
        static ILineCounter LineCounter;
        static ILineCountTracker LineCountTracker;

        static async Task<int> Main(string[] args)
        {
#if DEBUG
            //args = new[] { @".\Test Folder\", "*.*" };
            //args = new string[] { @"C:\Users\zthurston\Downloads\Test Folder\", "A*.*" };
            //args = new string[] { @"\\localhost\Test Folder", "*.txt" };
#endif

            CancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += Console_CancelKeyPress;
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(ConfigureWatcher);

            if (Watcher == null)
                return -1;

            await RunWatcher();
            return 0;
        }

        private static async Task RunWatcher()
        {
            try
            {
                Console.WriteLine("Starting Watcher");
                await Task.Run(() => Watcher.Run(CancellationTokenSource.Token));
                Console.WriteLine("Finished Watcher");
            }
            finally
            {
                Cleanup();
            }

            Console.WriteLine("Finished");
        }

        private static void ConfigureWatcher(CommandLineOptions options)
        {
            Watcher = new DirWatcher(TimeSpan.FromMilliseconds(ScanIntervalMs),
                       options.Directory, options.WatchPattern);
            Watcher.OnDirectoryScanned += OnDirectoryScanned;

            StateTracker = new DirectoryStateTracker(CancellationTokenSource.Token);
            Watcher.OnDirectoryScanned += StateTracker.OnDirectoryScanned;

            StateTracker.OnTrackedFileModified += StateTracker_OnTrackedFileModified;
            StateTracker.OnTrackedFileMissing += StateTracker_OnTrackedFileMissing;

            LineCounter = new LineCounter(CancellationTokenSource.Token);
            CancellableTaskRunner = new CancellableTaskRunner((task, onCancel) => new CancellableTask(task, onCancel));
            LineCountTracker = new LineCountTracker(CancellationTokenSource.Token, CancellableTaskRunner, LineCounter);
            LineCountTracker.OnLineCountFailed += LineCountTracker_OnLineCountFailed;

            StateTracker.OnNewFileScanned += LineCountTracker.OnFileChanged;
            StateTracker.OnTrackedFileModified += LineCountTracker.OnFileChanged;
            StateTracker.OnTrackedFileMissing += LineCountTracker.OnFileRemoved;
            LineCountTracker.OnLineCountCreated += LineCountTracker_OnLineCountCreated;
            LineCountTracker.OnLineCountUpdated += LineCountTracker_OnLineCountUpdated;
        }

        private static void LineCountTracker_OnLineCountFailed(LineCountFailedEventArgs args)
        {
            Task.Run(() =>
            {
                Console.WriteLine($"Counting lines for file: {args.FilePath} failed: {args.Exception}");

                // NOTE: here is where we'd hook up support for retrying line count failures
                // when files are locked or some other failure occurs
            });
        }

        private static void LineCountTracker_OnLineCountUpdated(LineCountUpdatedEventArgs args)
        {
            Task.Run(() =>
            {
                int delta = args.Current.Count - args.Previous.Count;
                char indicator = delta >= 0 ? '+' : '-';
                Console.WriteLine($"File: {args.FilePath} Line count Changed:{indicator}{Math.Abs(delta)}");
            });
        }

        private static void LineCountTracker_OnLineCountCreated(LineCountCreatedEventArgs args)
        {
            Task.Run(() =>
            {
                Console.WriteLine($"File: {args.FilePath} Line count: {args.Current.Count}");
            });
        }

        private static void StateTracker_OnTrackedFileMissing(TrackedFileEventArgs eventArgs)
        {
            Task.Run(() =>
            {
                Console.WriteLine($"Tracked file missing:  {eventArgs.Path} at {eventArgs.TimeStamp.TimeOfDay}");
            });
        }

        private static void StateTracker_OnTrackedFileModified(TrackedFileEventArgs eventArgs)
        {
            Task.Run(() =>
            {
                Console.WriteLine($"Tracked file Modified: {eventArgs.Path} at {eventArgs.TimeStamp.TimeOfDay}");
            });
        }

        private static void OnDirectoryScanned(DirectoryScannedEventArgs eventArgs)
        {
            Task.Run(() =>
            {
                Console.WriteLine($"Scanning for changes at {DateTime.UtcNow.TimeOfDay}");
                // Console.WriteLine($"Found files: {string.Join(' ', eventArgs.FilePathModificationTimes.Keys)} in {eventArgs.Directory} at {eventArgs.TimeStamp.ToLocalTime()}");
            });
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Cancelling watch...");
            CancellationTokenSource.Cancel();
        }

        private static void Cleanup()
        {
            // NOTE: not strictly necessary, as the process ending will
            // clean everything up anyway.
            Console.CancelKeyPress -= Console_CancelKeyPress;
            Watcher.OnDirectoryScanned -= OnDirectoryScanned;
            Watcher.OnDirectoryScanned -= StateTracker.OnDirectoryScanned;

            StateTracker.OnTrackedFileModified -= StateTracker_OnTrackedFileModified;
            StateTracker.OnTrackedFileMissing -= StateTracker_OnTrackedFileMissing;

            StateTracker.OnNewFileScanned -= LineCountTracker.OnFileChanged;
            StateTracker.OnTrackedFileModified -= LineCountTracker.OnFileChanged;
            LineCountTracker.OnLineCountCreated -= LineCountTracker_OnLineCountCreated;
            LineCountTracker.OnLineCountUpdated -= LineCountTracker_OnLineCountUpdated;
            LineCountTracker.OnLineCountFailed -= LineCountTracker_OnLineCountFailed;
            StateTracker.OnTrackedFileMissing -= LineCountTracker.OnFileRemoved;

            CancellableTaskRunner?.Dispose();
            CancellationTokenSource?.Dispose();
        }
    }
}
