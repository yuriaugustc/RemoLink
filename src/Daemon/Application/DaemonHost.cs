namespace Daemon.Application
{
    public class DaemonHost
    {
        private static readonly Lazy<DaemonHost> _instance = new(() => new());
        private DaemonHost() { }

        public static DaemonHost GetInstance() => _instance.Value;

        private readonly CancellationTokenSource _cts = new();

        public CancellationToken Token => _cts.Token;

        public void Stop() 
        {
            _cts.Cancel();
        }
    }
}
