namespace WindowsGrep.Configuration
{
    public enum ConfigItem
    {
        [DefaultValue(true)]
        IncludeSystemProtectedFiles,

        [DefaultValue(true)]
        IncludeHiddenFiles,

        [DefaultValue(false)]
        DisplayWorkingDirectoryInPrompt
    }
}
