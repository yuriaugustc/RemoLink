namespace Shared.Contracts.Responses
{
    public record class TunnelResponse(
        string Key,
        byte[] Payload,
        bool IsBinary
    );
}
