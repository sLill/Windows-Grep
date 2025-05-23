﻿namespace WindowsGrep.Core;

public enum ConsoleFlag
{
    SearchTerm,

    // Targets a specific file directory
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

    // Returns the first n results
    [ExpectsParameter(true)]
    [DescriptionCollection("-n", "--results=")]
    NResults,

    // Suppresses console output
    [DescriptionCollection("-s", "--suppress")]
    Suppress,

    // Interprets patterns as fixed strings, not regular expressions
    [DescriptionCollection("-F", "--fixed-strings")]
    FixedStrings,

    // Interprets patterns as basic regular expressions. This is default
    [DescriptionCollection("-G", "--basic-regexp")]
    BasicRegex,

    // Obtain patterns from a specific file
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"', '\\')]
    [DescriptionCollection("-f", "--file=")]
    TargetFile,

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
    [DescriptionCollection("-t", "--filetype-filter=")]
    FileTypeFilter,

    // Excludes all files with the specified extensions. Comma delimited
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"', '.', '\\')]
    [DescriptionCollection("-T", "--filetype-exclude-filter=")]
    FileTypeExcludeFilter,

    // Restricts search to filepaths matching the specified expressions. Comma delimited 
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"')]
    [DescriptionCollection("-p", "--path-filter=")]
    PathFilter,

    // Restricts search to filepaths that do not match any of the specified expressions. Comma delimited 
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"')]
    [DescriptionCollection("-P", "--path-exclude-filter=")]
    PathExcludeFilter,

    // Match against file names
    [DescriptionCollection("-k", "--filenames-only")]
    FileNamesOnly,

    // Return the filesize of items returned in the search that EXCEED this parameter
    // as well as the cumulative size of all files returned
    // Default size is kb unless specified. Other accepted sizes are mb, gb, tb
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"')]
    [DescriptionCollection("-z", "--filesize-minimum=")]
    FileSizeMinimum,

    // Return the filesize of items returned in the search that DO NOT EXCEED this parameter
    // as well as the cumulative size of all files returned
    // Default size is kb unless specified. Other accepted sizes are mb, gb, tb
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"')]
    [DescriptionCollection("-Z", "--filesize-maximum=")]
    FileSizeMaximum,

    // Write outputs to specified file
    [ExpectsParameter(true)]
    [FilterCharacterCollection('\'', '"', '\\')]
    [DescriptionCollection("-w", "--write=")]
    Write,

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

    // Show all commands
    [DescriptionCollection("-h", "--help")]
    Help,

    // Show hidden files
    [DescriptionCollection("--show-hidden")]
    ShowHidden,

    // Show system files
    [DescriptionCollection("--show-system")]
    ShowSystem,
}
