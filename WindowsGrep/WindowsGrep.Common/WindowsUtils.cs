using System;
using System.IO;
using System.Linq;

namespace WindowsGrep.Common
{
    public static class WindowsUtils
    {
        #region Member Variables..
        private static string _DiskRootName;
        private static uint _DiskClusterSize;
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        #region GetFileSizeOnDisk
        public static long GetFileSizeOnDisk(string file)
        {
            FileInfo FileInfo = new FileInfo(file);
            string DiskRootName = FileInfo.Directory.Root.FullName;

            if (DiskRootName != _DiskRootName)
            {
                uint SectorsPerCluster;
                uint BytesPerSector;

                WindowsApi.GetDiskFreeSpaceW(DiskRootName, out SectorsPerCluster, out BytesPerSector, out _, out _);

                _DiskRootName = DiskRootName;
                _DiskClusterSize = SectorsPerCluster * BytesPerSector;
            }

            uint FileSizeHigh;
            uint FileSizeLow = WindowsApi.GetCompressedFileSizeW(file, out FileSizeHigh);

            long FileSize = (long)FileSizeHigh << 32 | FileSizeLow;
            return ((FileSize + _DiskClusterSize - 1) / _DiskClusterSize) * _DiskClusterSize;
        }
        #endregion GetFileSizeOnDisk

        #region GetReducedSize
        /// <summary>
        /// Reduce a filesize (in kb) to the maximum target number of leading signifigant digits
        /// </summary>
        public static double GetReducedSize(long size, int targetNumberOfDigits, out FileSizeType fileSizeType)
        {
            long ReducedSize = size;

            fileSizeType = Enum.GetValues<FileSizeType>().Where(x => size > x.GetCustomAttribute<ValueAttribute>().Value).Max();
            long FileSizeTypeModifier = fileSizeType.GetCustomAttribute<ValueAttribute>().Value;

            return Math.Round(size / (double)FileSizeTypeModifier, 2);
        }
        #endregion GetReducedSize
        #endregion Methods..
    }
}
