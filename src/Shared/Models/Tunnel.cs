using Shared.Enums;

namespace Shared.Models
{
    public class Tunnel(
        string key, 
        string connectionId, 
        string subdomain, 
        ushort port,
        Protocol protocol,
        string targetUrl
    ) {
        public string Key { get; set; } = key;
        public string ConnectionId { get; set; } = connectionId;
        public string Subdomain { get; set; } = subdomain;
        public ushort Port { get; set; } = port;
        public Protocol Protocol { get; set; } = protocol;
        public string TargetUrl { get; set; } = targetUrl;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
    }
}
