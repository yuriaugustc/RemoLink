using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Requests;
using Shared.Contracts.Responses;
using Shared.Enums;
using Timer = System.Timers.Timer;

namespace Daemon.Infra.WebSocket
{
    internal class TunnelSocket
    {
        private HubConnection? _hub;
        private readonly string _url = "http://localhost:5145/tunnel"; //TODO: Replace with actual URL
        public bool IsConnected => _hub?.State == HubConnectionState.Connected;
        public bool IsReady => _hub is not null;
        private readonly string _sessionId = Guid.NewGuid().ToString();

        private static Lazy<TunnelSocket> Instance { get; } = new(() => new());
        private TunnelSocket() { }

        public static TunnelSocket GetInstance()
        {
            return Instance.Value;
        }

        public void Initialize(CancellationToken token = default)
        {
            if (IsReady) return;

            _hub = new HubConnectionBuilder()
                .WithUrl(_url)
                .WithAutomaticReconnect()
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .Build();

            _hub.Reconnected += async(_) => await ResumeTunnel(token);

            Timer timer = new()
            {
                Interval = TimeSpan.FromSeconds(30).TotalMilliseconds,
                Enabled = true,
                AutoReset = true
            };
            timer.Elapsed += async (_, _) => await Ping(token);
            timer.Start();
        }

        public void EnsureInitialized()
        {
            if (!IsReady)
                throw new InvalidOperationException("Hub connection is not initialized.");
        }

        public async Task StartAsync(CancellationToken token = default)
        {
            EnsureInitialized();
            await _hub!.StartAsync(token);

            await RegisterAsync(token);
        }

        public async Task StopAsync(CancellationToken token = default)
        {
            EnsureInitialized();

            await UnregisterAsync(token);

            await _hub!.StopAsync(token);
        }

        private async Task RegisterAsync(CancellationToken token = default)
        {
            EnsureInitialized();

            bool registered = await _hub!
                .InvokeAsync<bool>("Register", _sessionId, token);

            if (!registered)
                throw new InvalidOperationException("Failed to register session with the server.");
        }

        private async Task UnregisterAsync(CancellationToken token = default)
        {
            EnsureInitialized();
            await _hub!.InvokeAsync("Unregister", _sessionId, token);
        }

        public async Task<ExposeAckResponse> ExposeAsync(
            ushort port, 
            string subdomain, 
            Protocol protocol, 
            CancellationToken token = default
        ) {
            EnsureInitialized();
            return await _hub!
                .InvokeAsync<ExposeAckResponse>(
                    "Expose", 
                    new ExposeRequest(_sessionId, port, subdomain, protocol), 
                    token
                );
        }

        private async Task ResumeTunnel(CancellationToken token = default)
        {
            EnsureInitialized();
            await _hub!.SendAsync("ResumeTunnel", _sessionId, token);
        }

        private async Task<Ping> Ping(CancellationToken token = default)
        {
            EnsureInitialized();
            return await _hub!
                .InvokeAsync<Ping>("Ping", _sessionId, token);
        }
    }
}
