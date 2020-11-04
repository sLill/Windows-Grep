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

<i>* See <a href="https://github.com/sLill/Windows-BudgetGrep#reference">documentation</a> for detailed command descriptions

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

<h2>== REFERENCE ==</h2>

<b>Recursive Search &nbsp; (-f &nbsp; --recursive)</b></br>
Changes the scope of the command from "top level directory only" to "top level directory <b>and all sub-directories</b>"

<br/>

<b>Target Directory &nbsp; (-d &nbsp; --directory=)</b></br>
Sets the root directory for the command. Take directory as a parameter (Encapsulate in single-quotes '' or double-quotes "")<br/>
<i>ex. &nbsp;&nbsp; -d 'C:\Users' &nbsp; or &nbsp; --directory='C:\Users'</i>

<br/>

<b>Context Characters &nbsp; (-c &nbsp; --context=)</b></br>
Sets the number of surrounding characters to be returned with each query result. Takes an integer as a parameter<br/>
<i>ex. &nbsp;&nbsp; -c 50 &nbsp; or &nbsp; --context=50</i>

<br/>

<b>Ignore Line Breaks &nbsp; (-b &nbsp; --ignore-breaks)</b></br>
Ignores new line characters (\r\n) in queried files 

<br/>

<b>Ignore Case &nbsp; (-i &nbsp; --ignore-case)</b></br>
Ignores character-case mistmatches between the users' search term and queried file content

<br/>

<b>Target File &nbsp; (-f &nbsp; --file=)</b></br>
Sets the scope of the query to a single file. Takes filepath as a parameter (Encapsulate in single-quotes '' or double-quotes "")<br/>
<i>ex. &nbsp;&nbsp; -f 'MyFile.txt' &nbsp; or &nbsp; --file='MyFile.txt'</i>

<br/>

<b>Plain Text Search &nbsp; (-F &nbsp; --fixed-strings)</b></br>
Configures the query to interpret the users' search term literally <br/>
<i>ex. How is Hannah? <b>matches</b> How is Hannah?</i><br/>
<i>ex. How is Hannah? <b>does not match</b> How is Hanna</i>

<br/>

<b>Regular Expression Search &nbsp; (-G &nbsp; --basic-regexp)</b></br>
Configures the query to interpret the users' search term as a regular expression (WindowsGrep default search style)<br/>
<i>ex. How is Hannah? <b>matches</b> How is Hannah</i><br/>
<i>ex. How is Hannah? <b>matches</b> How is Hanna</i>

<br/>

<b>Filter on FileType(s) &nbsp; (-t &nbsp; --filetype-inclusion=)</b></br>
Restricts scope of files in query to those whose extension matches one of the provided extensions. Takes comma or semi-colon delimited list as a parameter<br/>
<i>ex. &nbsp;&nbsp; -t .cs;.txt &nbsp; or &nbsp; --filetype--inclusion=.cs;.txt</i>

<br/>

<b>Filter out FileType(s) &nbsp; (-t &nbsp; --filetype-inclusion=)</b></br>
Restricts scope of files in query to those whose extension does not matche any of the provided extensions. Takes comma or semi-colon delimited list as a parameter<br/>
<i>ex. &nbsp;&nbsp; -T .jpg;.resx &nbsp; or &nbsp; --filetype--exclusion=.jpg;.resx</i>

<br/>

<b>Filenames Only &nbsp; (-i &nbsp; --ignore-case)</b></br>
Modifies query to match against files names rather than file content

<br/>

<b>Replace Text &nbsp; (-R &nbsp; --replace=)</b></br>
Permanently replaces matched text in returned files with supplied text. Takes string as a parameter (Encapsulate in single-quotes '' or double-quotes "")<br/>
<i>ex. &nbsp;&nbsp; -R 'Hello there' &nbsp; or &nbsp; --replace='Hello there'</i>

<br/>

<b>Delete Files &nbsp; (-D &nbsp; --delete)</b></br>
Deletes all files returned in query 

<br/>

<b>Write Output to File &nbsp; (-w &nbsp; --write=)</b></br>
Write query results to an external file. Space delimited by default. Formats as comma-separated when saved with .csv extension. Takes filepath as a parameter (Encapsulate in single-quotes '' or double-quotes "")<br/>
<i>ex. &nbsp;&nbsp; -w 'MyGrepResuls.txt' &nbsp; or &nbsp; --write='MyGrepResuls.txt'</i><br/>
<i>ex. &nbsp;&nbsp; -w 'MyGrepResuls.csv' &nbsp; or &nbsp; --write='MyGrepResuls.csv'</i>
