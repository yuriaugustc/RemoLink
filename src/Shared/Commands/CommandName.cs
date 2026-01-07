namespace Shared.Commands
{
    public static class CommandName
    {
        //TODO: maybe in future add i18n support

        // commands
        public const string Start = "start";
        public const string Stop = "stop";
        public const string Expose = "expose";
        public const string Help = "help";
        public const string Exit = "exit";
        public const string ShellMode = "shell";

        // optional args
        public const string PortFlag = "--port";
        public const string NameFlag = "--name";
        public const string ProtocolFlag = "--protocol";
    }
}
