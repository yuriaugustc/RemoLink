using Client.Domain.Errors;
using Client.Domain.Messages;

namespace Client.Domain.Commands
{
    internal sealed record CommandResult 
    {
        public bool Success { get; }
        public string? Message { get; }
        public ExitCode ExitCode { get; }

        private CommandResult(bool success, string? message, ExitCode exitCode)
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
            bool success = code == ExitCode.Success;
            return new CommandResult(success, message, code);
        }
    }
}
