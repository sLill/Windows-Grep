namespace WindowsGrep
{
    public static class WindowsGrep
    {
        #region Methods..
        public static void RunCommand(string commandRaw, CommandResultCollection commandResultCollection, CancellationToken cancellationToken)
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
                    NativeService.BeginProcessNativeCommand(nativeCommand, commandResultCollection, cancellationToken);
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

                    GrepService.BeginProcessGrepCommand(grepCommand, commandResultCollection, cancellationToken);
                }
            }
        }
        #endregion Methods..
    }
}
