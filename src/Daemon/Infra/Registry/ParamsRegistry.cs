using Shared.Commands;

namespace Daemon.Infra.Registry
{
    internal class ParamsRegistry
    {
        private static Lazy<ParamsRegistry> Instance { get; } = new(() => new());
        private ParamsRegistry() { }
        public static ParamsRegistry GetInstance()
        {
            return Instance.Value;
        }

        private readonly Dictionary<string, Func<string[], CancellationToken, Task<CommandResult>>> _paramsActions = [];

        public void RegisterAction(string paramName, Action action)
        {
            _paramsActions[paramName] = (_, _) =>
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

        public void RegisterAction(string paramName, Action<string[], CancellationToken> action)
        {
            _paramsActions[paramName] = (args, token) =>
            {
                try
                {
                    action(args, token);
                    return Task.FromResult(CommandResult.Ok());
                }
                catch (Exception ex)
                {
                    return Task.FromResult(CommandResult.Failure(ex.Message));
                }
            };
        }

        public void RegisterAction(string paramName, Func<string[], CancellationToken, CommandResult> action)
        {
            _paramsActions[paramName] = async(args, token) =>
            {
                return action(args, token);
            };
        }

        public void RegisterAction(string paramName, Func<string[], CancellationToken, Task> action)
        {
            _paramsActions[paramName] = async(args, token) =>
            {
                try
                {
                    await action(args, token);
                    return CommandResult.Ok();
                }
                catch (Exception ex)
                {
                    return CommandResult.Failure(ex.Message);
                }
            };
        }

        public void RegisterAction(string paramName, Func<string[], CancellationToken, Task<CommandResult>> action)
        {
            _paramsActions[paramName] = action;
        }

        public async Task<CommandResult> InvokeActionAsync(string paramName, string[]? args = null, CancellationToken token = default)
        {
            if (_paramsActions.TryGetValue(paramName, out var action))
            {
                return await action(args ?? [], token);
            }
            return CommandResult.NotFound();
        }

        public CommandResult InvokeAction(string paramName, string[]? args = null, CancellationToken token = default)
        {
            return InvokeActionAsync(paramName, args, token)
                .GetAwaiter()
                .GetResult();
        }
    }
}
