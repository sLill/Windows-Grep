<h1>Windows Grep</h1> 

[![CodeFactor](https://www.codefactor.io/repository/github/slill/windows-grep/badge)](https://www.codefactor.io/repository/github/slill/windows-grep)
![.NET Core](https://github.com/sLill/Windows-Grep/workflows/.NET%20Core/badge.svg)

Command line grep-like file search utility for Windows</br>

-Supports-</br>
- Filename and file content searches</br>
- Regular Expressions</br>
- Chained commands</br>
- Filetype/Filepath/Filesize filtering</br>
- Output to external files</br>
- Mass replace and delete actions on queried files</br>
- Ease of use: Runnable from Cmd, Powershell, Windows Explorer or headless</br>

<h2>== INSTALLATION ==</h2>

1. Visit the release tab (https://github.com/sLill/Windows-Grep/releases)
2. Download and run WindowsGrepSetup.msi (This is the only file you need)

This will install Windows Grep in ProgramFilesx86, add "grep" to your system's PATH for command line use, and insert registry keys for context menu use within Windows Explorer directories.

<h2>== USAGE ==</h2>
Right-click in File Explorer > Windows Grep
<br/><br/>
OR
<br/><br/>
Open cmd > "grep [command]"<br/>

<h2>== REFERENCE ==</h2>

|                           |    |                       |
| ------------------------- | -- | :-------------------: |
| Recursive Search          | -r | --recursive           |
| Target Directory          | -d | --directory=          |
| Context Characters        | -c | --context=            |
| Limit n Results           | -n | --results=            |
| Suppress output           | -s | --suppress            |
| Ignore Line Breaks        | -b | --ignore-breaks       |
| Ignore Case               | -i | --ignore-case         |
| Target File               | -f | --file=               |
| Plain Text Search         | -F | --fixed-strings       |
| Regular Expression Search | -G | --basic-regexp        |
| Filter Files by Type(s) [Inclusive]    | -t | --filetype-filter=    |
| Filter Files by Type(s) [Exclusive]    | -T | --filetype-exclude-filter= |
| Filter Filepath(s) by Expression(s) [Inclusive]    | -p | --path-filter=    |
| Filter Filepath(s) by Expression(s) [Exclusive]    | -P | --path-exclude-filter= |
| Filenames Only            | -k | --filenames-only      |
| FileSize Minimum          | -z | --filesize-minimum=   |
| FileSize Maximum          | -Z | --filesize-maximum=   |
| Write Output to File      | -w | --write=              |
| Replace Text              | -RX| --replace=            |
| Delete Files              | -DX| --delete-files        |
| File Hashes               |    | --hash=  (0=SHA256, 1=MD5)     |


<i>* See <a href="https://github.com/sLill/Windows-Grep/wiki/WindowsGrep.CommandFlags">documentation</a> for detailed command descriptions </i>

<b>Command Order</b></br>
Order of flags and search terms is completely flexible. The only requirement is that parameters be given to flag types that expect them.
<br/><i>ex. &nbsp;&nbsp; -f 'MyFile.txt' &nbsp; or &nbsp; --file='MyFile.txt'</i>

<br/>

<h2>== EXAMPLE COMMANDS ==</h2>

<i>Recursively search all files for "Dug"</i><br/>
-r Dug
<br/><br/>

<i>Recursively search all files for "Dug" or "Dig". Ignore-case</i><br/>
-r -i D[ui]g
<br/><br/>

<i>Recursively search all text and csharp files for "Dug". Filter out matches that appear in the bin or obj subdirectory</i><br/>
-r Dug -t .txt;.cs -P bin;obj
<br/><br/>

<i>Recursively search all files for "Dug" and show 100 characters of text around the match for context</i><br/>
-r Dug -c 100
<br/><br/>

<i>Search for all filenames in the current directory containing "Dug", and then search that subset of files for "Mike"</i><br/>
-r Dug -k | Mike
<br/><br/>

<i>Search all files in the current directory for text that matches a phone number expression/pattern</i><br/>
[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}
<br/><br/>

<i>Recursively search all files for US zip codes. Filter out .dll matches. Limit the results 10. Write the output to a .csv</i><br/>
-r \d{5}(-\d{4})? -T .dll -n 10 -w 'C:\output.csv'
<br/><br/>


<b>For even more examples and detailed descriptions of each flag, visit the</b> <a href="https://github.com/sLill/Windows-Grep/wiki/WindowsGrep.CommandFlags">wiki</a>
