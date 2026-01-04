using Shared.Enums;

namespace Shared.Contracts.Responses
{
    public record class StopAckResponse : AckResponse
    {
        public StopAckResponse(
            bool success,
            string? message = null,
            ErrorCode? error = null
        ) : base(success, message, error)
        { }
    }
}
