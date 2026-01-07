using Shared.Enums;

namespace Shared.Extensions
{
    public static class ProtocolExtensions
    {
        extension(Protocol protocol)
        {
            public static Protocol GetProtocol(string protocolStr)
            {
                return protocolStr.ToLower() switch
                {
                    "tcp" => Protocol.Tcp,
                    "udp" => Protocol.Udp,
                    "http" => Protocol.Http,
                    "https" => Protocol.Https,
                    _ => Protocol.Http
                };
            }
        }
    }
}
