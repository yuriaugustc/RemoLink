namespace Client.Application.Output
{
    internal static class ConsoleFormatter
    {
        public static void WriteConsole(string message, bool isError)
        {
            if (isError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
