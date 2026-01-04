namespace Shared.Contracts.Requests
{
    public record class StartRequest(
        string Subdomain,
        int Port,
        string Protocol
    );
}
