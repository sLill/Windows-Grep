using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsGrep.Common
{
    public static class ConsoleFlags
    {
        #region Properties..
        #region Pattern Syntax
        // Interprets patterns as fixed strings, not regular expressions
        public const string FixedStringsCharacterCode = "-F";
        public const string FixedStrings = "--fixed-strings";

        // Interprets patterns as basic regular expressions. This is default
        public const string BasicRegexCharacterCode = "-G";
        public const string BasicRegex = "--basic-regexp";
        #endregion Pattern Syntax

        #region Matching Control
        // Obtain patterns from a specific file
        public const string FileCharacterCode = "-f";
        public const string File = "--file=";

        // Ignore case distinctions in patterns and input data
        public const string IgnoreCaseCharacterCode = "-i";
        public const string IgnoreCase = "--ignore-case";
        #endregion Matching Control

        #region General Output Control
        // Suppress normal output; Instead print a count of matching lines for each input file
        public const string CountCharacterCode = "-c";
        public const string Count = "--count";
        #endregion General Output Control

        #region File/Directory Selection
        // Searches also in the subdirectories of the target directory
        public const string RecursiveCharacterCode = "-r";
        public const string Recursive = "--recursive";
        #endregion File/Directory Selection
        #endregion Properties..
    }
}
