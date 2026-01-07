using Shared.Commands;
using Daemon.Infra.Registry;

namespace Daemon.Application
{
    internal class ActionDispatcher
    {
        private static Lazy<ActionDispatcher> Instance { get; } = new(() => new());
        public static ActionDispatcher GetInstance()
        {
            return Instance.Value;
        }
        private ActionDispatcher() 
        {
            InitializeParams();
        }

        private readonly ParamsRegistry _paramsRegistry = ParamsRegistry.GetInstance();
        private readonly CommandHandler _commandHandler = CommandHandler.GetInstance();

        public CommandResult ExecuteAction(string paramName, string[]? args = null)
        {
            return _paramsRegistry.InvokeAction(paramName, args);
        }

        public Task<CommandResult> ExecuteActionAsync(string paramName, string[]? args = null, CancellationToken token = default)
        {
            return _paramsRegistry.InvokeActionAsync(paramName, args, token);
        }

        private void InitializeParams()
        {
            _paramsRegistry.RegisterAction(
                CommandName.Help,
                _commandHandler.CommandHelpHandler
            );

            _paramsRegistry.RegisterAction(
                CommandName.Start,
                _commandHandler.CommandStartHandler
            );

            _paramsRegistry.RegisterAction(
                CommandName.Expose, 
                _commandHandler.CommandExposeHandler
            );

            _paramsRegistry.RegisterAction(
                CommandName.Stop,
                _commandHandler.CommandStopHandler
            );

            _paramsRegistry.RegisterAction(
                CommandName.Exit,
                _commandHandler.CommandExitHandler
            );
        }
    }
}
