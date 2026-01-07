using static Client.Application.Process.DaemonWatchdog;
using static Client.Application.Commands.CommandDispatcher;

await EnsureDaemon();
await DispatchCommand(args);