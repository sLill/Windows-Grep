<p align="center">
    <img src="https://i.imgur.com/15SNWH7.png" alt="Windows Grep logo" width="200" height="200">
</p>

<h1>Windows Grep</h1> 

[![CodeFactor](https://www.codefactor.io/repository/github/slill/windows-grep/badge)](https://www.codefactor.io/repository/github/slill/windows-grep)
![.NET Core](https://github.com/sLill/Windows-Grep/workflows/.NET/badge.svg)

Command line grep-like file search utility for Windows. 

Built to mimic the command style and behavior of Unix grep.
No bloated gui or custom shell. Just a fast, simple tool that runs in native cmd/powershell.</br>

<h4>Features</h4>

- Search by file name, file content, or even file hash
- Search non-standard filetypes (pdf, docx, etc)
- Mass edit files
- Filter by file type, directory or size
- Colored output, even in non-interactive shells 
- And more

<h2>Installation Methods</h2>

<h4>Installer</h4>

1. Visit the release tab (https://github.com/sLill/Windows-Grep/releases)
2. Download and run WindowsGrepSetup.msi. This will:
    - Install Windows Grep in Program Files (x86)
    - Add "grep" to PATH for command line use
    - Insert a registry key for context menu use within File Explorer

<h4>Standalone</h4>

1. Visit the release tab (https://github.com/sLill/Windows-Grep/releases)
2. Download one of the pre-compiled binaries

<h2>Usage</h2>

File Explorer > Right-Click > Windows Grep
<br/><b>or</b><br/>
cmd/powershell

<h4>grep [options] search_term [path]</h4>
<img width="582" height="92" alt="Snag_18400c60" src="https://github.com/user-attachments/assets/dea61c03-4ed0-408b-9b12-307b18308969" />

<h2>Reference</h2>

| Flag                          |    | Example                      |
| ------------------------- | -- | ------------------- |
| -h, --help | Show Help         |  | 
| -r         | Recursive Search  | `grep -r dug C:/` |
| -i         | Ignore Case       | `grep -c 20 dug C:/` | 
| -c         |  Show n Characters Around Match | `grep -i dug C:/` |
| -F         | Plain Text Search | `grep -F dug C:/MyFile.txt` |
| -t | Include files by Type(s) | `grep -t .txt,.js dug C:/` |
| -T | Exclude files by Type(s) | `grep -T .css,.git dug C:/inetpub` |
| -p | Include Filepaths by Expression | `grep -p AppData,Desktop dug C:/Users` | 
| -P | Exclude Filepaths by Expression | `grep -P Windows,Users dug C:/` |
| -k         | Filenames Only | `grep -k 'Log.*' C:/` |
| -o         | Redirect Output to File | `grep -o output.txt dug .` |
| -v         | Verbose Output | `grep -v dug ../` |
| --max-depth=    | Max Depth | `grep --max-depth=3 dug C:/` |
| --include-hidden   | Include Hidden Files | `grep --include-hidden dug C:/` |
| --include-system   | Include System Files | `grep --include-system dug C:/` |
| --filesize-min= | Minimum File Size (kb,mb,gb,tb)  | `grep --filesize-min=30mb dug C:/` |
| --filesize-max= | Maximum File Size (kb,mb,gb,tb)  | `grep --filesize-max=3gb dug C:/` |
| --ignore-breaks | Ignore Line Breaks | `grep --ignore-breaks dug C:/` |
| --hash= | Match by File Hash (0=SHA256, 1=MD5) | `grep --hash=0 74184D0\w+ C:/` |
| --replace= | Replace Text       | `grep --replace=dig dug C:/` |
| --delete   | Delete Files         | `grep --delete dug C:/` | 
