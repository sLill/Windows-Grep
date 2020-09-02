using System;

namespace WindowsGrep.Common
{
    public enum Shortcut
    {
        [KeyId(1000)]
        [KeyModifier(KeyModifier.Ctrl)]
        [KeyAttribute(46)]
        CtrlC
    }
}
