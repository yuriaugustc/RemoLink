using Shared.Commands;
using static Client.Application.Commands.CommandDispatcher;

namespace Client.Application.Output
{
    public static class ShellMode
    {
        public async static Task<bool> ReadCommand()
        {
            bool stillShellMode = true;

            Console.Write("remolink> ");
            
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                // if is Ctrl+C,
                // immediate return will reprint the prompt before callback
                await Task.Delay(50);

                return stillShellMode;
            }

            string[] inputArgs = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (inputArgs.Contains(CommandName.Exit))
                stillShellMode = false;

            await ProcessCommand(inputArgs);

            return stillShellMode;
        }

        public async static Task StartShellMode()
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("Exiting...");
                Environment.Exit(0);
            };

            bool shellMode = true;
            while (shellMode)
            {
                shellMode = await ReadCommand();
            }
        }
    }
}
