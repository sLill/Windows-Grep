namespace WindowsGrep.Common
{
    public enum ConsoleFlag
    {
        // The only required filter provided by the user
        SearchTerm,

        // Targets a specific file directory
        [ExpectsParameter(true)]
        [DescriptionCollection("-d", "--directory=")]
        Directory,

        // Suppress normal output; Instead print a count of matching lines for each input file
        //[DescriptionCollection("-c", "--count")]
        //Count,

        // Interprets patterns as fixed strings, not regular expressions
        [DescriptionCollection("-F", "--fixed-strings")]
        FixedStrings,

        // Interprets patterns as basic regular expressions. This is default
        [DescriptionCollection("-G", "--basic-regexp")]
        BasicRegex,

        // Obtain patterns from a specific file
        [ExpectsParameter(true)]
        [DescriptionCollection("-f", "--file=")]
        File,

        // Ignore case distinctions in patterns and input data
        [DescriptionCollection("-i", "--ignore-case")]
        IgnoreCase,

        // Searches also in the subdirectories of the target directory
        [DescriptionCollection("-r", "--recursive")]
        Recursive
    }
}
