namespace Client.Domain.Errors
{
    internal static class ExitCodeExtensions
    {
        extension(ExitCode code)
        {
            public bool IsSuccess => code == ExitCode.Success;
            public bool IsFailure => code == ExitCode.Fail;
            public bool IsTimeout => code == ExitCode.Timeout;
            public bool IsUsageError => (int)code is >= 10 and <= 19;
            public bool IsStateError => (int)code is >= 20 and <= 39;
        }
    }
}
