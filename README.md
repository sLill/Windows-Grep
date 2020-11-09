<h1>Windows Grep</h1> 

[![CodeFactor](https://www.codefactor.io/repository/github/slill/windows-budgetgrep/badge)](https://www.codefactor.io/repository/github/slill/windows-budgetgrep)
![.NET Core](https://github.com/sLill/Windows-BudgetGrep/workflows/.NET%20Core/badge.svg)

A file search utility. Performs as well or better than paid applications like FileLocator Pro without any additional UI overhead

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


<i>* See <a href="https://github.com/sLill/Windows-BudgetGrep/wiki/WindowsGrep.CommandFlags">documentation</a> for detailed command descriptions </i>

<b>Run WindowsGrep from Cmd/Powershell or Windows Explorer</b></br>
Running from Windows Explorer opens WindowsGrep with the current directory as the root 

<b>Command Order</b></br>
Order of flags and search terms is flexible. The only requirement is that flags that expect parameters be grouped with their respective parameter value
<br/><i>ex. &nbsp;&nbsp; -f 'MyFile.txt' &nbsp; or &nbsp; --file='MyFile.txt'</i>

<b>Chained Commands</b></br>
Like Unix grep, commands are chainable and delimited with a bar</br>
<img src="https://i.imgur.com/pjhqRBi.png"> 

<b>Regular Expressions</b></br>
WindowsGrep is configured to search using regular expressions by default (-G). Add the (-F) flag to queries that should be interpreted literally. The default setting for this can be modified in the application configuration file. 

<b>Writing Output to File</b></br>
The default output format when writing (-w) to a file is space delimited. Format as a list of Comma-Separated Values by saving with a .csv file extension

<h2>== EXAMPLES ==</h2>

#### 1. Searching for the grep.exe executable</br>
Recursive(-r) to include all sub-directories. Filename(-k) to target filenames rather than file content</br>
<img src="https://i.imgur.com/scPmoNa.png" height="204" width="495"></br>
<ul>
  <li><b>WindowsGrep - 11.94 seconds</b></li>
<li>FileLocator Pro - 14.8 seconds</li>
<li>Windows Explorer - 1 minute 39 seconds</li>
</ul>

The same query with additional filtering (-t) for .exe files to further improve performance</br>
<img src="https://i.imgur.com/PeC2mma.png" height="204" width="495"></br></br>

#### 2. Searching for the phrase "slow green turtle"</br>
Recursive(-r) to include all sub-directories. Filter by .txt filetype(-t). Ignore case(-i)</br>
<img src="https://i.imgur.com/4QqWzb3.png"></br>
<ul>
  <li><b>WindowsGrep - 0.93 seconds</b></li>
<li>FileLocator Pro - 1.87 seconds</li>
<li>Windows Explorer - N/A</li>
</ul>
