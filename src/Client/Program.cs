using Client.Application;

try
{
    if (args.Length == 0)
    {
        WriteConsole(
            "No commands were provided. Try use 'help' command.", 
            true
        );
        return;
    }

    var result = ActionDispatcher
        .GetInstance()
        .ExecuteAction(args[0], [..args.Skip(1)]);

    if(!string.IsNullOrEmpty(result.Message))
    {
        Console.WriteLine(result.Message, result.Success);
    }
} catch (Exception ex)
{
    WriteConsole(
        $"An error occurred while executing the action: {args[0]}\n{ex.Message}", 
        true
    );
}

static void WriteConsole(string message, bool isError)
{
    if(isError)
    {
        Console.ForegroundColor = ConsoleColor.Red;
    }

    Console.WriteLine(message);
    Console.ResetColor();
}
