<h1>Windows Grep</h1>

Performs as well or better than paid applications like FileLocator Pro without any additional UI overhead.  

<i>*Be cautious when using command flags that modify files like Replace (-R) and Delete (-D). There is no confirmation on these actions.</i>

<h2>== INSTALLATION ==</h2>

1. Visit the release tab (https://github.com/sLill/Windows-BudgetGrep/releases)
2. Download and run WindowsGrepSetup.msi (This is the only file you need)

This will install WindowsGrep in ProgramFilesx86, add "grep" to your system's PATH for command line use, and insert registry keys for context menu use within Windows Explorer directories.

<h2>== USAGE ==</h2>

|                           |    |                       |
| ------------------------- | -- | :-------------------: |
| Recursive Search          | -r | --recursive           |
| Target Directory          | -d | --directory=          |
| Context Characters        | -c | --context=            |
| Ignore Line Breaks        | -b | --ignore-breaks       |
| Ignore Case               | -i | --ignore-case         |
| Target File               | -f | --file=               |
| Plain Text Search         | -F | --fixed-strings       |
| Regular Expression Search | -G | --basic-regexp        |
| Filter on FileType(s)     | -t | --filetype-inclusion= |
| Filter out FileType(s)    | -T | --filetype-exclusion= |
| Filenames Only            | -k | --filenames-only      |
| Replace Text              | -R | --replace=            |
| Delete Files              | -D | --delete-files        |
| Write Output to File      | -w | --write=              |

<br/>

<i>* See <a href="https://github.com/sLill/Windows-BudgetGrep/wiki/WindowsGrep.References.CommandFlags">documentation</a> for detailed command descriptions

</br>

<h4>Run from Cmd/Powershell or Windows Explorer</h4>
Running from Windows Explorer opens WindowsGrep with the current directory as the root 

</br>

<h4>Command Order</h4>
Ordering of flags and search terms is flexible. The only requirement is that flags that expect parameters be grouped with their respective parameter value
<br/><i>ex. &nbsp;&nbsp; -f 'MyFile.txt' &nbsp; or &nbsp; --file='MyFile.txt'</i>

</br>

<h4>Chained Commands</h4>
Like Unix grep, commands are chainable and delimited with a bar
<img src="https://i.imgur.com/pjhqRBi.png"> 

</br>

<h4>Regular Expressions</h4>
WindowsGrep is configured to search using regular expressions by default (-G). Add the (-F) flag to queries that should be interpreted literally. The default setting for this can be modified in the application configuration file. 

</br>

<h4>Writing Output to File</h4>
The default output format when writing (-w) to a file is space delimited. Can be formatted as a list of Comma-Separated Values by saving with a .csv file extension
