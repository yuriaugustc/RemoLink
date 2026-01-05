namespace Client.Domain.Commands
{
    internal static class CommandName
    {
        //TODO: maybe in future add i18n support

        // commands
        public const string Start = "start";
        public const string Stop = "stop";
        public const string Expose = "expose";
        public const string Help = "help";

        // optional args
        public const string Port = "--port";
        public const string Name = "--name";
    }
}
