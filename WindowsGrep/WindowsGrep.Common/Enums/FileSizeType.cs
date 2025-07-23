namespace WindowsGrep.Common;
public enum FileSizeType
{
    // Value attributes represented in bytes
    [Value(1)]
    BYTES,

    [Value(1000)]
    KB,

    [Value(1000000)]
    MB,

    [Value(1000000000)]
    GB,

    [Value(1000000000000)]
    TB
}