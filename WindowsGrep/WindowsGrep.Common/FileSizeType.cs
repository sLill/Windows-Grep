using WindowsGrep.Common;

public enum FileSizeType
{
    // Value attributes represented in bytes
    [ValueAttribute(1000)]
    KB,

    [ValueAttribute(1000000)]
    MB,

    [ValueAttribute(1000000000)]
    GB,

    [ValueAttribute(1000000000000)]
    TB
}