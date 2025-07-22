namespace WindowsGrep.Core;

public class GrepService
{
    #region Fields..
    private const string FILE_NAME_PATTERN = @"^(.+)[\/\\](?<FileName>[^\/\\]+)$";

    private readonly PublisherService _publisherService;
    private readonly ConsoleService _consoleService;
    private readonly FileService _fileService;
  
    private static Regex _fileNameRegex = new Regex(FILE_NAME_PATTERN);
    private static object _metricsLock = new object();
    #endregion Fields..

    #region Constructors..
    public GrepService(PublisherService publisherService, ConsoleService consoleService, FileService fileService)
    {
        _publisherService = publisherService;
        _consoleService = consoleService;
        _fileService = fileService;
    }
    #endregion Constructors..

    #region Methods..
    public void RunCommand(GrepCommand grepCommand, List<ResultBase> results, CancellationToken cancellationToken)
    {
        _publisherService.Subscribe<ConsoleItem>(_consoleService.Write);

        bool writeFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.OutFile);
        if (writeFlag)
        {
            _fileService.Initialize(grepCommand.CommandArgs[CommandFlag.OutFile]);
            _publisherService.Subscribe<ConsoleItem>(_fileService.Write);
        }

        Stopwatch commandTimer = Stopwatch.StartNew();

        RegexOptions optionsFlags = CommandFlagUtils.GetRegexOptions(grepCommand);

        try
        {
            ProcessCommand(results, grepCommand, optionsFlags, cancellationToken);
        }
        catch (Exception ex)
        {
            _publisherService.Publish(new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = ex.Message });
        }

        // Publish command run time
        commandTimer.Stop();

        _publisherService.Publish(new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = $"{Environment.NewLine}[{Math.Round((commandTimer.ElapsedMilliseconds / 1000.0), 2)} second(s)]" });
        _publisherService.Publish(new ConsoleItem { Value = Environment.NewLine + Environment.NewLine });
    }

    private void BuildFileContentSearchResults(GrepCommand grepCommand, List<ResultBase> results, List<Match> matches,
        FileItem file, string fileRaw, CancellationToken cancellationToken)
    {
        bool ignoreBreaksFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.IgnoreBreaks);
        bool ignoreCaseFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.IgnoreCase);
        bool contextFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.Context);

        // Build file context search pattern
        int contextLength = contextFlag ? Convert.ToInt32(grepCommand.CommandArgs[CommandFlag.Context]) : 0;

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

            var result = new GrepResult(file, ResultScope.FileContent)
            {
                LeadingContextString = leadingContext,
                TrailingContextString = trailingContext,
                MatchedString = matchedString
            };

            // Line number
            int lineNumber = fileRaw.Substring(0, match.Groups["MatchedString"].Index).Split('\n').Length;
            result.LineNumber = lineNumber;

            lock (results)
            {
                results.Add(result);
                result.ToConsoleItemCollection().ForEach(y => _publisherService.Publish(y));
            }
        });
    }

    private IEnumerable<FileItem> GetFiles(GrepCommand grepCommand, List<ResultBase> results, string path, CancellationToken cancellationToken)
    {
        bool recursiveFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.Recursive);
        bool maxDepthFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.MaxDepth);
        bool includeHiddenFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.IncludeHidden);
        bool includeSystemFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.IncludeSystem);
        bool fileSizeMinimumFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.FileSizeMinimum);
        bool fileSizeMaximumFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.FileSizeMaximum);

        long fileSizeMin = fileSizeMinimumFlag ? CommandFlagUtils.GetFileSize(grepCommand.CommandArgs[CommandFlag.FileSizeMinimum]) : -1;
        long fileSizeMax = fileSizeMaximumFlag ? CommandFlagUtils.GetFileSize(grepCommand.CommandArgs[CommandFlag.FileSizeMaximum]) : -1;

        int maxDepth = maxDepthFlag ? Convert.ToInt32(grepCommand.CommandArgs[CommandFlag.MaxDepth]) : int.MaxValue;

        IEnumerable<FileItem> files = null;

        if (results.Count > 0)
            files = results.Select(result => result.SourceFile).DistinctBy(x => x.Name).ToList();
        else
        {
            FileAttributes fileAttributesToSkip = default;
            fileAttributesToSkip |= (includeSystemFlag ? 0 : FileAttributes.System);
            fileAttributesToSkip |= (includeHiddenFlag ? 0 : FileAttributes.Hidden);

            var pathExcludeFilters = CommandFlagUtils.GetPathExcludeFilters(grepCommand);
            files = WindowsUtils.GetFiles(path, recursiveFlag, maxDepth, fileSizeMin, fileSizeMax, cancellationToken, pathExcludeFilters, fileAttributesToSkip);
        }

        return files;
    }

    private void GetFileContentMatches(List<ResultBase> results, IEnumerable<FileItem> files, GrepCommand grepCommand, string searchPattern,
        Regex searchRegex, SearchMetrics searchMetrics, CancellationToken cancellationToken)
    {
        bool fixedStringsFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.FixedString);
        bool deleteFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.Delete);
        bool replaceFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.Replace);
        bool writeFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.OutFile);
        bool fileNamesOnlyFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.FileNamesOnly);

        var filetypeIncludeFilters = CommandFlagUtils.GetFileTypeIncludeFilters(grepCommand);
        var filetypeExcludeFilters = CommandFlagUtils.GetFileTypeExcludeFilters(grepCommand);
        var pathIncludeFilters = CommandFlagUtils.GetPathIncludeFilters(grepCommand);

        foreach (var file in files)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                // Filters
                bool isFiltered = false;
                isFiltered |= (filetypeIncludeFilters != null && !filetypeIncludeFilters.Contains(Path.GetExtension(file.Name).TrimOnce('.')));
                isFiltered |= (filetypeExcludeFilters != null && filetypeExcludeFilters.Contains(Path.GetExtension(file.Name).TrimOnce('.')));
                isFiltered |= (pathIncludeFilters != null && !pathIncludeFilters.Any(x => Path.GetDirectoryName(file.Name)?.Contains(x.TrimOnce('\'', '"')) ?? false));

                if (isFiltered || file.IsDirectory)
                    continue;

                string fileRaw = File.ReadAllText(file.Name);
                List<Match> matches = searchRegex.Matches(fileRaw).ToList();
                
                if (matches.Count > 0)
                {
                    bool isWriteOperation = replaceFlag || deleteFlag;
                    if (isWriteOperation)
                        PerformWriteOperations(grepCommand, file, searchPattern, matches.Count, fileRaw, searchMetrics, cancellationToken);
                    else
                    {
                        BuildFileContentSearchResults(grepCommand, results, matches, file, fileRaw, cancellationToken);

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

    private void GetFileHashMatches(List<ResultBase> results, IEnumerable<FileItem> files, GrepCommand grepCommand,
        string searchTerm, SearchMetrics searchMetrics, HashType hashType, CancellationToken cancellationToken)
    {
        var matches = new List<ResultBase>();

        bool deleteFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.Delete);
        bool writeFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.OutFile);

        var filetypeIncludeFilters = CommandFlagUtils.GetFileTypeIncludeFilters(grepCommand);
        var filetypeExcludeFilters = CommandFlagUtils.GetFileTypeExcludeFilters(grepCommand);
        var pathIncludeFilters = CommandFlagUtils.GetPathIncludeFilters(grepCommand);

        Regex anyRegex = new Regex(@".*");

        foreach (var file in files)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                // Filters
                bool isFiltered = false;
                isFiltered |= (filetypeIncludeFilters != null && !filetypeIncludeFilters.Contains(Path.GetExtension(file.Name).TrimOnce('.')));
                isFiltered |= (filetypeExcludeFilters != null && filetypeExcludeFilters.Contains(Path.GetExtension(file.Name).TrimOnce('.')));
                isFiltered |= (pathIncludeFilters != null && !pathIncludeFilters.Any(x => Path.GetDirectoryName(file.Name)?.Contains(x.TrimOnce('\'', '"')) ?? false));

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
                            GrepResult result = PerformReadOperations(grepCommand, file, fileNameMatch, searchMatch);
                            if (result != null)
                            {
                                searchMetrics.TotalFilesMatchedCount++;
                                results.Add(result);
                                result.ToConsoleItemCollection().ForEach(y => _publisherService.Publish(y));
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

    private void GetFileNameMatches(List<ResultBase> results, IEnumerable<FileItem> files, GrepCommand grepCommand,
        string searchPattern, Regex searchRegex, SearchMetrics searchMetrics, CancellationToken cancellationToken)
    {
        bool fixedStringsFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.FixedString);
        bool deleteFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.Delete);
        bool replaceFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.Replace);
        bool writeFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.OutFile);

        var filetypeIncludeFilters = CommandFlagUtils.GetFileTypeIncludeFilters(grepCommand);
        var filetypeExcludeFilters = CommandFlagUtils.GetFileTypeExcludeFilters(grepCommand);
        var pathIncludeFilters = CommandFlagUtils.GetPathIncludeFilters(grepCommand);

        foreach (var file in files)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            // Filters
            bool isFiltered = false;
            isFiltered |= (filetypeIncludeFilters != null && !filetypeIncludeFilters.Contains(Path.GetExtension(file.Name).TrimOnce('.')));
            isFiltered |= (filetypeExcludeFilters != null && filetypeExcludeFilters.Contains(Path.GetExtension(file.Name).TrimOnce('.')));
            isFiltered |= (pathIncludeFilters != null && !pathIncludeFilters.Any(x => Path.GetDirectoryName(file.Name)?.Contains(x.TrimOnce('\'', '"')) ?? false));

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
                        GrepResult result = PerformReadOperations(grepCommand, file, fileNameMatch, searchMatch);
                        if (result != null)
                        {
                            searchMetrics.TotalFilesMatchedCount++;
                            results.Add(result);
                            result.ToConsoleItemCollection().ForEach(y => _publisherService.Publish(y));
                        }
                    }
                }
            }
        }
    }

    private void ProcessCommand(List<ResultBase> results, GrepCommand grepCommand, RegexOptions optionsFlags, CancellationToken cancellationToken)
    {
        bool fileNamesOnlyFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.FileNamesOnly);
        bool fileHashesOnlyFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.FileHashes);

        string path = grepCommand.CommandArgs[CommandFlag.Path];
        HashType hashType = CommandFlagUtils.GetHashType(grepCommand);

        IEnumerable<FileItem> files = GetFiles(grepCommand, results, path, cancellationToken);

        // Clear the result collection between chained commands so that only the results of the final command are returned
        results.Clear();

        var searchMetrics = new SearchMetrics();

        // Build content search pattern
        string searchTerm = grepCommand.CommandArgs[CommandFlag.SearchTerm];
        string searchPattern = CommandFlagUtils.BuildSearchPattern(grepCommand);
        Regex searchRegex = new Regex(searchPattern, optionsFlags);

        if (fileHashesOnlyFlag)
        {
            bool isValidFileHash = WindowsUtils.IsValidFileHash(searchTerm, hashType);
            if (!isValidFileHash)
                throw new Exception($"Error: Hash does not match {hashType} format");

            GetFileHashMatches(results, files, grepCommand, searchTerm, searchMetrics, hashType, cancellationToken);
        }
        else if (fileNamesOnlyFlag)
            GetFileNameMatches(results, files, grepCommand, searchPattern, searchRegex, searchMetrics, cancellationToken);
        else
        {
            if (grepCommand.CommandArgs[CommandFlag.SearchTerm] == string.Empty)
                throw new Exception("Error: Search term not supplied");

            GetFileContentMatches(results, files, grepCommand, searchPattern, searchRegex, searchMetrics, cancellationToken);
        }

        // Notify user of files that could not be read from or written to
        PublishFileAccessSummary(grepCommand, searchMetrics);

        // Publish command summary to console
        PublishCommandSummary(grepCommand, results, searchMetrics);
    }

    private GrepResult PerformReadOperations(GrepCommand grepCommand, FileItem file, Group fileNameMatch, Match searchMatch)
    {
        GrepResult result = null;

        bool fileHashesFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.FileHashes);

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

        result = new GrepResult(file, resultScope)
        {
            LeadingContextString = leadingContextString,
            MatchedString = searchMatch.Value,
            TrailingContextString = trailingContextString
        };

        return result;
    }

    private void PerformWriteOperations(GrepCommand grepCommand, FileItem file, string searchPattern,
        int fileMatchesCount, string fileRaw, SearchMetrics searchMetrics, CancellationToken cancellationToken)
    {
        bool deleteFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.Delete);
        bool replaceFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.Replace);
        bool fileNamesOnlyFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.FileNamesOnly);

        // FileName
        _publisherService.Publish(new ConsoleItem { ForegroundColor = AnsiColors.DarkYellow, Value = $"{file.Name} " });

        try
        {
            if (deleteFlag)
            {
                File.Delete(file.Name);

                _publisherService.Publish(new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = $"Deleted" });

                lock (_metricsLock)
                    searchMetrics.DeleteSuccessCount++;
            }
            else if (replaceFlag)
            {
                if (fileNamesOnlyFlag)
                {
                    string directory = Path.GetDirectoryName(file.Name);
                    string fileName = Path.GetFileName(file.Name);

                    File.Move(file.Name, Path.Combine(directory, Regex.Replace(fileName, searchPattern, grepCommand.CommandArgs[CommandFlag.Replace])));
                    _publisherService.Publish(new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = $"Renamed" });
                }
                else
                {
                    // Replace all matches in file
                    fileRaw = Regex.Replace(fileRaw, searchPattern, grepCommand.CommandArgs[CommandFlag.Replace]);
                    File.WriteAllText(file.Name, fileRaw);

                    _publisherService.Publish(new ConsoleItem { ForegroundColor = AnsiColors.DarkMagenta, Value = $"{fileMatchesCount} match(es)" });
                }

                lock (_metricsLock)
                    searchMetrics.ReplacedSuccessCount += fileMatchesCount;
            }
        }
        catch
        {
            _publisherService.Publish(new ConsoleItem { ForegroundColor = AnsiColors.Gray, BackgroundColor = AnsiColors.DarkRedBg, Value = $"Access Denied" });

            lock (_metricsLock)
                searchMetrics.FailedWriteFiles.Add(file);
        }
        finally
        {
            // Empty buffer
            _publisherService.Publish(new ConsoleItem { Value = Environment.NewLine });

            lock (_metricsLock)
                searchMetrics.TotalFilesMatchedCount++;
        }
    }

    private void PublishCommandSummary(GrepCommand grepCommand, List<ResultBase> results, SearchMetrics searchMetrics)
    {
        bool deleteFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.Delete);
        bool replaceFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.Replace);
        bool fileSizeMinimumFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.FileSizeMinimum);
        bool fileSizeMaximumFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.FileSizeMaximum);

        string summary = string.Empty;

        if (deleteFlag)
            summary = $"[{searchMetrics.DeleteSuccessCount} of {searchMetrics.TotalFilesMatchedCount} file(s) deleted]";
        else if (replaceFlag)
            summary = $"[{searchMetrics.ReplacedSuccessCount} occurrence(s) replaced in {searchMetrics.TotalFilesMatchedCount} file(s)]";
        else
            summary = $"[{results.Count} result(s) {searchMetrics.TotalFilesMatchedCount} file(s)]";

        _publisherService.Publish(new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = summary });

        if (fileSizeMinimumFlag || fileSizeMaximumFlag)
        {
            var totalFileSize = results.Sum(x => x.SourceFile.FileSize);
            var fileSizeReduced = WindowsUtils.GetReducedSize(totalFileSize, 3, out FileSizeType fileSizeType);

            summary = $" [{fileSizeReduced} {fileSizeType}(s)]";
            _publisherService.Publish(new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = summary });
        }
    }

    private void PublishFileAccessSummary(GrepCommand grepCommand, SearchMetrics searchMetrics)
    {
        bool verbose = grepCommand.CommandArgs.ContainsKey(CommandFlag.Verbose);

        if (searchMetrics.FailedReadFiles.Any() || searchMetrics.FailedWriteFiles.Any())
        {
            if (searchMetrics.FailedReadFiles.Any())
            {
                string unreachableFiles = $"[{searchMetrics.FailedReadFiles.Count} file(s) unreadable/inaccessible]";
                _publisherService.Publish(new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = unreachableFiles });

                if (verbose)
                    searchMetrics.FailedReadFiles.ForEach(x => _publisherService.Publish(new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = $"{x.Name}{Environment.NewLine}" }));
            }

            if (searchMetrics.FailedWriteFiles.Any())
            {
                string unwriteableFiles = $"[{searchMetrics.FailedWriteFiles.Count} file(s) could not be modified]{Environment.NewLine}";
                _publisherService.Publish(new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = unwriteableFiles });

                if (verbose)
                    searchMetrics.FailedWriteFiles.ForEach(x => _publisherService.Publish(new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = $"{x.Name}{Environment.NewLine}" }));
            }

            _publisherService.Publish(new ConsoleItem { Value = Environment.NewLine });
        }
    }
    #endregion Methods..
}
