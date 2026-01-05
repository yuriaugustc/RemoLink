namespace Client.Domain.Errors
{
    internal enum ExitCode
    {
        // Success
        Success = 0,

        // Generic failure
        Fail = 1,

        // CLI argument errors (10–19)
        InvalidArgs = 10,
        NotFound = 11,

        // State errors / pre-condition (20–39)
        Timeout = 20,

        ExposeBeforeStart = 21,
        StopBeforeStart = 22,
        StartBeforeLogin = 23,

        StartedAlready = 30,
        ExposedAlready = 31,
        StoppedAlready = 32,
    }

}
