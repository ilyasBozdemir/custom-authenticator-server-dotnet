namespace CustomAuthenticatorServerDotNet;

public static class ConsoleHelper
{
    public static void WriteColoredLine(object text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void WriteColored(object text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }
   
}
