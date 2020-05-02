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
        TargetFile,

        // Ignore case distinctions in patterns and input data
        [DescriptionCollection("-i", "--ignore-case")]
        IgnoreCase,

        // Searches also in the subdirectories of the target directory
        [DescriptionCollection("-r", "--recursive")]
        Recursive,

        // Restricts search to files with the specified extensions. Comma delimited.
        [ExpectsParameter(true)]
        [DescriptionCollection("-t", "--filetype-include=")]
        FileTypeInclusions,

        // Excludes all files with the specified extensions. Comma delimited
        [ExpectsParameter(true)]
        [DescriptionCollection("-T", "--filetype-exclude=")]
        FileTypeExclusions,

        // Match against file names rather than file content
        [DescriptionCollection("-k", "--filenames-only")]
        FileNamesOnly,

        // Write outputs to specified file
        [ExpectsParameter(true)]
        [DescriptionCollection("-w", "--write=")]
        Write
    }
}
