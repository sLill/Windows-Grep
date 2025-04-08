using System.IO;

namespace WindowsGrep.Common;

public static class WindowsUtils
{
    #region Fields..
    private const string FILEHASH_PATTERN = "^[0-9a-fA-F]+$";

    private static Regex _fileHashRegex = new Regex(FILEHASH_PATTERN);
    private static string _diskRootName;
    private static uint _diskClusterSize;
    #endregion Fields..

    #region Methods..
    public static string GetCompressedPath(string fullPath)
    {
        var directories = fullPath.Split(Path.DirectorySeparatorChar);
        return directories.Length > 1 ? @"..\" + directories[directories.Length - 1] : fullPath;
    }

    public static IEnumerable<string> GetFiles(string root, bool recursive, int maxRecursionDepth, CancellationToken cancellationToken, 
        List<string> excludeDirectories = null, FileAttributes fileAttributesToSkip = default)
        => GetFiles(root, root, recursive, maxRecursionDepth, cancellationToken, excludeDirectories, fileAttributesToSkip);

    private static IEnumerable<string> GetFiles(string root, string currentDirectory, bool recursive, int maxRecursionDepth, CancellationToken cancellationToken, 
        List<string> excludeDirectories = null, FileAttributes fileAttributesToSkip = default)
    {
        var enumerationOptions = new EnumerationOptions() { AttributesToSkip = fileAttributesToSkip };

        // Current directory
        foreach (var file in Directory.EnumerateFiles(Path.TrimEndingDirectorySeparator(currentDirectory.TrimEnd()), "*", enumerationOptions))
            yield return file;

        string relativePath = Path.GetRelativePath(root, currentDirectory);
        int depth = relativePath == "." ? 0 : relativePath.Split(Path.DirectorySeparatorChar).Length;

        // Subdirectories
        if (recursive && depth < maxRecursionDepth)
        {
            foreach (var subDirectory in Directory.EnumerateDirectories(currentDirectory, "*", enumerationOptions))
            {
                var directoryInfo = new DirectoryInfo(subDirectory);

                if (excludeDirectories != null && excludeDirectories.Any(x => directoryInfo.Name.Equals(x, StringComparison.OrdinalIgnoreCase)))
                    continue;

                foreach (var file in GetFiles(root, subDirectory, recursive, maxRecursionDepth, cancellationToken, excludeDirectories, fileAttributesToSkip))
                    yield return file;
            }
        }
    }


    public static string GetFileHash(string filePath, HashType hashType)
    {
        HashAlgorithm hashAlgorithm = (hashType) switch
        {
            HashType.SHA256 => SHA256.Create(),
            HashType.MD5 => MD5.Create()
        };

        // Read file in 1mb segments
        using (var fileStream = new BufferedStream(File.OpenRead(filePath), 1024 * 1024))
        {
            var hashBytes = hashAlgorithm.ComputeHash(fileStream);
            hashAlgorithm.Dispose();

            return Convert.ToHexStringLower(hashBytes).Replace("-", "").ToLower();
        }
    }

    public static long GetFileSizeOnDisk(string filePath)
    {
        long fileSize = -1;
        try
        {
            FileInfo fileInfo = new FileInfo(filePath);
            string diskRootName = fileInfo.Directory.Root.FullName;

            if (diskRootName != _diskRootName)
            {
                uint sectorsPerCluster;
                uint bytesPerSector;

                WindowsApi.GetDiskFreeSpaceW(diskRootName, out sectorsPerCluster, out bytesPerSector, out _, out _);

                _diskRootName = diskRootName;
                _diskClusterSize = sectorsPerCluster * bytesPerSector;
            }

            uint fileSizeHigh;
            uint fileSizeLow = WindowsApi.GetCompressedFileSizeW(filePath, out fileSizeHigh);

            fileSize = (long)fileSizeHigh << 32 | fileSizeLow;
            fileSize = ((fileSize + _diskClusterSize - 1) / _diskClusterSize) * _diskClusterSize;
        }
        catch
        {
            throw new Exception("Error: Could not evaluate filesize");
        }

        return fileSize;
    }

    /// <summary>
    /// Reduce a filesize (in kb) to the maximum target number of leading signifigant digits
    /// </summary>
    public static double GetReducedSize(long size, int targetNumberOfDigits, out FileSizeType fileSizeType)
    {
        long reducedSize = size;

        var fileSizeTypes = Enum.GetValues<FileSizeType>().Where(x => size > x.GetCustomAttribute<ValueAttribute>().Value);
        fileSizeType = fileSizeTypes.Any() ? fileSizeTypes.Max() : FileSizeType.Kb;

        long fileSizeTypeModifier = fileSizeType.GetCustomAttribute<ValueAttribute>().Value;
        return Math.Round(size / (double)fileSizeTypeModifier, targetNumberOfDigits);
    }

    public static bool IsValidFileHash(string value, HashType hashType)
    {
        int validHashLength = (hashType) switch
        {
            HashType.SHA256 => 64,
            HashType.MD5 => 32
        };

        // Check if the input matches the hash pattern and has a valid length
        bool isValidPattern = _fileHashRegex.IsMatch(value);
        bool isValidLength = value.Length == validHashLength;

        return isValidPattern && isValidLength;
    }
    #endregion Methods..
}
