﻿The Chaos Koala is busy creating and modifying text files in some specific directories. We want you to create a command-line program to watch a directory we provide. The program should detect files created or modified and then output information about them. We strongly suggest creating some sample files of various sizes and testing your program for performance before submitting.

- The program takes 2 arguments, the directory to watch and a file pattern, example: `program.exe "c:\file folder" *.txt`
- The path may be an absolute path, relative to the current directory, or UNC.
- Use the modified date of the file as a trigger that the file has changed.
- Check for changes every 10 seconds.
- When a file is created output a line to the console with its name and how many lines are in it.
- When a file is modified output a line with its name and the change in number of lines (use a + or - to indicate more or less).
- When a file is deleted output a line with its name.
- Files will be ASCII or UTF-8 and will use Windows line separators (CR LF).
- Multiple files may be changed at the same time, can be up to 2 GB in size, and may be locked for several seconds at a time.
- Use multiple threads so that the program doesn't block on a single large or locked file.
- Program will be run on Windows 10.
- File names are case insensitive.

From: https://github.com/pdq/PDQ-FileWatcher-Public
