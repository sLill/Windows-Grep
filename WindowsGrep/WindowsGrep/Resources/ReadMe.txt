================= Windows Grep =================
*Enclose any filepath parameters in single/double quotes

-r	--recursive			: Recursive search
-d	--directory=			: Target directory (surround with ' or ")
-c	--context=			: Return n leading/trailing characters surrounding the matched term
-b	--ignore-breaks			: Ignore line breaks
-i	--ignore-case			: Ignore case
-f	--file=				: Target file
-F	--fixed-strings			: Interprets patterns as fixed strings
-G	--basic-regexp			: Interprets patterns as basic regular expressions (default)
-t	--filetype-inclusion=		: Filters all files of non-specified type (comma or semicolon delimited)
-T	--filetype-exclusion=		: Filters files of specified type (comma or semicolon delimited)
-k	--filenames-only		: Match against file names rather than file content
-R	--replace=			: Replace text in matched files (surround with ' or ")
-D	--delete-files			: Delete files returned in search
-w	--write=			: Write output to specified filepath