> WORK IN PROGRESS
>
> What you see here are ever evolving notes and changing code as I investigate the file format.

# Notepad-Windowstate-Buffer

These are my attempts to reverse engineer the Windowstate files for Notepad in Microsoft Windows 11.. These files are located at: `%localappdata%\Packages\Microsoft.WindowsNotepad_8wekyb3d8bbwe\LocalState\WindowState`

Please see my other repository for the [Tabstate files](https://github.com/ogmini/Notepad-Tabstate-Buffer). 

## Overall Behavior

Each new tab adds more and more data to the end

## File Format

 - First 2 bytes "NP"
 - Sequence number (Stored as an unsigned LEB128)
 - Number of bytes to the CRC32 (unsigned LEB128?)
 - Unknown 2 bytes (Possibly number of tabs?)
 - Collection of chunks (1 for every tab)
   - New chunks appear to be added with new tabs
   - GUID of the associated Tabstate file
 - Six 2 byte chunks that are delimited by 00. (Ex. 51 04 00 00)
   - Chunk 1 and 2 are the X,Y Coordinates in UINT16 of the top left corner of the window.
   - Chunk 3 and 4 are the X,Y Coordinates in UINT16 of the bottom right corner of the window.
   - Chunk 5 and 6 are the X,Y Coordinates in UINT16 of the relation to the bottom right corner from the top left. Essentially giving the window width and height.
 - CRC 32 of all the previous bytes starting from the sequence number

Opening New Tabs will cause more bytes to be written what are these?


