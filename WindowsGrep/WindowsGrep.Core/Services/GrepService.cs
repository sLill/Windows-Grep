namespace WindowsGrep.Core;

public class GrepService
{
    #region Fields..
    private const string FILE_NAME_PATTERN = @"^(.+)[\/\\](?<FileName>[^\/\\]+)$";

    private static Regex _fileNameRegex = new Regex(FILE_NAME_PATTERN);
    private static object _metricsLock = new object();
    #endregion Fields..

    #region Methods..
    public void BeginProcessGrepCommand(GrepCommand grepCommand, CommandResultCollection commandResultCollection, CancellationToken cancellationToken)
    {
        switch (grepCommand.CommandType)
        {
            case GrepCommandType.Help:
                ConsoleUtils.PublishHelp(false);
                break;

            case GrepCommandType.Help_Full:
                ConsoleUtils.PublishHelp(true);
                break;

            case GrepCommandType.Query:
                Query(grepCommand, commandResultCollection, cancellationToken);
                break;
        }
    }

    private void Query(GrepCommand grepCommand, CommandResultCollection commandResultCollection, CancellationToken cancellationToken)
    {
        bool writeFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.OutFile);
        Stopwatch commandTimer = Stopwatch.StartNew();

        RegexOptions optionsFlags = CommandUtils.GetRegexOptions(grepCommand);
        ProcessCommand(commandResultCollection, grepCommand, optionsFlags, cancellationToken);

        if (writeFlag)
            commandResultCollection.Write(grepCommand.CommandArgs[ConsoleFlag.OutFile]);

        // Publish command run time
        commandTimer.Stop();

        ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"{Environment.NewLine}[{Math.Round((commandTimer.ElapsedMilliseconds / 1000.0), 2)} second(s)]" });
        ConsoleUtils.WriteConsoleItem(new ConsoleItem() { Value = Environment.NewLine + Environment.NewLine });
    }

    private void BuildFileContentSearchResults(GrepCommand grepCommand, CommandResultCollection commandResultCollection, List<Match> matches,
        FileItem file, string fileRaw, CancellationToken cancellationToken)
    {
        bool ignoreBreaksFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreBreaks);
        bool ignoreCaseFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.IgnoreCase);
        bool contextFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Context);

        // Build file context search pattern
        string searchTerm = grepCommand.CommandArgs[ConsoleFlag.SearchTerm];
        int contextLength = contextFlag ? Convert.ToInt32(grepCommand.CommandArgs[ConsoleFlag.Context]) : 0;

        matches.ForEach(match =>
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            string leadingContext = string.Empty;
            string trailingContext = string.Empty;

            if (contextFlag)
            {
                // Rebuild matches with contextual text
                int leadingContextStartIndex = match.Groups["MatchedString"].Index - contextLength < 0 ? 0 : match.Groups["MatchedString"].Index - contextLength;
                int leadingContextLength = match.Groups["MatchedString"].Index < contextLength ? match.Groups["MatchedString"].Index : contextLength;

                int trailingContextStartIndex = match.Groups["MatchedString"].Index + match.Groups["MatchedString"].Value.Length;
                int trailingContextLength = trailingContextStartIndex + contextLength > fileRaw.Length ? fileRaw.Length - trailingContextStartIndex : contextLength;

                leadingContext = Environment.NewLine + fileRaw.Substring(leadingContextStartIndex, leadingContextLength);
                trailingContext = fileRaw.Substring(trailingContextStartIndex, trailingContextLength) + Environment.NewLine;
            }

            string matchedString = match.Groups["MatchedString"].Value;

            GrepCommandResult grepResult = new GrepCommandResult(file, ResultScope.FileContent)
            {
                LeadingContextString = leadingContext,
                TrailingContextString = trailingContext,
                MatchedString = matchedString
            };

            // Line number
            int lineNumber = fileRaw.Substring(0, match.Groups["MatchedString"].Index).Split('\n').Length;
            grepResult.LineNumber = lineNumber;

            lock (commandResultCollection)
                commandResultCollection.AddItem(grepResult);
        });
    }

    private IEnumerable<FileItem> GetFiles(GrepCommand grepCommand, IList<CommandResultBase> commandResultCollection, string filepath, CancellationToken cancellationToken)
    {
        bool recursiveFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Recursive);
        bool maxDepthFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.MaxDepth);
        bool showHiddenFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.ShowHidden);
        bool showSystemFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.ShowSystem);

        long fileSizeMin = CommandUtils.GetFileSizeMinimum(grepCommand);
        long fileSizeMax = CommandUtils.GetFileSizeMaximum(grepCommand);

        int maxDepth = maxDepthFlag ? Convert.ToInt32(grepCommand.CommandArgs[ConsoleFlag.MaxDepth]) : int.MaxValue;

        IEnumerable<FileItem> files = null;

        if (commandResultCollection.Any())
            files = commandResultCollection.Select(result => result.SourceFile).DistinctBy(x => x.Name).ToList();
        else
        {
            FileAttributes fileAttributesToSkip = default;
            fileAttributesToSkip |= (showSystemFlag ? 0 : FileAttributes.System);
            fileAttributesToSkip |= (showHiddenFlag ? 0 : FileAttributes.Hidden);

            var pathExcludeFilters = CommandUtils.GetPathExcludeFilters(grepCommand);
            files = WindowsUtils.GetFiles(filepath, recursiveFlag, maxDepth, fileSizeMin, fileSizeMax, cancellationToken, pathExcludeFilters, fileAttributesToSkip);
        }

        return files;
    }

    private void GetFileContentMatches(CommandResultCollection commandResultCollection, IEnumerable<FileItem> files, GrepCommand grepCommand, string searchPattern,
        Regex searchRegex, SearchMetrics searchMetrics, CancellationToken cancellationToken)
    {
        bool fixedStringsFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FixedString);
        bool deleteFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
        bool replaceFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);
        bool writeFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.OutFile);
        bool fileNamesOnlyFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileNamesOnly);

        var fileTypeFilters = CommandUtils.GetFileTypeFilters(grepCommand);
        var fileTypeExcludeFilters = CommandUtils.GetFileTypeExcludeFilters(grepCommand);
        var pathFilters = CommandUtils.GetPathFilters(grepCommand);

        foreach (var file in files)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                // Filters
                bool isFiltered = false;
                isFiltered |= (fileTypeFilters != null && !fileTypeFilters.Contains(Path.GetExtension(file.Name).Trim('.')));
                isFiltered |= (fileTypeExcludeFilters != null && fileTypeExcludeFilters.Contains(Path.GetExtension(file.Name).Trim('.')));
                isFiltered |= (pathFilters != null && !pathFilters.Any(x => Regex.IsMatch(Path.GetDirectoryName(file.Name), x)));

                if (isFiltered || file.IsDirectory)
                    continue;

                string fileRaw = File.ReadAllText(file.Name);
                List<Match> matches = searchRegex.Matches(fileRaw).ToList();
                
                if (matches.Any())
                {
                    bool isWriteOperation = replaceFlag || deleteFlag;
                    if (isWriteOperation)
                        PerformWriteOperations(grepCommand, file, searchPattern, matches.Count, fileRaw, searchMetrics, cancellationToken);
                    else
                    {
                        BuildFileContentSearchResults(grepCommand, commandResultCollection, matches, file, fileRaw, cancellationToken);

                        lock (_metricsLock)
                            searchMetrics.TotalFilesMatchedCount++;
                    }
                }
            }
            catch
            {
                lock (_metricsLock)
                    searchMetrics.FailedReadFiles.Add(file);
            }
        }
    }

    private void GetFileHashMatches(CommandResultCollection commandResultCollection, IEnumerable<FileItem> files, GrepCommand grepCommand,
        string searchTerm, SearchMetrics searchMetrics, HashType hashType, CancellationToken cancellationToken)
    {
        var matches = new List<CommandResultBase>();

        bool deleteFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
        bool writeFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.OutFile);

        var fileTypeFilters = CommandUtils.GetFileTypeFilters(grepCommand);
        var fileTypeExcludeFilters = CommandUtils.GetFileTypeExcludeFilters(grepCommand);
        var pathFilters = CommandUtils.GetPathFilters(grepCommand);

        Regex anyRegex = new Regex(@".*");

        foreach (var file in files)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                // Filters
                bool isFiltered = false;
                isFiltered |= (fileTypeFilters != null && !fileTypeFilters.Contains(Path.GetExtension(file.Name).Trim('.')));
                isFiltered |= (fileTypeExcludeFilters != null && fileTypeExcludeFilters.Contains(Path.GetExtension(file.Name).Trim('.')));
                isFiltered |= (pathFilters != null && !pathFilters.Any(x => Regex.IsMatch(Path.GetDirectoryName(file.Name), x)));

                if (isFiltered || file.IsDirectory)
                    continue;

                // Compare file hashes
                string fileHash = WindowsUtils.GetFileHash(file.Name, hashType);
                if (string.Equals(fileHash, searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    // Parse filename from path
                    Match fileNameMatch = _fileNameRegex.Match(file.Name);
                    if (fileNameMatch != null)
                    {
                        Group fileNameGroup = fileNameMatch.Groups["FileName"];
                        Match searchMatch = anyRegex.Match(fileHash);

                        bool isWriteOperation = deleteFlag;
                        if (isWriteOperation)
                        {
                            PerformWriteOperations(grepCommand, file, searchTerm, 1, file.Name, searchMetrics, cancellationToken);
                            searchMetrics.TotalFilesMatchedCount++;
                        }
                        else
                        {
                            GrepCommandResult result = PerformReadOperations(grepCommand, file, fileNameMatch, searchMatch);
                            if (result != null)
                            {
                                searchMetrics.TotalFilesMatchedCount++;
                                commandResultCollection.AddItem(result);
                            }
                        }
                    }
                }
            }
            catch
            {
                lock (_metricsLock)
                    searchMetrics.FailedReadFiles.Add(file);
            }
        }
    }

    private void GetFileNameMatches(CommandResultCollection commandResultCollection, IEnumerable<FileItem> files, GrepCommand grepCommand,
        string searchPattern, Regex searchRegex, SearchMetrics searchMetrics, CancellationToken cancellationToken)
    {
        bool fixedStringsFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FixedString);
        bool deleteFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
        bool replaceFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);
        bool writeFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.OutFile);

        var fileTypeFilters = CommandUtils.GetFileTypeFilters(grepCommand);
        var fileTypeExcludeFilters = CommandUtils.GetFileTypeExcludeFilters(grepCommand);
        var pathFilters = CommandUtils.GetPathFilters(grepCommand);

        foreach (var file in files)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            // Filters
            bool isFiltered = false;
            isFiltered |= (fileTypeFilters != null && !fileTypeFilters.Contains(Path.GetExtension(file.Name).Trim('.')));
            isFiltered |= (fileTypeExcludeFilters != null && fileTypeExcludeFilters.Contains(Path.GetExtension(file.Name).Trim('.')));
            isFiltered |= (pathFilters != null && !pathFilters.Any(x => Regex.IsMatch(Path.GetDirectoryName(file.Name), x)));

            if (isFiltered || file.IsDirectory)
                continue;

            // Parse filename from path
            var fileNameMatch = _fileNameRegex.Match(file.Name)?.Groups["FileName"];
            if (fileNameMatch != null)
            {
                // Query against filename
                var searchMatch = searchRegex.Match(fileNameMatch.Value);
                if (searchMatch != Match.Empty)
                {
                    bool isWriteOperation = replaceFlag || deleteFlag;
                    if (isWriteOperation)
                    {
                        PerformWriteOperations(grepCommand, file, searchPattern, 1, file.Name, searchMetrics, cancellationToken);
                        searchMetrics.TotalFilesMatchedCount++;
                    }
                    else
                    {
                        GrepCommandResult result = PerformReadOperations(grepCommand, file, fileNameMatch, searchMatch);
                        if (result != null)
                        {
                            searchMetrics.TotalFilesMatchedCount++;
                            commandResultCollection.AddItem(result);
                        }
                    }
                }
            }
        }
    }

    private void ProcessCommand(CommandResultCollection commandResultCollection, GrepCommand grepCommand, RegexOptions optionsFlags, CancellationToken cancellationToken)
    {
        bool fileNamesOnlyFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileNamesOnly);
        bool fileHashesOnlyFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileHashes);

        string filepath = CommandUtils.GetPath(grepCommand);
        HashType hashType = CommandUtils.GetHashType(grepCommand);

        IEnumerable<FileItem> files = GetFiles(grepCommand, commandResultCollection, filepath, cancellationToken);

        // Clear the result collection between chained commands so that only the results of the final command are returned
        commandResultCollection.Clear();

        var searchMetrics = new SearchMetrics();

        // Build content search pattern
        string searchTerm = grepCommand.CommandArgs[ConsoleFlag.SearchTerm];
        string searchPattern = CommandUtils.BuildSearchPattern(grepCommand);
        Regex searchRegex = new Regex(searchPattern, optionsFlags);

        if (fileHashesOnlyFlag)
        {
            bool isValidFileHash = WindowsUtils.IsValidFileHash(searchTerm, hashType);
            if (!isValidFileHash)
                throw new Exception($"Error: Hash does not match {hashType} format");

            GetFileHashMatches(commandResultCollection, files, grepCommand, searchTerm, searchMetrics, hashType, cancellationToken);
        }
        else if (fileNamesOnlyFlag)
            GetFileNameMatches(commandResultCollection, files, grepCommand, searchPattern, searchRegex, searchMetrics, cancellationToken);
        else
        {
            if (grepCommand.CommandArgs[ConsoleFlag.SearchTerm] == string.Empty)
                throw new Exception("Error: Search term not supplied");

            GetFileContentMatches(commandResultCollection, files, grepCommand, searchPattern, searchRegex, searchMetrics, cancellationToken);
        }

        // Notify user of files that could not be read from or written to
        PublishFileAccessSummary(grepCommand, searchMetrics);

        // Publish command summary to console
        PublishCommandSummary(grepCommand, commandResultCollection, searchMetrics);
    }

    private GrepCommandResult PerformReadOperations(GrepCommand grepCommand, FileItem file, Group fileNameMatch, Match searchMatch)
    {
        GrepCommandResult result = null;

        bool fileHashesFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileHashes);

        var resultScope = fileHashesFlag ? ResultScope.FileHash : ResultScope.FileName;

        string leadingContextString = string.Empty;
        int leadingContextStringStartIndex = 0;
        int leadingContextStringEndIndex = 0;

        string trailingContextString = string.Empty;
        int trailingContextStringStartIndex = 0;
        int trailingContextStringEndIndex = 0;

        switch (resultScope)
        {
            case ResultScope.FileName:
                leadingContextStringEndIndex = fileNameMatch.Index + searchMatch.Index;
                trailingContextStringStartIndex = fileNameMatch.Index + searchMatch.Index + searchMatch.Length;
                trailingContextStringEndIndex = file.Name.Length - trailingContextStringStartIndex;

                leadingContextString = file.Name.Substring(leadingContextStringStartIndex, leadingContextStringEndIndex);
                trailingContextString = file.Name.Substring(trailingContextStringStartIndex, trailingContextStringEndIndex);
                break;

            case ResultScope.FileHash:
                break;
        }

        result = new GrepCommandResult(file, resultScope)
        {
            LeadingContextString = leadingContextString,
            MatchedString = searchMatch.Value,
            TrailingContextString = trailingContextString
        };

        return result;
    }

    private List<ConsoleItem> PerformWriteOperations(GrepCommand grepCommand, FileItem file, string searchPattern,
        int fileMatchesCount, string fileRaw, SearchMetrics searchMetrics, CancellationToken cancellationToken)
    {
        var consoleItemCollection = new List<ConsoleItem>();

        bool deleteFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
        bool replaceFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);
        bool fileNamesOnlyFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileNamesOnly);

        // FileName
        consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkYellow, Value = $"{file.Name} " });

        try
        {
            if (deleteFlag)
            {
                File.Delete(file.Name);

                consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"Deleted" });

                lock (_metricsLock)
                    searchMetrics.DeleteSuccessCount++;
            }
            else if (replaceFlag)
            {
                if (fileNamesOnlyFlag)
                {
                    string directory = Path.GetDirectoryName(file.Name);
                    string fileName = Path.GetFileName(file.Name);

                    File.Move(file.Name, Path.Combine(directory, Regex.Replace(fileName, searchPattern, grepCommand.CommandArgs[ConsoleFlag.Replace])));
                    consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"Renamed" });
                }
                else
                {
                    // Replace all matches in file
                    fileRaw = Regex.Replace(fileRaw, searchPattern, grepCommand.CommandArgs[ConsoleFlag.Replace]);
                    File.WriteAllText(file.Name, fileRaw);

                    consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.DarkMagenta, Value = $"{fileMatchesCount} match(es)" });
                }

                lock (_metricsLock)
                    searchMetrics.ReplacedSuccessCount += fileMatchesCount;
            }
        }
        catch
        {
            consoleItemCollection.Add(new ConsoleItem() { ForegroundColor = ConsoleColor.Gray, BackgroundColor = ConsoleColor.DarkRed, Value = $"Access Denied" });

            lock (_metricsLock)
                searchMetrics.FailedWriteFiles.Add(file);
        }
        finally
        {
            // Empty buffer
            consoleItemCollection.Add(new ConsoleItem() { Value = Environment.NewLine });

            ConsoleUtils.WriteConsoleItemCollection(consoleItemCollection, cancellationToken);

            lock (_metricsLock)
                searchMetrics.TotalFilesMatchedCount++;
        }

        return consoleItemCollection;
    }

    private void PublishCommandSummary(GrepCommand grepCommand, IList<CommandResultBase> commandResultCollection, SearchMetrics searchMetrics)
    {
        bool deleteFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Delete);
        bool replaceFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Replace);
        bool fileSizeMinimumFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMinimum);
        bool fileSizeMaximumFlag = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.FileSizeMaximum);

        string summary = string.Empty;

        if (deleteFlag)
            summary = $"[{searchMetrics.DeleteSuccessCount} of {searchMetrics.TotalFilesMatchedCount} file(s) deleted]";
        else if (replaceFlag)
            summary = $"[{searchMetrics.ReplacedSuccessCount} occurrence(s) replaced in {searchMetrics.TotalFilesMatchedCount} file(s)]";
        else
            summary = $"[{commandResultCollection.Count} result(s) {searchMetrics.TotalFilesMatchedCount} file(s)]";

        ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = summary });

        if (fileSizeMinimumFlag || fileSizeMaximumFlag)
        {
            var totalFileSize = commandResultCollection.Sum(x => x.SourceFile.FileSize);
            var fileSizeReduced = WindowsUtils.GetReducedSize(totalFileSize, 3, out FileSizeType fileSizeType);

            summary = $" [{fileSizeReduced} {fileSizeType}(s)]";
            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = summary });
        }
    }

    private void PublishFileAccessSummary(GrepCommand grepCommand, SearchMetrics searchMetrics)
    {
        bool verbose = grepCommand.CommandArgs.ContainsKey(ConsoleFlag.Verbose);

        if (searchMetrics.FailedReadFiles.Any() || searchMetrics.FailedWriteFiles.Any())
        {
            if (searchMetrics.FailedReadFiles.Any())
            {
                string unreachableFiles = $"[{searchMetrics.FailedReadFiles.Count} file(s) unreadable/inaccessible]{Environment.NewLine}";
                ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = unreachableFiles });

                if (verbose)
                    searchMetrics.FailedReadFiles.ForEach(x => ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"{x.Name}{Environment.NewLine}" }));
            }

            if (searchMetrics.FailedWriteFiles.Any())
            {
                string unwriteableFiles = $"[{searchMetrics.FailedWriteFiles.Count} file(s) could not be modified]{Environment.NewLine}";
                ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = unwriteableFiles });

                if (verbose)
                    searchMetrics.FailedWriteFiles.ForEach(x => ConsoleUtils.WriteConsoleItem(new ConsoleItem() { ForegroundColor = ConsoleColor.Red, Value = $"{x.Name}{Environment.NewLine}" }));
            }

            ConsoleUtils.WriteConsoleItem(new ConsoleItem() { Value = Environment.NewLine });
        }
    }
    #endregion Methods..
}
