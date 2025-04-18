namespace WindowsGrep.Common;
public struct FileItem
{
    #region Properties..
    public string Name { get; private set; }
    public bool IsDirectory { get; private set; }
    public long FileSize { get; private set; }
    #endregion Properties..

    #region Constructors..
    public FileItem(string name, bool isDirectory, long fileSize)
    {
        Name = name;
        IsDirectory = isDirectory;
        FileSize = fileSize;
    }
    #endregion Constructors..
}
