using Client.Domain.Commands;

namespace Client.Services.Registry
{
    internal class ParamsRegistry
    {
        private static Lazy<ParamsRegistry> Instance { get; } = new(() => new());
        private ParamsRegistry() { }
        public static ParamsRegistry GetInstance()
        {
            return Instance.Value;
        }

        private readonly Dictionary<string, Func<string[], Task<CommandResult>>> _paramsActions = [];

        public void RegisterAction(string paramName, Action action)
        {
            _paramsActions[paramName] = (_) =>
            {
                try
                {
                    action();
                    return Task.FromResult(CommandResult.Ok());
                }
                catch (Exception ex)
                {
                    return Task.FromResult(CommandResult.Failure(ex.Message));
                }
            };
        }

        public void RegisterAction(string paramName, Action<string[]> action)
        {
            _paramsActions[paramName] = (args) =>
            {
                try
                {
                    action(args);
                    return Task.FromResult(CommandResult.Ok());
                }
                catch (Exception ex)
                {
                    return Task.FromResult(CommandResult.Failure(ex.Message));
                }
            };
        }

        public void RegisterAction(string paramName, Func<string[], CommandResult> action)
        {
            _paramsActions[paramName] = async(args) =>
            {
                return action(args);
            };
        }

        public void RegisterAction(string paramName, Func<string[], Task> action)
        {
            _paramsActions[paramName] = async(args) =>
            {
                try
                {
                    await action(args);
                    return CommandResult.Ok();
                }
                catch (Exception ex)
                {
                    return CommandResult.Failure(ex.Message);
                }
            };
        }

        public void RegisterAction(string paramName, Func<string[], Task<CommandResult>> action)
        {
            _paramsActions[paramName] = action;
        }

        public async Task<CommandResult> InvokeActionAsync(string paramName, string[]? args = null)
        {
            if (_paramsActions.TryGetValue(paramName, out var action))
            {
                return await action(args ?? []);
            }
            return CommandResult.NotFound();
        }

        public CommandResult InvokeAction(string paramName, string[]? args = null)
        {
            return InvokeActionAsync(paramName, args)
                .GetAwaiter()
                .GetResult();
        }
    }
}
