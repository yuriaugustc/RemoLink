using Client.Infra.Pipes;
using Shared.Commands;
using static Client.Application.Output.ConsoleFormatter;
using static Client.Application.Output.ShellMode;

namespace Client.Application.Commands
{
    internal static class CommandDispatcher
    {
        public static async Task DispatchCommand(string[] args)
        {
            if (args.Length > 0 && !args.Contains(CommandName.ShellMode))
            {
                await ProcessCommand(args);
                return;
            }

            await StartShellMode();
        }

        public static async Task ProcessCommand(string[] args)
        {
            try
            {
                CancellationTokenSource source =
                    new(TimeSpan.FromSeconds(30));

                CommandResult result =
                    await ClientPipe.SendCommandAsync(args, source.Token);

                if (!string.IsNullOrEmpty(result.Message))
                {
                    WriteConsole(result.Message, !result.Success);
                }
            } 
            catch (OperationCanceledException)
            {
                WriteConsole("The command has timed out.", false);
                return;
            }
            catch (Exception ex)
            {
                WriteConsole($"An error occurred while processing the command: {ex.Message}", false);
                return;
            }
        }
    }
}
