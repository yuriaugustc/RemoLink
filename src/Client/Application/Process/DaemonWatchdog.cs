using System.Diagnostics;

namespace Client.Application.Process
{
    using Process = System.Diagnostics.Process;
    public static class DaemonWatchdog
    {
        private const string _daemonExecutableName = "remolinkd";

        public static async Task EnsureDaemon()
        {
            if(IsDaemonRunning())
                return;

            await StartDaemon();
        }

        private static bool IsDaemonRunning()
        {
            Process[] processes = 
                Process.GetProcessesByName(_daemonExecutableName);

            return processes.Length > 0;
        }

        private static async Task StartDaemon()
        {
            _ = Process.Start(new ProcessStartInfo
                {
                    FileName = _daemonExecutableName,
                    Arguments = "",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }) 
                ?? throw new InvalidOperationException("Failed to start the daemon process.");
        }
    }
}
