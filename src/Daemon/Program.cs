using Daemon.Application;
using Daemon.Infra.Pipes;

DaemonHost host = DaemonHost.GetInstance();
ServerPipe serverPipe = ServerPipe.GetInstance();

AppDomain.CurrentDomain.ProcessExit += (s, e) =>
{
    host.Stop();
};

await serverPipe.ListenAsync(host.Token);
