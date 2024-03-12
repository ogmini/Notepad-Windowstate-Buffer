#include <std/string.pat>
#include <type/leb128.pat>
#include <std/time.pat>
#include <std/core.pat>

using ul = type::uLEB128;
using int = u32;

struct Tab
{
    u128 GUID;
};

struct WindowState
{
    char NullTerminatedHeaderIdentifier[2]; // NP
    ul SequenceNumber;
    ul BytesToCRC32;
    u8 Delim[1];
    ul NumberTabs;

    // TODO: Loop records  
    Tab Tabs[NumberTabs];
    
    
    //
    ul ActiveTab;
    
    int CoordTopLeftX;
    int CoordTopLeftY;
    
    int CoordBottomRightX;
    int CoordBottomRightY;
    
    int WindowSizeWidth;
    int WindowSizeHeight;
    
    u8 Delim2[1];
    
    int crc32;
};




// -- Declaration --

WindowState state @ 0x0;



