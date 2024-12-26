public enum FileSizeType
{
    // Value attributes represented in bytes
    [ValueAttribute(1000)]
    Kb,

    [ValueAttribute(1000000)]
    Mb,

    [ValueAttribute(1000000000)]
    Gb,

    [ValueAttribute(1000000000000)]
    Tb
}