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
 - Sequence number (uLEB128)
 - Number of bytes to the CRC32 (uLEB128)
 - Delimiter? (0x00)
 - Number of Tabs (uLEB128)
 - Collection of chunks (1 for every tab)
   - New chunks added with new tabs
   - Chunks are ordered the same was as in Notepad. Changing the order will change the order of the chunks accordingly.
   - GUID of the associated Tabstate file
 - Active Tab (uLEB128)
 - Six 4 byte chunks 
   - Chunk 1 and 2 are the X,Y Coordinates in uINT32 of the top left corner of the window.
   - Chunk 3 and 4 are the X,Y Coordinates in uINT32 of the bottom right corner of the window.
   - Chunk 5 and 6 are the X,Y Coordinates in uINT32 of the relation to the bottom right corner from the top left. Essentially giving the window width and height.
 - Delimiter? (0x00)
 - CRC 32 of all the previous bytes starting from the sequence number




