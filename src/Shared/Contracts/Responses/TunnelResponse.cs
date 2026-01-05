namespace Shared.Contracts.Responses
{
    public record class TunnelResponse(
        IDictionary<string, string> Headers,
        string JsonBody,
        byte StatusCode
    );
}
