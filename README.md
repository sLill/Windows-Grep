<h1>Windows Grep</h1>

Jokes about the Windows file system aside, this is a useful tool that performs as well as paid applications like FileLocator Pro without any additional UI overhead.  

<h2>TO INSTALL</h2>

1. Visit the release tab (https://github.com/sLill/Windows-BudgetGrep/releases)
2. Download WindowsGrepSetup.msi (This is the only file you need)
2. Run WindowsGrepSetup.msi

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

<u>USAGE</u>
<h4>WindowsGrep can be run from cmd, Powershell or by selecting it from the Windows File Explorer context menu</h4>
<ul><li>Doing so from Windows Explorer uses the current directory as the root</li></ul> 

<span style="text-decoration: underline">Cmd/Powershell Examples</span> <br/>

<h4>Recursively search through all directories/sub-directories for a txt file containing the term "foo"</h4>
<ul><li>grep -d 'C:\Program Files (x86)\' -r -t .txt foo</li></ul>

<h4>Windows Explorer Examples</h4>

