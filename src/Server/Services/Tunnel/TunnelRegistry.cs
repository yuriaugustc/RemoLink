using System.Collections.Concurrent;

namespace Server.Services.Tunnel
{
    using Tunnel = Shared.Models.Tunnel;
    public class TunnelRegistry
    {
        private readonly ConcurrentDictionary<string, Tunnel> _tunnels = [];
        private readonly ConcurrentDictionary<string, string> _subdomainKey = [];
        private readonly ConcurrentDictionary<string, string> _connIdKey = [];

        public void RegisterTunnel(Tunnel tunnel)
        {
            _tunnels.TryAdd(tunnel.Key, tunnel);
            _subdomainKey.TryAdd(tunnel.Subdomain, tunnel.Key);
            _subdomainKey.TryAdd(tunnel.ConnectionId, tunnel.Key);
        }

        public void UnregisterTunnel(string key)
        {
            _subdomainKey.TryRemove(_tunnels[key].Subdomain, out _);
            _connIdKey.TryRemove(_tunnels[key].ConnectionId, out _);
            _tunnels.TryRemove(key, out _);
        }

        public void UnregisterTunnelByConnId(string connecionId)
        {
            _connIdKey.TryRemove(connecionId, out string? key);
            
            if (string.IsNullOrEmpty(key)) return;

            _tunnels.TryRemove(key, out Tunnel? tunnel);

            if (tunnel is null) return;
            
            _subdomainKey.TryRemove(tunnel.Subdomain, out _);
        }

        public bool IsSubdomainTaken(string subdomain)
        {
            return _subdomainKey.ContainsKey(subdomain);
        }

        public Tunnel? GetTunnelByKey(string key)
        {
            _tunnels.TryGetValue(key, out var tunnel);
            return tunnel;
        }

        public Tunnel? GetTunnelBySubdomain(string subdomain)
        {
            string? key = GetKeyBySubdomain(subdomain);

            if(string.IsNullOrEmpty(key)) return null;

            return GetTunnelByKey(key);
        }

        public string? GetKeyBySubdomain(string key)
        {
            _subdomainKey.TryGetValue(key, out string? subdomain);
            return subdomain;
        }

        public string? GetKeyByConnectionId(string connectionId)
        {
            _connIdKey.TryGetValue(connectionId, out string? key);
            return key;
        }

        public void CleanUpExpiredTunnels()
        {
            DateTime expirationTime = DateTime.UtcNow.AddMinutes(-1);
            foreach (Tunnel tunnel in _tunnels.Values)
            {
                if (tunnel.LastActive < expirationTime)
                {
                    UnregisterTunnel(tunnel.Key);
                }
            }
        }
    }
}
