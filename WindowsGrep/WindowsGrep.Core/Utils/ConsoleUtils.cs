namespace WindowsGrep.Core;
public static class ConsoleUtils
{
    #region Methods..
    public static void PublishSplash()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resource = "WindowsGrep.Core.Properties.Resources.Splash.txt";

        using (Stream stream = assembly.GetManifestResourceStream(resource))
        using (StreamReader streamReader = new StreamReader(stream))
        {
            string content = streamReader.ReadToEnd();
            string formattedContent = FormatEscapeSequences(content);
            Console.WriteLine(formattedContent);
        }
    }

    public static void PublishHelp(bool extended)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resource = extended ? "WindowsGrep.Core.Properties.Resources.Help_Extended.txt" : "WindowsGrep.Core.Properties.Resources.Help.txt";

        using (Stream stream = assembly.GetManifestResourceStream(resource))
        using (StreamReader streamReader = new StreamReader(stream))
        {
            string content = streamReader.ReadToEnd();
            string formattedContent = FormatEscapeSequences(content);
            Console.WriteLine(formattedContent);
        }
    }

    public static void PublishPrompt()
    {
        Console.Write("$ ");
    }

    public static string FormatEscapeSequences(string input)
    {
        return input.Replace("\\u001b", "\u001b")
                    .Replace("\\e", "\u001b")
                    .Replace("\\033", "\u001b");
    }

    public static void ClearConsole()
        => Console.Clear();
    #endregion Methods..
}
