using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsGrep.Common
{
    public static class WindowsUtils
    {
        #region Fields..
        private const string FILEHASH_PATTERN = "^[0-9a-fA-F]+$";

        private static Regex _fileHashRegex = new Regex(FILEHASH_PATTERN);
        private static string _diskRootName;
        private static uint _diskClusterSize;
        #endregion Fields..

        #region Methods..
        public static async Task<List<string>> GetFilesAsync(string path, bool recursive, CancellationToken cancellationToken, FileAttributes fileAttributesToSkip = default)
        {
            var enumerationOptions = new EnumerationOptions() { ReturnSpecialDirectories = true, AttributesToSkip = fileAttributesToSkip };

            if (recursive)
                enumerationOptions.RecurseSubdirectories = true;

            List<string> files = new List<string>();

            await Task.Run(() =>
            {
                foreach (var file in Directory.EnumerateFiles(Path.TrimEndingDirectorySeparator(path.TrimEnd()), "*", enumerationOptions))
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    files.Add(file);
                }
            }, cancellationToken);

            return files;
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

                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
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
}
