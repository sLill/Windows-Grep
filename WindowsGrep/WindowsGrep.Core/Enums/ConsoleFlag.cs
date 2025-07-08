namespace WindowsGrep.Core;

public enum ConsoleFlag
{
    SearchTerm,

    // Set the working directory
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"', '\\')]
    [DescriptionCollection("-d", "--directory=")]
    Directory,

    // Suppress normal output; Instead print a count of matching lines for each input file
    //[DescriptionCollection("-c", "--count")]
    //Count,

    // Returns local text surrounding the search term in each result
    [ExpectsParameter(true)]
    [DescriptionCollection("-c", "--context=")]
    Context,

    // Interprets search term as a string literal instead of an expression
    [DescriptionCollection("-F", "--fixed-string")]
    FixedString,

    // Ignore breaks in-between lines within the file
    [DescriptionCollection("-b", "--ignore-breaks")]
    IgnoreBreaks,

    // Ignore case distinctions in patterns and input data
    [DescriptionCollection("-i", "--ignore-case")]
    IgnoreCase,

    // Searches also in the subdirectories of the target directory
    [DescriptionCollection("-r", "--recursive")]
    Recursive,

    // Sets the max search depth when recursion (-r) is enabled
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"')]
    [DescriptionCollection("--max-depth=")]
    MaxDepth, 

    // Restricts search to files with the specified extensions. Comma delimited
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"', '.', '\\')]
    [DescriptionCollection("-t", "--filetype-include=")]
    FileTypeIncludeFilter,

    // Excludes all files with the specified extensions. Comma delimited
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"', '.', '\\')]
    [DescriptionCollection("-T", "--filetype-exclude=")]
    FileTypeExcludeFilter,

    // Restricts search to filepaths matching the specified expressions. Comma delimited 
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"')]
    [DescriptionCollection("-p", "--path-include=")]
    PathIncludeFilter,

    // Restricts search to filepaths that do not match any of the specified expressions. Comma delimited 
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"')]
    [DescriptionCollection("-P", "--path-exclude=")]
    PathExcludeFilter,

    // Match against file names
    [DescriptionCollection("-k", "--filenames")]
    FileNamesOnly,

    // Return the filesize of items returned in the search that EXCEED this parameter
    // as well as the cumulative size of all files returned
    // Default size is kb unless specified. Other accepted sizes are mb, gb, tb
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"')]
    [DescriptionCollection("--filesize-min=")]
    FileSizeMinimum,

    // Return the filesize of items returned in the search that DO NOT EXCEED this parameter
    // as well as the cumulative size of all files returned
    // Default size is kb unless specified. Other accepted sizes are mb, gb, tb
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"')]
    [DescriptionCollection("--filesize-max=")]
    FileSizeMaximum,

    // Redirect output to file
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"', '\\')]
    [DescriptionCollection("-o", "--out-file=")]
    OutFile,

    // Replace instances of the search term with the replace parameter
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"')]
    [DescriptionCollection("-RX", "--replace=")]
    Replace,

    // Delete files returned in search
    [DescriptionCollection("-DX", "--delete")]
    Delete,

    // Match againt file hashes (0=SHA256, 1=MD5)
    [ExpectsParameter(true)]
    [DescriptionCollection("--hash=")]
    FileHashes,

    // Show help
    [DescriptionCollection("-h")]
    Help,

    // Show help (full)
    [DescriptionCollection("--help")]
    Help_Full,

    // Show hidden files
    [DescriptionCollection("--show-hidden")]
    ShowHidden,

    // Show system files
    [DescriptionCollection("--show-system")]
    ShowSystem,

    [DescriptionCollection("-v", "--verbose")]
    Verbose
}
