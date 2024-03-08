> WORK IN PROGRESS
>
> What you see here are ever evolving notes and changing code as I investigate the file format.

# Notepad-Windowstate-Buffer

These are my attempts to reverse engineer the Windowstate files for Notepad in Microsoft Windows 11.. These files are located at: `%localappdata%\Packages\Microsoft.WindowsNotepad_8wekyb3d8bbwe\LocalState\WindowState`

Please see my other repository for the [Tabstate files](https://github.com/ogmini/Notepad-Tabstate-Buffer). 

## File Format

 - First 2 bytes "NP"
 - Sequence number (Stored as an unsigned LEB128)
 - 4th byte appears to be a flag 2C
 - Unknown
   - There appear to be six 2 byte chunks that are related to moving and sizing. Maybe some sort of coordinates? They also appear to be qualified by 00. (Ex. 00 51 04 00) 
 - CRC 32 of all the previous bytes starting from the sequence number


