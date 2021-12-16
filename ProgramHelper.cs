static class ProgramHelper
{
    /// <summary>
    /// Checks for a condition; if the condition is false, displays a message box that shows the call stack.
    /// </summary>
    /// <param name="condition">The conditional expression to evaluate. If the condition is true, a failure message is not sent and the message box is not displayed.</param>
    public static void Assert(bool condition) => System.Diagnostics.Debug.Assert(condition);

    /// <summary>
    /// Checks for a condition; if the condition is false, outputs a specified message and displays a message box that shows the call stack.
    /// </summary>
    /// <param name="condition">The conditional expression to evaluate. If the condition is true, the specified message is not sent and the message box is not displayed.</param>
    /// <param name="message">The message to display.</param>
    public static void Assert(bool condition, string? message) => System.Diagnostics.Debug.Assert(condition, message);

    public static void ReadKey(bool intercept = true) => Console.ReadKey(intercept);

    public static void SetCursorPosition(int left, int top) => Console.SetCursorPosition(left, top);

    public static void Clear() => Console.Clear();

    public static void WriteLine(object value, ConsoleColor? color = null) => WriteLine(value?.ToString(), color);

    public static void WriteLine(string? message, ConsoleColor? color = null) => Console.Write(message + Environment.NewLine, color);

    public static void WriteLine() => Console.Write(Environment.NewLine);

    public static void Write(object value, ConsoleColor? color = null) => Write(value?.ToString(), color);

    public static void Write(string? message, ConsoleColor? color = null)
    {
        ConsoleColor? previousColor = null;
        if (color != null)
        {
            previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color.Value;
        }

        Console.Write(message);

        if (previousColor != null)
        {
            Console.ForegroundColor = previousColor.Value;
        }
    }
}
