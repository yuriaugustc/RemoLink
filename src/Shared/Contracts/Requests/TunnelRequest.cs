namespace Shared.Contracts.Requests
{
    public record class TunnelRequest(
        string Key,
        byte[] Payload,
        bool IsBinary
    );
}
