using Shared.Commands;
using static Shared.Commands.CommandName;
using Daemon.Infra.WebSocket;
using System.Threading.Tasks;
using Shared.Contracts.Requests;
using Shared.Enums;
using Shared.Extensions;
using System.Text;
using Shared.Contracts.Responses;

namespace Daemon.Application
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

        public async Task<CommandResult> CommandStartHandler(string[] args, CancellationToken token = default)
        {
            if(!_tunnelSocket.IsReady)
                _tunnelSocket.Initialize(token);
            
            if(!_tunnelSocket.IsConnected)
                await _tunnelSocket.StartAsync(token);

            return CommandResult.Ok("Start command completed successfully.");
        }

        public async Task<CommandResult> CommandStopHandler(string[] args, CancellationToken token = default)
        {
            if (_tunnelSocket.IsConnected)
                await _tunnelSocket.StopAsync(token);
            
            return CommandResult.Ok("Stop command completed successfully.");
        }

        public async Task<CommandResult> CommandExposeHandler(string[] args, CancellationToken token = default)
        {
            if(!args.Contains(PortFlag))
            {
                return CommandResult.Failure("Expose command requires a port argument.");
            }

            // Get the index of the port argument
            int idx = args.IndexOf(PortFlag);

            // Validate that there is a value after the port argument
            if (idx + 1 >= args.Length ||
                !ushort.TryParse(args[idx + 1], out ushort port) 
                || port is 0
            ) {
                return CommandResult.Failure("Invalid port number.");
            }

            if (!_tunnelSocket.IsReady || !_tunnelSocket.IsConnected)
            {
                return CommandResult.FromExitCode(
                    ExitCode.ExposeBeforeStart, 
                    "Command 'start' not called yet. Call 'start' before this action."
                );
            }

            string name = args.Contains(NameFlag) && args.IndexOf(NameFlag) + 1 < args.Length
                ? args[args.IndexOf(NameFlag) + 1].Trim().ToLower()
                : string.Empty;

            Protocol protocol = args.Contains(ProtocolFlag) && args.IndexOf(ProtocolFlag) + 1 < args.Length
                ? ProtocolExtensions.GetProtocol(args[args.IndexOf(ProtocolFlag) + 1].Trim().ToLower()) 
                : Protocol.Http;

            ExposeAckResponse response = 
                await _tunnelSocket.ExposeAsync(port, name, protocol, token);

            if(!response.Success)
            {
                return CommandResult.FromExitCode(
                    ExitCode.Fail, 
                    $"Failed to expose service. {response.Message ?? "No additional information."}"
                );
            }
            StringBuilder sb = new();
            if(response.Message is not null)
                sb.AppendLine(response.Message);

            sb.AppendLine($"Service exposed at:\n{response.PublicUrl}\n");

            return CommandResult.Ok(sb.ToString());
        }

        public CommandResult CommandHelpHandler(string[] args, CancellationToken token = default)
        {
            //TODO: in future, look in args if user asked for help about a specific command
            if(args.Length > 0)
            {
                string specificCommand = args[0];
                return CommandResult.Ok($"Help for command '{specificCommand}' is not implemented yet.");
            }

            StringBuilder sb = new();
            sb.AppendLine("Available commands:\n");
            sb.AppendLine("\tstart: Initializes and starts the daemon.");
            sb.AppendLine("\texpose --port <port> [--name <name>] [--protocol <protocol>]: Exposes a local service through the tunnel.");
            sb.AppendLine("\tstop: Stops the daemon and closes all tunnels.");
            sb.AppendLine("\texit: Stops the daemon and exits the application.");
            sb.AppendLine("\thelp [<command>]: Displays help information for commands.");

            return CommandResult.Ok(sb.ToString());
        }

        public async Task<CommandResult> CommandExitHandler(string[] args, CancellationToken token = default)
        {
            await CommandStopHandler(args, token);

            DaemonHost.GetInstance().Stop();

            return CommandResult.Ok();
        }
    }
}
