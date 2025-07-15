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

    public static IEnumerable<FileItem> GetFiles(string rootPath, bool recursive, int maxRecursionDepth, long fileSizeMin, long fileSizeMax, 
        CancellationToken cancellationToken, List<string> excludeDirectories = null, FileAttributes fileAttributesToSkip = default)
        => GetFiles(rootPath, rootPath, recursive, maxRecursionDepth, fileSizeMin, fileSizeMax, cancellationToken, excludeDirectories, fileAttributesToSkip);

    private static IEnumerable<FileItem> GetFiles(string rootPath, string subPath, bool recursive, int maxRecursionDepth, long fileSizeMin, long fileSizeMax, 
        CancellationToken cancellationToken, List<string> excludeDirectories = null, FileAttributes fileAttributesToSkip = default)
    {
        var enumerationOptions = new EnumerationOptions() { AttributesToSkip = fileAttributesToSkip };

        // File
        if (Path.HasExtension(subPath) && File.Exists(subPath))
        {
            var fileSize = WindowsUtils.GetFileSizeOnDisk(subPath);
            yield return new FileItem(subPath, false, fileSize);
        }

        // Directory
        else
        {
            // Subdirectories
            foreach (var subDirectory in Directory.EnumerateDirectories(subPath, "*", enumerationOptions))
            {
                var directoryInfo = new DirectoryInfo(subDirectory);
                yield return new FileItem(directoryInfo.FullName, true, -1);
            }

            // Files in current directory
            foreach (var file in Directory.EnumerateFiles(Path.TrimEndingDirectorySeparator(subPath.TrimEnd()), "*", enumerationOptions))
            {
                var fileSize = WindowsUtils.GetFileSizeOnDisk(file);
                bool fileSizeValidateSuccess = ValidateFileSize(fileSize, fileSizeMin, fileSizeMax);
                if (fileSizeValidateSuccess)
                    yield return new FileItem(file, false, fileSize);
            }

            string relativePath = Path.GetRelativePath(rootPath, subPath);
            int depth = relativePath == "." ? 0 : relativePath.Split(Path.DirectorySeparatorChar).Length;

            // Recurse
            if (recursive && depth < maxRecursionDepth)
            {
                foreach (var subDirectory in Directory.EnumerateDirectories(subPath, "*", enumerationOptions))
                {
                    var directoryInfo = new DirectoryInfo(subDirectory);
                    if (excludeDirectories != null && excludeDirectories.Any(x => directoryInfo.Name.Contains(x)))
                        continue;

                    foreach (var result in GetFiles(rootPath, subDirectory, recursive, maxRecursionDepth, fileSizeMin, fileSizeMax, cancellationToken, excludeDirectories, fileAttributesToSkip))
                        yield return result;
                }
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

    private static bool ValidateFileSize(long fileSize, long fileSizeMinimum, long fileSizeMaximum)
    {
        bool isValid = true;

        isValid &= (fileSizeMinimum == -1 || (fileSize >= fileSizeMinimum));
        isValid &= (fileSizeMaximum == -1 || (fileSize <= fileSizeMaximum));

        return isValid;
    }

    public static void TryEnableAnsi()
    {
        try
        {
            // STD_OUTPUT_HANDLE
            var handle = WindowsApi.GetStdHandle(-11);

            // ENABLE_VIRTUAL_TERMINAL_PROCESSING
            WindowsApi.GetConsoleMode(handle, out uint mode);
            WindowsApi.SetConsoleMode(handle, mode | 0x4);
        }
        catch { }
    }
    #endregion Methods..
}
