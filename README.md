<p align="center">
    <img src="https://i.imgur.com/15SNWH7.png" alt="Windows Grep logo" width="200" height="200">
</p>

<h1>Windows Grep</h1> 

[![CodeFactor](https://www.codefactor.io/repository/github/slill/windows-grep/badge)](https://www.codefactor.io/repository/github/slill/windows-grep)
![.NET Core](https://github.com/sLill/Windows-Grep/workflows/.NET/badge.svg)

Command line grep-like file search utility for Windows</br>

<h2>Supports</h2>
- Basic and advanced file searches</br>
- Regular Expressions</br>
- Chained commands</br>
- Filetype/Filepath/Filesize filtering</br>
- Exports</br>
- Mass replace and delete</br>
- Ease of use: Runs from cmd, Powershell, Windows Explorer or headless</br>

<h1>INSTALLATION</h1>

1. Visit the release tab (https://github.com/sLill/Windows-Grep/releases)
2. Download and run WindowsGrepSetup.msi (This is the only file you need)

This will install Windows Grep in ProgramFilesx86, add "grep" to your system's PATH for command line use, and insert registry keys for context menu use within Windows Explorer directories.

<h1>USAGE</h1>
Right-click in File Explorer > Windows Grep
<br/><br/>
OR
<br/><br/>
Open cmd > "grep [command]"<br/>

<h1>REFERENCE</h1>

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
Order of flags and the search term is completely flexible

<br/>

<h1>EXAMPLE COMMANDS</h1>

<i>Recursive search.</i><br/>
-r Dug
<br/><br/>

<i>Recursive search. ignore-case.</i><br/>
-r -i D[ui]g
<br/><br/>

<i>Recursive search. txt and cs files only. Filter out bin and obj matches</i><br/>
-r Dug -t .txt;.cs -P bin;obj
<br/><br/>

<i>Recursive search. show 100 characters around the match.</i><br/>
-r Dug -c 100
<br/><br/>

<i>Recursive search. Filenames only. Additional search on results from first command for "Mike"</i><br/>
-r Dug -k | Mike
<br/><br/>

<i>Match phone number</i><br/>
[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}
<br/><br/>

<i>Recursive search. US zipcode expression. Filter out .dll matches. Limit results 10. Write output to .csv</i><br/>
-r \d{5}(-\d{4})? -T .dll -n 10 -w 'C:\output.csv'
<br/><br/>


<b>For even more examples and detailed descriptions of each flag, visit the</b> <a href="https://github.com/sLill/Windows-Grep/wiki/WindowsGrep.CommandFlags">wiki</a>
