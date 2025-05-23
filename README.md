<p align="center">
    <img src="https://i.imgur.com/15SNWH7.png" alt="Windows Grep logo" width="200" height="200">
</p>

<h1>Windows Grep</h1> 

[![CodeFactor](https://www.codefactor.io/repository/github/slill/windows-grep/badge)](https://www.codefactor.io/repository/github/slill/windows-grep)
![.NET Core](https://github.com/sLill/Windows-Grep/workflows/.NET/badge.svg)

Command line grep-like file search utility for Windows. 

No bloated gui or custom console. Just a fast, simple tool that runs in native cmd/powershell. </br>

<h2>Supports</h2>
- Regular Expressions</br>
- Chained commands</br>
- Filter by Filetype/Filepath/Filesize</br>
- Exports</br>
- Mass replace/delete</br>
- Ease of use: Run from cmd, Powershell and Windows Explorer</br>

<h1>INSTALLATION</h1>

<h3>Installer</h2>

1. Visit the release tab (https://github.com/sLill/Windows-Grep/releases)
2. Download and run WindowsGrepSetup.msi (This is the only file you need)

This will install Windows Grep in Program Files (x86), add "grep" to PATH for command line use, and insert a registry key for context menu use within Windows Explorer.

<h3>Manual Compilation</h2>

1. Clone repo and build the `WindowsGrep` project as Release
2. Publish `WindowsGrep`
3. Download [Wix Toolset & Wix VS Extension](https://wixtoolset.org/docs/wix3/)
4. Build the `WindowsGrep.Setup` project to generate a msi installer in the `WindowsGrep.Setup` bin

<h1>USAGE</h1>
Right-click in File Explorer > Windows Grep
<br/><br/>
OR
<br/><br/>
Open cmd/powershell > "grep [command]" <br/> <br/>

![image](https://github.com/user-attachments/assets/6798a573-43db-4012-a4d0-04ff76e9ae3a)


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
| Delete Files              | -DX| --delete              |
| File Hashes               |    | --hash=  (0=SHA256, 1=MD5)     |
| Max Depth                 |    | --max-depth=          |
| Show Hidden Files         |    | --show-hidden         |
| Show System Files         |    | --show-system         |  


<i>* See <a href="https://github.com/sLill/Windows-Grep/wiki/WindowsGrep.CommandFlags">documentation</a> for detailed command descriptions </i>

<b>Command Order</b></br>
Order of flags and the search term is completely flexible

<br/>

<h1>EXAMPLE COMMANDS</h1>

<i>Recursive search for file content containing "Dug".</i><br/>
-r Dug
<br/><br/>

<i>Recursive search for file names containing "Dug".</i><br/>
-r -k Dug
<br/><br/>

<i>Search for "Dug" or "Dig". Ignore-case.</i><br/>
-i D[ui]g
<br/><br/>

<i>Filter for .txt and .cs files only. Additionally, filter out files in bin and obj subdirectories</i><br/>
Dug -t .txt;.cs -P bin;obj
<br/><br/>

<i>Show 100 characters of context around the match.</i><br/>
Dug -c 100
<br/><br/>

<i>Search for filenames containing "Dug". Additionally, search the files returned from command one for content containing "mike"</i><br/>
-r Dug -k | -i mike
<br/><br/>

<i>Recursive search for US zipcodes. Filter out .dll's, limit results 10 and write output to a .csv</i><br/>
-r \d{5}(-\d{4})? -T .dll -n 10 -w 'C:\output.csv'
<br/><br/>


<b>For even more examples and detailed descriptions of each flag, visit the</b> <a href="https://github.com/sLill/Windows-Grep/wiki/WindowsGrep.CommandFlags">wiki</a>
