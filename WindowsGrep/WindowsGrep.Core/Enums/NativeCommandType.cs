﻿namespace WindowsGrep.Core;

public enum NativeCommandType
{
    [DescriptionCollection("ls")]
    List,

    [ExpectsParameter(true)]
    [DescriptionCollection("cd")]
    ChangeDirectory,

    [DescriptionCollection("clear")]
    ClearConsole,

    [DescriptionCollection("pwd")]
    PrintWorkingDirectory
}
