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

            TunnelRequest request = await CreateFromRequest();

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

            await FillHttpResponse(response);

            return Empty;
        }

        private async Task<TunnelRequest> CreateFromRequest()
        {
            using StreamReader reader = new(Request.Body);
            return new TunnelRequest(
                Method: Request.Method,
                Path: Request.Path + Request.QueryString,
                Headers: Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                JsonBody: await reader.ReadToEndAsync()
            );
        }

        private async Task FillHttpResponse(TunnelResponse response)
        {
            Response.StatusCode = response.StatusCode;

            foreach (KeyValuePair<string, string> header in response.Headers)
            {
                Response.Headers[header.Key] = header.Value;
            }
            
            if (!string.IsNullOrEmpty(response.JsonBody))
            {
                Response.ContentType = "application/json";
                await Response.WriteAsync(response.JsonBody);
            }
        }
    }
}
