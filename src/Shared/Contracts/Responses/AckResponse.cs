using Shared.Enums;

namespace Shared.Contracts.Responses
{
    public record class AckResponse(
        bool Success,
        string? Message = null,
        ErrorCode? Error = null,
        object? Data = default
    );
}
