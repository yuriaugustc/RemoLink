using Newtonsoft.Json;
using Shared.Enums;
using Shared.Extensions;
using Shared.Messages;

namespace Shared.Commands
{
    public sealed record CommandResult
    {
        public bool Success { get; }
        public string? Message { get; }
        public ExitCode ExitCode { get; }

        [JsonConstructor]
        public CommandResult(bool success, string? message, ExitCode exitCode)
        {
            Success = success;
            Message = message;
            ExitCode = exitCode;
        }

        public static CommandResult Ok(string? message = null)
        {
            return new CommandResult(true, message, ExitCode.Success);
        }

        public static CommandResult Failure(string? message = null)
        {
            return new CommandResult(false, message, ExitCode.Fail);
        }

        public static CommandResult Timeout(string? message = null)
        {
            return new CommandResult(false, message, ExitCode.Timeout);
        }

        public static CommandResult NotFound(string? message = null)
        {
            return new CommandResult(
                false,
                message ?? DefaultReturnMessages.CommandNotFound,
                ExitCode.NotFound
            );
        }

        public static CommandResult FromExitCode(ExitCode code, string? message = null)
        {
            return new CommandResult(code.IsSuccess, message, code);
        }

        public static CommandResult FromSerialized(string serialized)
        {
            return JsonConvert.DeserializeObject<CommandResult>(serialized)
                ?? FromExitCode(
                        ExitCode.DeserializationError,
                        DefaultReturnMessages.DeserializationError
                    );
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
