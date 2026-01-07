namespace Shared.Contracts.Pipes
{
    public static class PipeConventions
    {
        public const string PipeName = "remolinkd_pipe";
        public const char ArgumentSeparator = ';';

        public const int TimeoutMilliseconds = 1000;
        public const int MaxPayloadSize = 64 * 1024;
    }
}
