using System;

namespace DirWatcher
{
    public class DirWatcher : IDirWatcher
    {
        public void Run(CommandLineOptions options)
        {
            Console.ReadKey();
        }
    }
}
