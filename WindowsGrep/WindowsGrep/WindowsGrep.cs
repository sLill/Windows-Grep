namespace WindowsGrep
{
    public class WindowsGrep
    {
        #region Fields..
        private NativeService _nativeService;
        private GrepService _grepService;
        #endregion Fields..

        #region Constructors..
        public WindowsGrep(NativeService nativeService, GrepService grepService)
        {
            _nativeService = nativeService;
            _grepService = grepService;
        }
        #endregion Constructors..

        #region Methods..
        public void RunCommand(string commandRaw, CommandResultCollection commandResultCollection, CancellationToken cancellationToken)
        {
            string splitPattern = @"\|(?![^{]*}|[^\(]*\)|[^\[]*\])";
            string[] commandCollection = Regex.Split(commandRaw, splitPattern);

            foreach (string command in commandCollection)
            {
                // Native commands
                var nativeCommandArgs = ConsoleUtils.ParseNativeCommandArgs(command);
                if (nativeCommandArgs != default)
                {
                    var nativeCommand = new NativeCommand() { CommandType = nativeCommandArgs.CommandType.Value, CommandParameter = nativeCommandArgs.CommandParameter };
                    _nativeService.BeginProcessNativeCommand(nativeCommand, commandResultCollection, cancellationToken);
                }

                // Grep commands
                else
                {
                    var grepCommandArgs = ConsoleUtils.ParseGrepCommandArgs(command);

                    GrepCommand grepCommand = default;

                    // Help
                    if (grepCommandArgs.ContainsKey(ConsoleFlag.Help))
                        grepCommand = new GrepCommand(GrepCommandType.Help);
                    else if (grepCommandArgs.ContainsKey(ConsoleFlag.Help_Full))
                        grepCommand = new GrepCommand(GrepCommandType.Help_Full);

                    // Query
                    else
                        grepCommand = new GrepCommand(GrepCommandType.Query) { CommandArgs = grepCommandArgs };

                    _grepService.BeginProcessGrepCommand(grepCommand, commandResultCollection, cancellationToken);
                }
            }
        }
        #endregion Methods..
    }
}
