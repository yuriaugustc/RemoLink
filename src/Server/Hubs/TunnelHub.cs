using Microsoft.AspNetCore.SignalR;
using Server.Services.Subdomain;
using Server.Services.Tunnel;
using Shared.Contracts.Requests;
using Shared.Contracts.Responses;
using Shared.Enums;
using Shared.Models;

namespace Server.Hubs
{
    public class TunnelHub(
        IConfiguration config,
        ILogger<TunnelHub> logger,
        TunnelRegistry registry
    ) : Hub
    {
        private readonly IConfiguration _config = config;
        private readonly ILogger<TunnelHub> _logger = logger;
        private readonly TunnelRegistry _tunnels = registry;

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string connectionId = Context.ConnectionId;

            _tunnels.UnregisterTunnelByConnId(connectionId);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Connection disconnected: {ConnectionId}", connectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public override Task OnConnectedAsync()
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Connection established: {ConnectionId}", Context.ConnectionId);
            }
            
            return base.OnConnectedAsync();
        }

        //TODO: Implement Register method when we have auth policy (will be called on --start)
        public void Register(string key)
        {

        }

        public StopAckResponse Unregister(string key)
        {
            _tunnels.UnregisterTunnel(key);

            return new StopAckResponse(
                success: true,
                message: "Tunnel  successfully"
            );
        }

        public StartAckResponse Expose(ExposeRequest message)
        {
            #region Validations
            string badRequest = string.Empty;
            ErrorCode errorCode = ErrorCode.UnknownError;

            if (message.Port is 0)
            {
                badRequest = "Invalid port number";
                errorCode = ErrorCode.InvalidPort;
            }
            else if (message.Protocol is null or Protocol.Unknown)
            {
                badRequest = "Unsupported protocol";
                errorCode = ErrorCode.BadRequest;
            }
            else if (string.IsNullOrEmpty(message.Key))
            {
                badRequest = "Missing identification key";
                errorCode = ErrorCode.Unauthorized;
            } 
            else if (
                !string.IsNullOrEmpty(message.Subdomain) && 
                _tunnels.IsSubdomainTaken(message.Subdomain))
            {
                badRequest = "Subdomain in use";
                errorCode = ErrorCode.SubdomainTaken;
            }

            if (badRequest != string.Empty)
            {
                return StartAckResponse.Failure(
                    message: badRequest,
                    error: errorCode
                );
            }
            #endregion

            string subdomain = 
                message.Subdomain?.Trim().ToLowerInvariant() 
                ?? string.Empty;

            if (string.IsNullOrEmpty(subdomain))
            {
                // even is difficult to have collisions, check it anyway
                do {
                    subdomain = SubdomainGenerator.Generate();
                } while (_tunnels.IsSubdomainTaken(subdomain));
            }

            string baseDomain = _config["Server:Domain"] ?? "localhost";
            string protocolStr = message.Protocol.ToString()!.ToLower();
            string targetUrl = message.Protocol == Protocol.Tcp
                ? $"{protocolStr}://{subdomain}.{baseDomain}:{message.Port}"
                : $"{protocolStr}://{subdomain}.{baseDomain}";

            _tunnels.RegisterTunnel(new Tunnel(
                key: message.Key,
                connectionId: Context.ConnectionId,
                subdomain: subdomain,
                port: message.Port,
                protocol: message.Protocol!.Value,
                targetUrl: targetUrl
            ));

            return StartAckResponse.Started(targetUrl);
        }

        public Task ResumeTunnel(string key)
        {
            Tunnel? tunnel = _tunnels.GetTunnelByKey(key);
            if (tunnel is null)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning("Tunnel with key {Key} not found", key);
                }
                
                return Task.CompletedTask;
            }

            tunnel.ConnectionId = Context.ConnectionId;
            tunnel.LastActive = DateTime.UtcNow;

            return Task.CompletedTask;
        }
    }
}
