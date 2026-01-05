using Client.Domain.Commands;
using Client.Services.Registry;

namespace Client.Application
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

        public Task<CommandResult> ExecuteActionAsync(string paramName, string[]? args = null)
        {
            return _paramsRegistry.InvokeActionAsync(paramName, args);
        }

        private void InitializeParams()
        {
            _paramsRegistry.RegisterAction(
                "help",
                _commandHandler.CommandHelpHandler
            );

            _paramsRegistry.RegisterAction(
                "start",
                _commandHandler.CommandStartHandler
            );

            _paramsRegistry.RegisterAction(
                "expose", 
                _commandHandler.CommandExposeHandler
            );

            _paramsRegistry.RegisterAction(
                "stop",
                _commandHandler.CommandStopHandler
            );
        }
    }
}
