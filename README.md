<p align="center">
    <img src="https://i.imgur.com/15SNWH7.png" alt="Windows Grep logo" width="200" height="200">
</p>

<h1>Windows Grep</h1> 

[![CodeFactor](https://www.codefactor.io/repository/github/slill/windows-grep/badge)](https://www.codefactor.io/repository/github/slill/windows-grep)
![.NET Core](https://github.com/sLill/Windows-Grep/workflows/.NET/badge.svg)

Command line grep-like file search utility for Windows. 

Built to mimic the command style and behavior of Unix grep for those looking for familiarity.
No bloated gui or custom shell. Just a fast, simple tool that runs in native cmd/powershell.</br>

<h2>Supports</h2>

- Most Unix grep options
- Regular Expressions
- Chaining commands
- Filtering by filetypes, directory, and filesize
- Mass editing with replace and delete
- Ease of use. Runs from cmd, Powershell and Windows Explorer

<h2>INSTALLATION</h2>

<h3>Installer</h2>

1. Visit the release tab (https://github.com/sLill/Windows-Grep/releases)
2. Download and run WindowsGrepSetup.msi (This is the only file you need)

This will install Windows Grep in Program Files (x86), add "grep" to PATH for command line use, and insert a registry key for context menu use within Windows Explorer.

<h3>Manual Compilation</h3>

1. Clone repo and build the `WindowsGrep` project as Release
2. Publish `WindowsGrep`
3. Download [Wix Toolset & Wix VS Extension](https://wixtoolset.org/docs/wix3/)
4. Build the `WindowsGrep.Setup` project to generate a msi installer in the `WindowsGrep.Setup` bin

<h2>USAGE</h2>
<h4>grep [options] search_term [path]</h4><br/>

Right-click in File Explorer > Windows Grep
<br/><b>or</b><br/>
Open cmd/powershell > "grep [command]" <br/> <br/>

![image](https://github.com/user-attachments/assets/6798a573-43db-4012-a4d0-04ff76e9ae3a)


<h2>REFERENCE</h2>

|                           |    |                       |
| ------------------------- | -- | :-------------------: |
| Show Help                 | -h | --help                |
| Recursive Search          | -r | --recursive           |
| Ignore Case               | -i | --ignore-case         |
| Show n Characters Around Match | -c | --context=            |
| Ignore Line Breaks        | -b | --ignore-breaks       |
| Plain Text Search         | -F | --fixed-strings       |
| Filenames Only            | -k | --filenames           |
| Redirect Output to File   | -o | --out-file=           |
| Match by File Hash        |    | --hash=  (0=SHA256, 1=MD5)     |
| Max Depth                 |    | --max-depth=          |
| Show Hidden Files         |    | --show-hidden         |
| Show System Files         |    | --show-system         |
| Filter Files by Type(s) [Inclusive]    | -t | --filetype-include=	|
| Filter Files by Type(s) [Exclusive]    | -T | --filetype-exclude= |
| Filter Filepath(s) by Expression(s) [Inclusive]    | -p | --path-include=    |
| Filter Filepath(s) by Expression(s) [Exclusive]    | -P | --path-exclude=    |
| Minimum File Size           |    | --filesize-min=   |
| Maximum File Size           |    | --filesize-max=   |
| Replace Text                | -RX| --replace=        |
| Delete Files                | -DX| --delete          |


<i>* See <a href="https://github.com/sLill/Windows-Grep/wiki/WindowsGrep.CommandFlags">documentation</a> for detailed command descriptions </i>
<br/>

<h2>EXAMPLE COMMANDS</h2>

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
