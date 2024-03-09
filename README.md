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
 - Unknown 18 bytes
 - Six 2 byte chunks that are by qualified by 00. (Ex. 00 51 04 00)
   - Chunk 1 and 2 are the X,Y Coordinates in UINT16 of the top left corner of the window.
   - Chunk 3 and 4 are the X,Y Coordinates in UINT16 of the bottom right corner of the window.
   - Chunk 5 and 6 are the X,Y Coordinates in UINT16 of the relation to the bottom right corner from the top left. Essentially giving the window width and height.
 - CRC 32 of all the previous bytes starting from the sequence number


