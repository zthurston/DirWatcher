﻿using DirWatcher.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DirWatcher
{
    public delegate void DirectoryScanned(DirectoryScannedEventArgs eventArgs);

    /// <summary>
    /// Scans a directory for files matching a pattern, on a refresh interval, broadcasting current state on each interval
    /// </summary>
    public class DirWatcher : IDirWatcher
    {
        public DirWatcher(TimeSpan refreshInterval, string directory, string watchPattern)
        {
            RefreshInterval = refreshInterval;
            WatchDirectory = directory ?? throw new ArgumentNullException(nameof(WatchDirectory));
            WatchPattern = watchPattern ?? throw new ArgumentNullException(nameof(WatchPattern));
        }

        public TimeSpan RefreshInterval { get; }
        public string WatchDirectory { get; }
        public string WatchPattern { get; }

        public event DirectoryScanned OnDirectoryScanned;

        public async Task Run(CancellationToken token)
        {
            Console.WriteLine("Starting Watch...");

            try
            {
                while (!token.IsCancellationRequested)
                {
                    Console.WriteLine("Scanning...");
                    var files = Directory.GetFiles(WatchDirectory, WatchPattern);
                    OnDirectoryScanned?.Invoke(new DirectoryScannedEventArgs(WatchDirectory, files));

                    if (!token.IsCancellationRequested)
                        await Task.Delay(RefreshInterval, token);
                }
            }
            catch(TaskCanceledException)
            {
                Debug.WriteLine($"{(nameof(DirWatcher))}.{nameof(Run)}() Cancelled");
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"{nameof(DirWatcher)}.{nameof(Run)} exception: {ex.ToString()}");
                throw;
            }

            Console.WriteLine("Finished watch");
        }
    }
}
