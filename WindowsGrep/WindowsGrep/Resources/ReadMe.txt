================= Windows Grep =================

-r	--recursive			: Recursive search
-d	--directory=			: Target directory (surround with ' or ")
-c	--context=			: Return n leading/trailing characters surrounding the matched term
-n	--results=			: Returns first n results
-b	--ignore-breaks			: Ignore line breaks
-i	--ignore-case			: Ignore case
-f	--file=				: Target file (surround with ' or ")
-F	--fixed-strings			: Interprets patterns as fixed strings
-G	--basic-regexp			: Interprets patterns as basic regular expressions (default)
-t	--filetype-inclusion=		: Filters all files of non-specified type (comma or semicolon delimited)
-T	--filetype-exclusion=		: Filters all files of specified type (comma or semicolon delimited)
-k	--filenames-only		: Match against file names rather than file content
-z	--filesize-minimum=		: Filter matched files <= n kilobytes
-Z	--filesize-maximum=		: Filter matched files >= n kilobytes
-RX	--replace=			: Replace text in matched files (surround with ' or ")
-DX	--delete-files			: Delete files returned in search
-w	--write=			: Write output to specified filepath (surround with ' or ")