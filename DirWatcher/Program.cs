using CommandLine;
using System;

namespace DirWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            IDirWatcher watcher = new DirWatcher();
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(o => watcher.Run(o));
        }
    }
}
