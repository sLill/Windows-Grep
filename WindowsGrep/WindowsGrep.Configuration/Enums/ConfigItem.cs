namespace WindowsGrep.Configuration;

public enum ConfigItem
{
    [DefaultValue(false)]
    DisplayWorkingDirectoryInPrompt,

    [DefaultValue(false)]
    ShowInaccessibleFiles
}
