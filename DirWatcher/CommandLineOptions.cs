using CommandLine;

namespace DirWatcher
{
    public class CommandLineOptions
    {
        [Value(0, Required = true, MetaName = nameof(Directory), HelpText = "Directory to watch as absolute or relative path or UNC path")]
        public string Directory { get; set; }

        [Value(1, Required = true, MetaName = nameof(WatchPattern), HelpText = "Pattern of files for which to watch for changes")]
        public string WatchPattern { get; set; }
    }
}
