namespace Shared.Contracts.Requests
{
    public record class TunnelRequest(
        string Method,
        string Path,
        IDictionary<string, string> Headers,
        string JsonBody
    );
}
