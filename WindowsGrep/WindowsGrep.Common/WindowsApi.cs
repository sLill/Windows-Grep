using System.Runtime.InteropServices;

namespace WindowsGrep.Common
{
    public static class WindowsApi
    {
        #region Methods..
        #region GetCompressedFileSizeW
        [DllImport("kernel32.dll")]
        public static extern uint GetCompressedFileSizeW([In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [Out, MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh);
        #endregion GetCompressedFileSizeW

        #region GetDiskFreeSpaceW
        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
        public static extern int GetDiskFreeSpaceW([In, MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName, out uint lpSectorsPerCluster, out uint lpBytesPerSector, out uint lpNumberOfFreeClusters, out uint lpTotalNumberOfClusters);
        #endregion GetDiskFreeSpaceW
        #endregion Methods..
    }
}
