using Client.Domain.Commands;
using Client.Domain.Errors;
using Client.Services.WebSocket;
using static Client.Domain.Commands.CommandName;

namespace Client.Application
{
    internal class CommandHandler
    {
        private static Lazy<CommandHandler> Instance { get; } = new(() => new());
        public static CommandHandler GetInstance()
        {
            return Instance.Value;
        }
        private CommandHandler() { }

        private readonly TunnelSocket _tunnelSocket = TunnelSocket.GetInstance();

        public async Task<CommandResult> CommandStartHandler(string[] args)
        {
            _tunnelSocket.Initialize();
            await _tunnelSocket.StartAsync();

            Console.WriteLine("Start command executed.");
            return CommandResult.Ok("Start command completed successfully.");
        }

        public CommandResult CommandStopHandler(string[] args)
        {
            Console.WriteLine("Stop command executed.");
            return CommandResult.Ok("Stop command completed successfully.");
        }

        public CommandResult CommandExposeHandler(string[] args)
        {
            if(!args.Contains(Port))
            {
                return CommandResult.Failure("Expose command requires a port argument.");
            }

            // Get the index of the port argument
            int idx = args.IndexOf(Port);

            // Validate that there is a value after the port argument
            if (!ushort.TryParse(args[idx + 1], out ushort port) || port is 0)
            {
                return CommandResult.Failure("Invalid port number.");
            }

            if (!_tunnelSocket.IsReady)
            {
                return CommandResult.FromExitCode(
                    ExitCode.ExposeBeforeStart, 
                    "Command 'start' not called yet. Call 'start' before this action."
                );
            }

            Console.WriteLine("Expose command executed.");

            return CommandResult.Ok("Service exposed at:\nhttps://teste.remolink.io.\n");
        }

        public CommandResult CommandHelpHandler(string[] args)
        {
            //TODO: in future, look in args if user asked for help about a specific command
            if(args.Length > 0)
            {
                string specificCommand = args[0];
                return CommandResult.Ok($"Help for command '{specificCommand}' is not implemented yet.");
            }

            //TODO: improve help message
            string helpMessage = @"
            We have only three commands, dumbass, how you don't know that?
            Here we are:
            
            start
            expose
            stop

            Is that hard? Geez..
            ";

            return CommandResult.Ok(helpMessage);
        }
    }
}
