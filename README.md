<h1>Windows Grep</h1>

Performs as well or better than paid applications like FileLocator Pro without any additional UI overhead.  

<i>*Be cautious when using command flags that modify files like Replace (-R) and Delete (-D). There is no confirmation on these actions.</i>

<h2>TO INSTALL</h2>

1. Visit the release tab (https://github.com/sLill/Windows-BudgetGrep/releases)
2. Download and run WindowsGrepSetup.msi (This is the only file you need)

This will install WindowsGrep in ProgramFilesx86, add "grep" to your system's PATH for command line use, and insert registry keys for context menu use within Windows Explorer directories.

<h2>REFERENCE</h2>

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
| Search by Filename        | -k | --filenames-only      |
| Replace Text              | -R | --replace=            |
| Delete Files              | -D | --delete-files        |
| Write Output to File      | -w | --write=              |

<br/>

<h2>USAGE</h2>
<h3>WindowsGrep can be run from cmd, Powershell or by selecting it from the Windows File Explorer context menu</h4>
<ul><li>Executing from Windows Explorer runs WindowsGrep with the current directory as the root</li></ul> 

<h3>Flexible Command Order</h3>
Order of flags and search terms does not matter when building a query. The only requirement is that flags that expect parameters be grouped with their respective parameter value
<br/><i>ex. &nbsp;&nbsp; -f 'MyFile.txt' &nbsp; or &nbsp; --file='MyFile.txt'</i>

<h3>Chained Commands</h3>
Like Unix grep, commands are chainable when delimited by a bar
<img src="https://i.imgur.com/pjhqRBi.png"> 
