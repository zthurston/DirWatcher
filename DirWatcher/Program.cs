using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using DirWatcher.Models;

namespace DirWatcher
{
    class Program
    {
        const int ScanIntervalMs = 1_000;
        static CancellationTokenSource CancellationTokenSource;
        static IDirWatcher Watcher;

        static async Task<int> Main(string[] args)
        {

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
                await Task.Run(() => Watcher.Run(CancellationTokenSource.Token));
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
        }

        private static void OnDirectoryScanned(DirectoryScannedEventArgs eventArgs)
        {
            Console.WriteLine($"Found files: {string.Join(' ', eventArgs.FilePaths)} in {eventArgs.Directory} at {eventArgs.TimeStamp.ToLocalTime()}");
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
        }
    }
}
