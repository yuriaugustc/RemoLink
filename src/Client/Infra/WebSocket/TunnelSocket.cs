using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Requests;
using Shared.Contracts.Responses;
using Shared.Enums;

namespace Client.Services.WebSocket
{
    internal class TunnelSocket
    {
        private HubConnection? _hub;
        private readonly string _url = ""; // Replace with actual URL
        public bool IsConnected => _hub?.State == HubConnectionState.Connected;
        public bool IsReady => _hub is not null;

        private static Lazy<TunnelSocket> Instance { get; } = new(() => new());
        private TunnelSocket() { }

        public static TunnelSocket GetInstance()
        {
            return Instance.Value;
        }

        public void Initialize()
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
        }

        public void EnsureInitialized()
        {
            if (!IsReady)
                throw new InvalidOperationException("Hub connection is not initialized.");
        }

        public async Task StartAsync()
        {
            EnsureInitialized();
            await _hub!.StartAsync();
        }

        public async Task StopAsync()
        {
            EnsureInitialized();
            await _hub!.StopAsync();
        }

        public async Task Register(string key)
        {
            EnsureInitialized();
            await _hub!.SendAsync("Register", key);
        }

        public async Task Unregister(string key)
        {
            EnsureInitialized();
            await _hub!.SendAsync("Unregister", key);
        }

        public async Task<StartAckResponse> Expose(ExposeRequest message)
        {
            EnsureInitialized();
            return await _hub!
                .InvokeAsync<StartAckResponse>("Expose", message);
        }

        public async Task ResumeTunnel(string key)
        {
            EnsureInitialized();
            await _hub!.SendAsync("ResumeTunnel", key);
        }

        public async Task<Ping> Ping(string key)
        {
            EnsureInitialized();
            return await _hub!
                .InvokeAsync<Ping>("Ping", key);
        }
    }
}
