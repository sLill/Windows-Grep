namespace WindowsGrep.Core;

public static class WindowsGrepUtils
{
    #region Methods..
    public static bool ArePathsEqual(string path1, string path2)
    {
        // Normalize paths to ensure consistent comparison
        var normalizedPath1 = Path.GetFullPath(path1).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var normalizedPath2 = Path.GetFullPath(path2).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return string.Equals(normalizedPath1, normalizedPath2, StringComparison.OrdinalIgnoreCase);
    }
    #endregion Methods..
}
