namespace WindowsGrep.Core;

public class GrepService
{
    #region Fields..
    private const string FILE_NAME_PATTERN = @"^(.+)[\/\\](?<FileName>[^\/\\]+)$";

    private readonly PublisherService _publisherService;
    private readonly ConsoleService _consoleService;
    private readonly FileService _fileService;
  
    private static Regex _fileNameRegex = new Regex(FILE_NAME_PATTERN, RegexOptions.Compiled);
    private static Regex _anyRegex = new Regex(@".*", RegexOptions.Compiled);

    private GrepCommand _grepCommand;
    private CancellationToken _cancellationToken;

    private List<ResultBase> _results;
    private List<Task> _commandTasks;
    private SearchMetrics _searchMetrics;
    private Stopwatch _commandTimer;
    private bool _preProcessComplete;

    private static object _metricsLock = new object();
    private AutoResetEvent _search = new AutoResetEvent(false);
    #endregion Fields..

    #region Properties..
    public int FilesPerTask { get; set; } = 10;
    #endregion Properties..

    #region Constructors..
    public GrepService(PublisherService publisherService, ConsoleService consoleService, FileService fileService)
    {
        _publisherService = publisherService;
        _consoleService = consoleService;
        _fileService = fileService;
    }
    #endregion Constructors..

    #region Methods..
    public void ProcessFiles(List<FileItem> files)
    {
        RegexOptions optionsFlags = CommandFlagUtils.GetRegexOptions(_grepCommand);
        var hashType = CommandFlagUtils.GetHashType(_grepCommand);

        // Build content search pattern
        string searchTerm = _grepCommand.CommandArgs[CommandFlag.SearchTerm];
        string searchPattern = CommandFlagUtils.BuildSearchPattern(_grepCommand);
        Regex searchRegex = new Regex(searchPattern, optionsFlags);

        bool fileNamesOnlyFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.FileNamesOnly);
        bool fileHashesOnlyFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.FileHashes);

        _commandTasks.Add(Task.Run(() =>
        {
            if (fileHashesOnlyFlag)
            {
                bool isValidFileHash = WindowsUtils.IsValidFileHash(searchTerm, hashType);
                if (!isValidFileHash)
                    throw new Exception($"Error: Hash does not match {hashType} format");

                GetFileHashMatches(files, searchTerm, hashType);
            }
            else if (fileNamesOnlyFlag)
                GetFileNameMatches(files, searchPattern, searchRegex);
            else
            {
                if (_grepCommand.CommandArgs[CommandFlag.SearchTerm] == string.Empty)
                    throw new Exception("Error: Missing Search term ");

                GetFileContentMatches(files, searchPattern, searchRegex);
            }

            if (_preProcessComplete && _commandTasks.Count(x => !x.IsCompleted) == 1)
            {
                _commandTimer.Stop();

                PublishFileAccessSummary();
                PublishCommandSummary();

                _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = $"{Environment.NewLine}[{Math.Round((_commandTimer.ElapsedMilliseconds / 1000.0), 2)} second(s)]" });
                _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { Value = Environment.NewLine + Environment.NewLine });

                _search.Set();
            }
        }));
    }

    public void RunCommand(GrepCommand grepCommand, List<ResultBase> results, CancellationToken cancellationToken)
    {
        _grepCommand = grepCommand;
        _results = results; 
        _cancellationToken = cancellationToken;

        _publisherService.RemoveAllSubscribers();
        _publisherService.Subscribe<ConsoleItem>(PublisherMessage.StandardOut, _consoleService.Write);
        _publisherService.Subscribe<List<FileItem>>(PublisherMessage.FileGroupReady, ProcessFiles);

        bool writeFlag = grepCommand.CommandArgs.ContainsKey(CommandFlag.OutFile);
        if (writeFlag)
        {
            _fileService.Initialize(grepCommand.CommandArgs[CommandFlag.OutFile]);
            _publisherService.Subscribe<ConsoleItem>(PublisherMessage.StandardOut, _fileService.Write);
        }

        Initialize();
        _search.WaitOne();
    }

    private void BuildFileContentSearchResults(List<Match> matches, FileItem file, string fileRaw)
    {
        bool ignoreBreaksFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.IgnoreBreaks);
        bool ignoreCaseFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.IgnoreCase);
        bool contextFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.Context);

        // Build file context search pattern
        int contextLength = contextFlag ? Convert.ToInt32(_grepCommand.CommandArgs[CommandFlag.Context]) : 0;

        matches.ForEach(match =>
        {
            if (_cancellationToken.IsCancellationRequested)
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

            lock (_results)
                _results.Add(result);

            result.ToConsoleItemCollection().ForEach(y => _publisherService.Publish(PublisherMessage.StandardOut, y));
        });
    }

    private void GetFiles(List<ResultBase>? previousResults)
    {
        try
        {
            string path = _grepCommand.CommandArgs[CommandFlag.Path];
            bool recursiveFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.Recursive);
            bool maxDepthFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.MaxDepth);
            bool includeHiddenFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.Hidden);
            bool includeSystemFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.System);
            bool fileSizeMinimumFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.FileSizeMinimum);
            bool fileSizeMaximumFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.FileSizeMaximum);

            long fileSizeMin = fileSizeMinimumFlag ? CommandFlagUtils.GetFileSize(_grepCommand.CommandArgs[CommandFlag.FileSizeMinimum]) : -1;
            long fileSizeMax = fileSizeMaximumFlag ? CommandFlagUtils.GetFileSize(_grepCommand.CommandArgs[CommandFlag.FileSizeMaximum]) : -1;

            int maxDepth = maxDepthFlag ? Convert.ToInt32(_grepCommand.CommandArgs[CommandFlag.MaxDepth]) : int.MaxValue;

            var fileGroup = new List<FileItem>();
            IEnumerable<FileItem> files;

            if (previousResults?.Count > 0)
                files = previousResults.Select(result => result.SourceFile).DistinctBy(x => x.Name);
            else
            {
                FileAttributes fileAttributesToSkip = default;
                fileAttributesToSkip |= (includeSystemFlag ? 0 : FileAttributes.System);
                fileAttributesToSkip |= (includeHiddenFlag ? 0 : FileAttributes.Hidden);

                var pathExcludeFilters = CommandFlagUtils.GetPathExcludeFilters(_grepCommand);
                files = WindowsUtils.GetFiles(path, recursiveFlag, maxDepth, fileSizeMin, fileSizeMax, _cancellationToken, pathExcludeFilters, fileAttributesToSkip);
            }

            foreach (var file in files)
            {
                if (fileGroup.Count >= FilesPerTask)
                {
                    _publisherService.Publish(PublisherMessage.FileGroupReady, new List<FileItem>(fileGroup));
                    fileGroup.Clear();
                }

                fileGroup.Add(file);
            }

            if (fileGroup.Count > 0)
                _publisherService.Publish(PublisherMessage.FileGroupReady, new List<FileItem>(fileGroup));
        }
        catch (Exception ex)
        {
            _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = ex.Message });
        }
        finally
        {
            _preProcessComplete = true;
        }
    }

    private void GetFileContentMatches(IEnumerable<FileItem> files, string searchPattern, Regex searchRegex)
    {
        bool deleteFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.Delete);
        bool replaceFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.Replace);

        var filetypeIncludeFilters = CommandFlagUtils.GetFileTypeIncludeFilters(_grepCommand);
        var filetypeExcludeFilters = CommandFlagUtils.GetFileTypeExcludeFilters(_grepCommand);
        var pathIncludeFilters = CommandFlagUtils.GetPathIncludeFilters(_grepCommand);

        foreach (var file in files)
        {
            try
            {
                if (_cancellationToken.IsCancellationRequested)
                    return;

                // Filters
                bool isFiltered = (filetypeIncludeFilters != null && !filetypeIncludeFilters.Contains(Path.GetExtension(file.Name).TrimOnce('.')));
                isFiltered |= (filetypeExcludeFilters != null && filetypeExcludeFilters.Contains(Path.GetExtension(file.Name).TrimOnce('.')));
                isFiltered |= (pathIncludeFilters != null && !pathIncludeFilters.Any(x => Path.GetDirectoryName(file.Name)?.Contains(x.TrimOnce('\'', '"')) ?? false));

                if (isFiltered || file.IsDirectory)
                    continue;

                string fileText = string.Empty;
                
                if (string.Equals(Path.GetExtension(file.Name), ".docx", StringComparison.OrdinalIgnoreCase))
                    fileText = FileUtils.ReadDocX(file.Name);
                else if (string.Equals(Path.GetExtension(file.Name), ".pdf", StringComparison.OrdinalIgnoreCase))
                    fileText = FileUtils.ReadPdf(file.Name);
                else
                    fileText = File.ReadAllText(file.Name);

                List<Match> matches = searchRegex.Matches(fileText).ToList();
                if (matches.Count > 0)
                {
                    if (replaceFlag || deleteFlag)
                        PerformWrites(file, searchPattern, matches.Count, fileText);
                    else
                    {
                        BuildFileContentSearchResults(matches, file, fileText);

                        lock (_metricsLock)
                            _searchMetrics.TotalFilesMatchedCount++;
                    }
                }
            }
            catch
            {
                lock (_metricsLock)
                    _searchMetrics.FailedReadFiles.Add(file);
            }
        }
    }

    private void GetFileHashMatches(IEnumerable<FileItem> files, string searchTerm, HashType hashType)
    {
        var matches = new List<ResultBase>();

        bool deleteFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.Delete);
        var filetypeIncludeFilters = CommandFlagUtils.GetFileTypeIncludeFilters(_grepCommand);
        var filetypeExcludeFilters = CommandFlagUtils.GetFileTypeExcludeFilters(_grepCommand);
        var pathIncludeFilters = CommandFlagUtils.GetPathIncludeFilters(_grepCommand);

        foreach (var file in files)
        {
            try
            {
                if (_cancellationToken.IsCancellationRequested)
                    break;

                // Filters
                bool isFiltered = (filetypeIncludeFilters != null && !filetypeIncludeFilters.Contains(Path.GetExtension(file.Name).TrimOnce('.')));
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
                        Match searchMatch = _anyRegex.Match(fileHash);

                        if (deleteFlag)
                        {
                            PerformWrites(file, searchTerm, 1, file.Name);
                            
                            lock (_metricsLock)
                                _searchMetrics.TotalFilesMatchedCount++;
                        }
                        else
                        {
                            GrepResult result = PerformReads(file, fileNameMatch, searchMatch);
                            if (result != null)
                            {
                                lock (_metricsLock)
                                    _searchMetrics.TotalFilesMatchedCount++;

                                lock (_results)
                                    _results.Add(result);

                                result.ToConsoleItemCollection().ForEach(y => _publisherService.Publish(PublisherMessage.StandardOut, y));
                            }
                        }
                    }
                }
            }
            catch
            {
                lock (_metricsLock)
                    _searchMetrics.FailedReadFiles.Add(file);
            }
        }
    }

    private void GetFileNameMatches(IEnumerable<FileItem> files, string searchPattern, Regex searchRegex)
    {
        bool deleteFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.Delete);
        bool replaceFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.Replace);
        var filetypeIncludeFilters = CommandFlagUtils.GetFileTypeIncludeFilters(_grepCommand);
        var filetypeExcludeFilters = CommandFlagUtils.GetFileTypeExcludeFilters(_grepCommand);
        var pathIncludeFilters = CommandFlagUtils.GetPathIncludeFilters(_grepCommand);

        foreach (var file in files)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;

            // Filters
            bool isFiltered = (filetypeIncludeFilters != null && !filetypeIncludeFilters.Contains(Path.GetExtension(file.Name).TrimOnce('.')));
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
                    if (replaceFlag || deleteFlag)
                    {
                        PerformWrites(file, searchPattern, 1, file.Name);
                        _searchMetrics.TotalFilesMatchedCount++;
                    }
                    else
                    {
                        GrepResult result = PerformReads(file, fileNameMatch, searchMatch);
                        if (result != null)
                        {
                            _searchMetrics.TotalFilesMatchedCount++;

                            lock (_results)
                                _results.Add(result);

                            result.ToConsoleItemCollection().ForEach(y => _publisherService.Publish(PublisherMessage.StandardOut, y));
                        }
                    }
                }
            }
        }
    }

    private void Initialize()
    {
        _preProcessComplete = false;
        _searchMetrics = new SearchMetrics();
        _commandTasks = new List<Task>();
        _commandTimer = Stopwatch.StartNew();

        // Clear the result collection between chained commands so that only the results of the final command are returned
        var previousResults = _results.Count > 0 ? new List<ResultBase>(_results) : null;
        _results.Clear();

        Task.Run(() => GetFiles(previousResults));
    }

    private GrepResult PerformReads(FileItem file, Group fileNameMatch, Match searchMatch)
    {
        GrepResult result = null;

        bool fileHashesFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.FileHashes);

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

    private void PerformWrites(FileItem file, string searchPattern, int fileMatchesCount, string fileRaw)
    {
        bool deleteFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.Delete);
        bool replaceFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.Replace);
        bool fileNamesOnlyFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.FileNamesOnly);

        // FileName
        _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { ForegroundColor = AnsiColors.DarkYellow, Value = $"{file.Name} " });

        try
        {
            if (deleteFlag)
            {
                File.Delete(file.Name);

                _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = $"Deleted" });

                lock (_metricsLock)
                    _searchMetrics.DeleteSuccessCount++;
            }
            else if (replaceFlag)
            {
                if (fileNamesOnlyFlag)
                {
                    string directory = Path.GetDirectoryName(file.Name);
                    string fileName = Path.GetFileName(file.Name);

                    File.Move(file.Name, Path.Combine(directory, Regex.Replace(fileName, searchPattern, _grepCommand.CommandArgs[CommandFlag.Replace])));
                    _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = $"Renamed" });
                }
                else
                {
                    // Unsupported file types for replacement
                    if (string.Equals(Path.GetExtension(file.Name), ".pdf", StringComparison.OrdinalIgnoreCase) || string.Equals(Path.GetExtension(file.Name), ".docx", StringComparison.OrdinalIgnoreCase))
                    {
                        lock (_metricsLock)
                            _searchMetrics.FailedWriteFiles.Add(file);
                    }
                    else 
                    {
                        // Replace all matches in file
                        fileRaw = Regex.Replace(fileRaw, searchPattern, _grepCommand.CommandArgs[CommandFlag.Replace]);
                        File.WriteAllText(file.Name, fileRaw);

                        _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { ForegroundColor = AnsiColors.DarkMagenta, Value = $"{fileMatchesCount} match(es)" });
                    }
                }

                lock (_metricsLock)
                    _searchMetrics.ReplacedSuccessCount += fileMatchesCount;
            }
        }
        catch
        {
            _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { ForegroundColor = AnsiColors.Gray, BackgroundColor = AnsiColors.DarkRedBg, Value = $"Access Denied" });

            lock (_metricsLock)
                _searchMetrics.FailedWriteFiles.Add(file);
        }
        finally
        {
            // Empty buffer
            _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { Value = Environment.NewLine });

            lock (_metricsLock)
                _searchMetrics.TotalFilesMatchedCount++;
        }
    }

    private void PublishCommandSummary()
    {
        bool deleteFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.Delete);
        bool replaceFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.Replace);
        bool fileSizeMinimumFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.FileSizeMinimum);
        bool fileSizeMaximumFlag = _grepCommand.CommandArgs.ContainsKey(CommandFlag.FileSizeMaximum);

        string summary = string.Empty;

        if (deleteFlag)
            summary = $"[{_searchMetrics.DeleteSuccessCount} of {_searchMetrics.TotalFilesMatchedCount} file(s) deleted]";
        else if (replaceFlag)
            summary = $"[{_searchMetrics.ReplacedSuccessCount} occurrence(s) replaced in {_searchMetrics.TotalFilesMatchedCount} file(s)]";
        else
            summary = $"[{_results.Count} result(s) {_searchMetrics.TotalFilesMatchedCount} file(s)]";

        _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = summary });

        if (fileSizeMinimumFlag || fileSizeMaximumFlag)
        {
            var totalFileSize = _results.Sum(x => x.SourceFile.FileSize);
            var fileSizeReduced = WindowsUtils.GetReducedSize(totalFileSize, 3, out FileSizeType fileSizeType);

            summary = $" [{fileSizeReduced} {fileSizeType}(s)]";
            _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = summary });
        }
    }

    private void PublishFileAccessSummary()
    {
        bool verbose = _grepCommand.CommandArgs.ContainsKey(CommandFlag.Verbose);

        if (_searchMetrics.FailedReadFiles.Any() || _searchMetrics.FailedWriteFiles.Any())
        {
            if (_searchMetrics.FailedReadFiles.Any())
            {
                string unreachableFiles = $"[{_searchMetrics.FailedReadFiles.Count} file(s) unreadable/inaccessible]";
                _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = unreachableFiles });

                if (verbose)
                    _searchMetrics.FailedReadFiles.ForEach(x => _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = $"{x.Name}{Environment.NewLine}" }));
            }

            if (_searchMetrics.FailedWriteFiles.Any())
            {
                string unwriteableFiles = $"[{_searchMetrics.FailedWriteFiles.Count} file(s) could not be modified]{Environment.NewLine}";
                _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = unwriteableFiles });

                if (verbose)
                    _searchMetrics.FailedWriteFiles.ForEach(x => _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { ForegroundColor = AnsiColors.Red, Value = $"{x.Name}{Environment.NewLine}" }));
            }

            _publisherService.Publish(PublisherMessage.StandardOut, new ConsoleItem { Value = Environment.NewLine });
        }
    }
    #endregion Methods..
}
