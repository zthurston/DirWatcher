using System;
using System.Threading;
using System.Threading.Tasks;

namespace DirWatcher
{
    public class MockLineCounter : ILineCounter
    {
        private static readonly Random Random = new Random();
        public MockLineCounter(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }

        protected CancellationToken CancellationToken { get; }

        public async Task<int> CountLinesAsync(string filePath, CancellationToken cancellationToken)
        {
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken))
            {
                await Task.Delay(Random.Next(500, 5_000), cts.Token);
            }

            return Random.Next(0, 2_000_000_000);
        }
    }
}
