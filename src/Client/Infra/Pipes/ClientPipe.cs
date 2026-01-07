using Shared.Commands;
using Shared.Contracts.Pipes;
using System.Buffers.Binary;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace Client.Infra.Pipes
{
    internal static class ClientPipe
    {
        public static async Task<CommandResult> SendCommandAsync(string[] command, CancellationToken token)
        {
            using NamedPipeClientStream pipe = new(
                ".",
                PipeConventions.PipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous
            );

            await pipe.ConnectAsync(PipeConventions.TimeoutMilliseconds, token);

            string commandCSV = string.Join(
                PipeConventions.ArgumentSeparator,
                command
            );

            byte[] payload = Encoding.UTF8.GetBytes(commandCSV);
            byte[] lengthBuffer = new byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(lengthBuffer, payload.Length);
            // Send length
            await pipe.WriteAsync(lengthBuffer, token);
            // Send payload
            await pipe.WriteAsync(payload,  token);

            await pipe.FlushAsync(token);

            // Read response length
            byte[] responseLengthBuffer = new byte[4];
            await ReadExactAsync(pipe, responseLengthBuffer, token);

            int length = BinaryPrimitives.ReadInt32LittleEndian(responseLengthBuffer);
            if (length is <= 0 or > 64 * 1024)
                throw new InvalidOperationException("Invalid message length.");

            byte[] payloadBuffer = new byte[length];
            await ReadExactAsync(pipe, payloadBuffer, token);

            pipe.Close();

            string resultJson = Encoding.UTF8.GetString(payloadBuffer);

            return CommandResult.FromSerialized(resultJson);
        }

        private static async Task ReadExactAsync(
            Stream stream,
            byte[] buffer,
            CancellationToken token
        )
        {
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
