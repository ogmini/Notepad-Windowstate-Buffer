> WORK IN PROGRESS
>
> What you see here are ever evolving notes and changing code as I investigate the file format.

# Notepad-Windowstate-Buffer

These are my attempts to reverse engineer the Windowstate files for Notepad in Microsoft Windows 11.. These files are located at: `%localappdata%\Packages\Microsoft.WindowsNotepad_8wekyb3d8bbwe\LocalState\WindowState`

Please see my other repository for the [Tabstate files](https://github.com/ogmini/Notepad-Tabstate-Buffer). 

- [010 Editor Binary Template](https://github.com/ogmini/Notepad-Windowstate-Buffer/tree/main/Templates/Notepad-WindowState.bt) - This is also in the 010 Editor online template repository for download as Notepad-WindowState.bt    
- [imHex Pattern](https://github.com/ogmini/Notepad-Windowstate-Buffer/blob/main/Templates/Notepad-WindowState.hexpat)

## Thanks

[NordGaren](https://github.com/Nordgaren) for his 010 Editor Binary Template file on the [Notepad Tabstate file](https://github.com/Nordgaren/tabstate-util/blob/master/TabState.bt). It was very handy learning the syntax and the struct/functions for uLEB128.     
[JustArrion](https://github.com/JustArion) for his imHex Pattern file on the [Notepad Tabstate file](https://github.com/JustArion/Notepad-Tabs/blob/main/ImHex-Patterns/NotepadTab.hexpat). It was very handly learning the syntax.   

## Overall Behavior

Each new tab adds more and more data to the end. Closing a tab creates a new chunk at the end of the stream. The files appears to never get smaller? New tabs will remove the chunks at the end.

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
 - Six uINT32 Numbers
   - Numbers 1 and 2 are the X,Y Coordinates in uINT32 of the top left corner of the window.
   - Numbers 3 and 4 are the X,Y Coordinates in uINT32 of the bottom right corner of the window.
   - Numbers 5 and 6 are the X,Y Coordinates in uINT32 of the relation to the bottom right corner from the top left. Essentially giving the window width and height.
 - Delimiter? (0x00)
 - CRC 32 of all the previous bytes starting from the sequence number
 - Slackspace as tabs are removed/closed
   - Prior CRC32, Coords, partial Active Tab, and partial GUID can be recovered potentially
   - This data will get super munged over time



