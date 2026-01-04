namespace Server.Services.Tunnel
{
    public class TunnelCleanUp(TunnelRegistry registry) : BackgroundService
    {
        private readonly TunnelRegistry _tunnels = registry;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _tunnels.CleanUpExpiredTunnels();

                Task.Delay(TimeSpan.FromMinutes(1), stoppingToken)
                    .Wait(stoppingToken);
            }

            return Task.CompletedTask;
        }
    }
}
