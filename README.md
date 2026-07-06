<p align="center">
    <img src="https://i.imgur.com/15SNWH7.png" alt="Windows Grep logo" width="180" height="180">
</p>

<h1 align="center">Windows Grep</h1>

<p align="center">A fast, grep-style file search utility for Windows.</p>

<p align="center">
    <a href="https://www.codefactor.io/repository/github/slill/windows-grep"><img src="https://www.codefactor.io/repository/github/slill/windows-grep/badge" alt="CodeFactor"></a>
    <a href="https://github.com/sLill/Windows-Grep/actions/workflows/rust.yml"><img src="https://github.com/sLill/Windows-Grep/actions/workflows/rust.yml/badge.svg" alt="Build"></a>
    <a href="https://github.com/sLill/Windows-Grep/releases"><img src="https://img.shields.io/github/v/release/sLill/Windows-Grep" alt="Release"></a>
    <a href="LICENSE"><img src="https://img.shields.io/badge/license-MIT-blue.svg" alt="License: MIT"></a>
</p>

Windows Grep mirrors the command style and behavior of Unix `grep`. There's no GUI to learn or shell to install - run it as a one-shot
command in cmd/PowerShell, right-click a folder in File Explorer, or drop into its optional
interactive shell.

## Features

- **Search contents, names, or hashes** - match file text, file names (`-k`), or an exact SHA-256/MD5 hash (`--hash`)
- **Search non-plaintext filetypes** - extracts and searches text inside of all standard filetypes and extended filetypes (PDF, DOCX, XLSX, etc) 
- **Mass edit** - find-and-replace or delete across thousands of files in a single command
- **Rich filtering** - by file type, path expression, size, recursion depth, and hidden/system attributes
- **Interactive shell** - optional REPL with built-in `cd`, `ls`, `pwd`, and `clear`
- **Colored output**, even in remote shells

## Installation

### Installer

1. Go to the [Releases](https://github.com/sLill/Windows-Grep/releases) page.
2. Download and run the setup executable (`x64` or `arm64`). This will:
    - Install Windows Grep
    - Add `grep` to the system `PATH` for command-line use
    - Add **Windows Grep** to the File Explorer context menu

### Standalone

1. Go to the [Releases](https://github.com/sLill/Windows-Grep/releases) page.
2. Download a pre-compiled binary and run it directly.

### Build from source

Requires the [Rust toolchain](https://rustup.rs/).

```sh
git clone https://github.com/sLill/Windows-Grep
cd Windows-Grep/WindowsGrep
cargo build --release
# Binary: target/release/grep.exe
```

## Usage

```
grep [options] search_term [path]
```

The `search_term` is treated as a regular expression unless `-F` is used. `path` is optional and
defaults to the current directory. Searches are non-recursive unless `-r` is supplied.

Launch from **File Explorer → Right-Click → Windows Grep**, or from cmd/PowerShell:

<img width="605" alt="Windows Grep output example" src="https://github.com/user-attachments/assets/92d7c76e-7b64-4fed-a407-b8d6c6e1daaf" />

## Examples

```sh
# Search the current directory for the term "error"
grep error .

# Recursive, case-insensitive search from C:\
grep -r -i error C:/

# Show 30 characters of context around each match
grep -c 30 TODO .

# Filter for .js and .ts filetypes only
grep -t .js,.ts useEffect C:/app

# Skip node_modules and .git directories
grep -P node_modules,.git useEffect C:/app 

# Match by file name instead of contents
grep -k "report.pdf" C:/Users

# Match a file by its exact SHA-256 hash
grep --hash=0 e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855 C:/

# Find and replace text across many files at once
grep --replace=newName oldName C:/app

# Delete every file that contains the term "dug" (use with care)
grep --delete dug C:/temp
```

## Reference

### Options

| Flag | Description | Example |
| --- | --- | --- |
| `-h`, `--help` | Show help (`--help` lists every option) | `grep --help` |
| `-r` | Search subdirectories recursively | `grep -r dug C:/` |
| `-i` | Ignore case | `grep -i dug C:/` |
| `-c <n>` | Print *n* characters of context around each match | `grep -c 20 dug C:/` |
| `-F` | Treat the search term as plain text (no regex) | `grep -F "v1.2.0" .` |
| `-t <types>` | Include only these file types (`,` or `;` delimited) | `grep -t .txt,.js dug C:/` |
| `-T <types>` | Exclude these file types (`,` or `;` delimited) | `grep -T .png,.jpg dug C:/` |
| `-p <exprs>` | Include only paths matching these expressions | `grep -p AppData,Desktop dug C:/Users` |
| `-P <exprs>` | Exclude paths matching these expressions | `grep -P Windows,Users dug C:/` |
| `-k` | Match file names instead of contents | `grep -k "Log.*" C:/` |
| `-o <file>` | Mirror output to a file | `grep -o results.txt dug .` |
| `-v` | Verbose — also list files that couldn't be read or modified | `grep -v dug ../` |

### Long options

| Flag | Description | Example |
| --- | --- | --- |
| `--hidden` | Include hidden files | `grep --hidden dug C:/` |
| `--system` | Include system files | `grep --system dug C:/` |
| `--max-depth=<n>` | Limit recursion depth (use with `-r`) | `grep -r --max-depth=3 dug C:/` |
| `--filesize-min=<n>` | Skip files smaller than *n* (`kb`/`mb`/`gb`/`tb`) | `grep --filesize-min=30mb dug C:/` |
| `--filesize-max=<n>` | Skip files larger than *n* (`kb`/`mb`/`gb`/`tb`) | `grep --filesize-max=3gb dug C:/` |
| `--ignore-breaks` | Allow matches to span line breaks | `grep --ignore-breaks dug C:/` |
| `--hash=<algo>` | Match by exact file hash — `0`=SHA-256, `1`=MD5 | `grep --hash=0 <hash> C:/` |
| `--replace=<text>` | Replace matches in file contents (or rename files with `-k`) | `grep --replace=dig dug C:/` |
| `--delete` | Delete matched files | `grep --delete dug C:/` |

### Shell commands

Available inside the interactive shell.

| Command | Description |
| --- | --- |
| `cd <path>` | Change the working directory |
| `ls` | List the working directory |
| `pwd` | Print the working directory |
| `clear` | Clear the screen |
| `exit`, `quit` | Leave the shell |

## License

[MIT](LICENSE) © Samuel Turner-Lill
