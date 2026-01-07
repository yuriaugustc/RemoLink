using Daemon.Application;
using Shared.Commands;
using Shared.Contracts.Pipes;
using System.Buffers.Binary;
using System.IO.Pipes;
using System.Text;

namespace Daemon.Infra.Pipes
{
    internal class ServerPipe
    {
        private static readonly Lazy<ServerPipe> _instance = new(() => new ServerPipe());
        public static ServerPipe GetInstance() => _instance.Value;
        private ServerPipe() { }

        private readonly ActionDispatcher _dispatcher = ActionDispatcher.GetInstance();

        public async Task ListenAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                using NamedPipeServerStream pipe = new(
                    PipeConventions.PipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous
                );

                await pipe.WaitForConnectionAsync(token);

                await HandleClientAsync(pipe);

                pipe.Close();
            }
        }

        private async Task HandleClientAsync(Stream stream)
        {
            // First message: message length
            byte[] lengthBuffer = new byte[4];
            await ReadExactAsync(stream, lengthBuffer);

            int length = BinaryPrimitives.ReadInt32LittleEndian(lengthBuffer);
            if (length is <= 0 or > 64 * 1024)
                throw new InvalidOperationException("Invalid message length.");

            // Second message: payload
            byte[] payloadBuffer = new byte[length];
            await ReadExactAsync(stream, payloadBuffer);

            string command = Encoding.UTF8.GetString(payloadBuffer);

            CommandResult result = await HandleCommand(command);

            // Send response
            byte[] payload = Encoding.UTF8.GetBytes(result.Serialize());
            byte[] responseLengthBuffer = new byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(responseLengthBuffer, payload.Length);
            // Send length
            await stream.WriteAsync(responseLengthBuffer);
            // Send payload
            await stream.WriteAsync(payload);

            await stream.FlushAsync();
        }

        private async Task<CommandResult> HandleCommand(string commandCSV, CancellationToken token = default)
        {
            string[] args = commandCSV.Split(PipeConventions.ArgumentSeparator);
            return await _dispatcher
                .ExecuteActionAsync(args[0], [.. args.Skip(1)], token);
        }

        private static async Task ReadExactAsync(
            Stream stream,
            byte[] buffer,
            CancellationToken token = default
        ) {
            int offset = 0;

            while (offset < buffer.Length)
            {
                int read = await stream
                    .ReadAsync(
                        buffer.AsMemory(offset, buffer.Length - offset),
                        token
                    );

                if (read == 0)
                    throw new EndOfStreamException();

                offset += read;
            }
        }
    }
}
