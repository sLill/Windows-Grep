using System;
using System.IO;
using System.Linq;

namespace WindowsGrep.Common
{
    public static class WindowsUtils
    {
        #region Fields..
        private static string _diskRootName;
        private static uint _diskClusterSize;
        #endregion Fields..

        #region Methods..
        #region GetFileSizeOnDisk
        public static long GetFileSizeOnDisk(string file)
        {
            long fileSize = -1;
            try
            {
                FileInfo fileInfo = new FileInfo(file);
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
                uint fileSizeLow = WindowsApi.GetCompressedFileSizeW(file, out fileSizeHigh);

                fileSize = (long)fileSizeHigh << 32 | fileSizeLow;
                fileSize = ((fileSize + _diskClusterSize - 1) / _diskClusterSize) * _diskClusterSize;
            }
            catch
            {
                throw new Exception("Error: Could not evaluate filesize");
            }

            return fileSize;
        }
        #endregion GetFileSizeOnDisk

        #region GetReducedSize
        /// <summary>
        /// Reduce a filesize (in kb) to the maximum target number of leading signifigant digits
        /// </summary>
        public static double GetReducedSize(long size, int targetNumberOfDigits, out FileSizeType fileSizeType)
        {
            long reducedSize = size;

            var fileSizeTypes = Enum.GetValues<FileSizeType>().Where(x => size > x.GetCustomAttribute<ValueAttribute>().Value);
            fileSizeType = fileSizeTypes.Any() ? fileSizeTypes.Max() : FileSizeType.Kb;

            long fileSizeTypeModifier = fileSizeType.GetCustomAttribute<ValueAttribute>().Value;
            return Math.Round(size / (double)fileSizeTypeModifier, 2);
        }
        #endregion GetReducedSize
        #endregion Methods..
    }
}
