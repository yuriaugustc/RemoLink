using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using Server.Services.Tunnel;
using Shared.Contracts.Requests;
using Shared.Contracts.Responses;
using Shared.Models;

namespace Server.Controllers
{
    public class TunnelController(
        ILogger<TunnelController> logger,
        TunnelRegistry tunnel,
        IHubContext<TunnelHub> hubContext
    ) : ControllerBase
    {
        private readonly ILogger<TunnelController> _logger = logger;
        private readonly TunnelRegistry _tunnels = tunnel;
        private readonly IHubContext<TunnelHub> _hubContext = hubContext;

        [HttpGet("{**catchAll}")]
        [HttpPost("{**catchAll}")]
        [HttpPut("{**catchAll}")]
        [HttpDelete("{**catchAll}")]
        public async Task<IActionResult> Index()
        {
            string subdomain = Request.Host.Host.Split('.')[0];

            Tunnel? tunnel = _tunnels.GetTunnelBySubdomain(subdomain);

            if (tunnel is null)
            {
                return NotFound();
            }

            TunnelRequest request = CreateFromRequest();

            using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

            TunnelResponse response;
            try
            {
                response =
                    await _hubContext
                        .Clients
                        .Client(tunnel.ConnectionId)
                        .InvokeAsync<TunnelResponse>("TunnelRequest", request, cts.Token);
            }
            catch (OperationCanceledException)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning("Tunnel request to {Subdomain} timed out", subdomain);
                }

                return StatusCode(
                    StatusCodes.Status504GatewayTimeout
                );
            }

            FillHttpResponse(response);

            return Empty;
        }

        private TunnelRequest CreateFromRequest()
        {
            Request;
            //TODO: implement
        }

        private void FillHttpResponse(TunnelResponse response)
        {
            Response;
            //TODO: implement
        }
    }
}
