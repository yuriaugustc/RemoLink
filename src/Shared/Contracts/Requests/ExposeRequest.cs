using Shared.Enums;

namespace Shared.Contracts.Requests
{
    public record class ExposeRequest(
        string Key,
        ushort Port,
        string? Subdomain = null,
        Protocol? Protocol = null
    );
}
