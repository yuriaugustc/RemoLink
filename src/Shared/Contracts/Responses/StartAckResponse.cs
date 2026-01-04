using Shared.Enums;

namespace Shared.Contracts.Responses
{
    public record class StartAckResponse : AckResponse
    {
        public string PublicUrl { get; init; }

        public StartAckResponse(
            bool success,
            string publicUrl,
            string? message = null,
            ErrorCode? error = null,
            object? data = default
        ) : base(success, message, error, data)
        {
            PublicUrl = publicUrl;
        }

        public static StartAckResponse Started(
           string publicUrl,
           string? message = null
        ) => new(true, publicUrl, message);

        public static StartAckResponse Failure(
            string? message = null,
            ErrorCode? error = null,
            object? data = default
        ) => new(false, string.Empty, message, error, data);
    }
}
