using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad_Windowstate_Buffer
{
    internal class SlackSpaceRecovery
    {
        private List<byte> data;

        /// <summary>
        /// NONE OF THE BELOW IS RELIABLE
        /// Another possible tactic... Look for 5 byte sequences starting with 0x00 and 4 non-NULL bytes. This isn't perfect because 0x00 0x00 0x00 0x00 0x00 is 
        /// mathematically possible... We'll have to ignore that possibility
        /// Will need to assume 1 byte Active Tabs or 2 byte Active Tabs or possible more...
        /// </summary>
        /// <param name="slackByes"></param>
        public SlackSpaceRecovery(List<byte> slackByes)
        {
            data = slackByes;
        }

        private static string BytestoString(byte[] bytes)
        {
            string retVal = string.Empty;

            foreach (byte b in bytes)
            {
                retVal += String.Format("0x{0} ", b.ToString("X2"));
            }

            return retVal;
        }

        /// <summary>
        /// Attempts to find possible end CRC32 bytes and read backwards from there. Very bruteforced with many false positives.
        /// Can handle closing multiple tabs at once. Opening/closing tabs in different combinations will mess up this approach.
        /// It is very likely that the uINT32 Coordinates will result in multiple false hits. 
        /// </summary>
        public void Approach1()
        {
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("----- Parsing slack space using Approach 1 ------");
            Console.WriteLine("-------------------------------------------------");

            var approach1 = data.ToArray();

            //1 byte Active Tab 
            for (int x = approach1.Length - 5; x >= 0; x--)
            {
                if (approach1[x] == 0 && approach1[x + 1] != 0)
                {
                    var crc = new Byte[4];
                    Array.Copy(approach1, x + 1, crc, 0, 4);
                    if (BitConverter.ToInt32(crc, 0) > 0)
                    {
                        try
                        {
                            Console.WriteLine("-------------------------------------------------");
                            Console.WriteLine("---- (DANGER) Start Recovered Chunk (DANGER) ----");
                            Console.WriteLine("-------------------------------------------------");

                            Console.WriteLine("CRC: {0}", BytestoString(crc));

                            var pc6 = new Byte[4];
                            Array.Copy(approach1, x - 4, pc6, 0, 4);
                            Console.WriteLine("Window Size: Width __ Height {0}", BitConverter.ToInt32(pc6, 0));
                            var pc5 = new Byte[4];
                            Array.Copy(approach1, x - 8, pc5, 0, 4);
                            Console.WriteLine("-Window Size: Width {0} Height {1}", BitConverter.ToInt32(pc5, 0), BitConverter.ToInt32(pc6, 0));

                            var pc4 = new Byte[4];
                            Array.Copy(approach1, x - 12, pc4, 0, 4);
                            Console.WriteLine("Bottom Right Coordinate: (__, {0})", BitConverter.ToInt32(pc4, 0));
                            var pc3 = new Byte[4];
                            Array.Copy(approach1, x - 16, pc3, 0, 4);
                            Console.WriteLine("-Bottom Right Coordinate: ({0}, {1})", BitConverter.ToInt32(pc3, 0), BitConverter.ToInt32(pc4, 0));

                            var pc2 = new Byte[4];
                            Array.Copy(approach1, x - 20, pc2, 0, 4);
                            Console.WriteLine("Bottom Right Coordinate: (__, {0})", BitConverter.ToInt32(pc2, 0));
                            var pc1 = new Byte[4];
                            Array.Copy(approach1, x - 24, pc1, 0, 4);
                            Console.WriteLine("-Bottom Right Coordinate: ({0}, {1})", BitConverter.ToInt32(pc1, 0), BitConverter.ToInt32(pc2, 0));

                            var aTab = new Byte[1];
                            Array.Copy(approach1, x - 25, aTab, 0, 1);
                            Console.WriteLine("Active Tab: {0}", aTab.ReadLEB128Unsigned());

                            //TODO: Try to handle partial GUIDs
                            for (int p = x - 41; p > 0; p = p - 16)
                            {
                                var gChunk = new Byte[16];
                                Array.Copy(approach1, p, gChunk, 0, 16);
                                Guid g = new Guid(gChunk);
                                Console.WriteLine("GUID: {0}", g);
                            }
                            Console.WriteLine("-------------------------------------------------");
                            Console.WriteLine("----- (DANGER) End Recovered Chunk (DANGER) -----");
                            Console.WriteLine("-------------------------------------------------");
                            Console.WriteLine();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("*** End of Buffer Reached - {0} ***", ex.Message);
                            Console.WriteLine("-------------------------------------------------");
                            Console.WriteLine("----- (DANGER) End Recovered Chunk (DANGER) -----");
                            Console.WriteLine("-------------------------------------------------");
                            Console.WriteLine();
                        }
                    }
                }
            }


            //2 byte Active Tab
            for (int x = approach1.Length - 5; x >= 0; x--)
            {
                if (approach1[x] == 0 && approach1[x + 1] != 0)
                {
                    var crc = new Byte[4];
                    Array.Copy(approach1, x + 1, crc, 0, 4);
                    if (BitConverter.ToInt32(crc, 0) > 0)
                    {
                        try
                        {
                            Console.WriteLine("-------------------------------------------------");
                            Console.WriteLine("---- (DANGER) Start Recovered Chunk (DANGER) ----");
                            Console.WriteLine("-------------------------------------------------");
                            Console.WriteLine("CRC: {0}", BytestoString(crc));

                            var pc6 = new Byte[4];
                            Array.Copy(approach1, x - 4, pc6, 0, 4);
                            Console.WriteLine("Window Size: Width __ Height {0}", BitConverter.ToInt32(pc6, 0));
                            var pc5 = new Byte[4];
                            Array.Copy(approach1, x - 8, pc5, 0, 4);
                            Console.WriteLine("-Window Size: Width {0} Height {1}", BitConverter.ToInt32(pc5, 0), BitConverter.ToInt32(pc6, 0));

                            var pc4 = new Byte[4];
                            Array.Copy(approach1, x - 12, pc4, 0, 4);
                            Console.WriteLine("Bottom Right Coordinate: (__, {0})", BitConverter.ToInt32(pc4, 0));
                            var pc3 = new Byte[4];
                            Array.Copy(approach1, x - 16, pc3, 0, 4);
                            Console.WriteLine("-Bottom Right Coordinate: ({0}, {1})", BitConverter.ToInt32(pc3, 0), BitConverter.ToInt32(pc4, 0));

                            var pc2 = new Byte[4];
                            Array.Copy(approach1, x - 20, pc2, 0, 4);
                            Console.WriteLine("Bottom Right Coordinate: (__, {0})", BitConverter.ToInt32(pc2, 0));
                            var pc1 = new Byte[4];
                            Array.Copy(approach1, x - 24, pc1, 0, 4);
                            Console.WriteLine("-Bottom Right Coordinate: ({0}, {1})", BitConverter.ToInt32(pc1, 0), BitConverter.ToInt32(pc2, 0));

                            var aTab = new Byte[2];
                            Array.Copy(approach1, x - 26, aTab, 0, 2);
                            Console.WriteLine("Active Tab: {0}", BytestoString(aTab));

                            //TODO: Try to handle partial GUIDs
                            for (int p = x - 42; p > 0; p = p - 16)
                            {
                                var gChunk = new Byte[16];
                                Array.Copy(approach1, p, gChunk, 0, 16);
                                Guid g = new Guid(gChunk);
                                Console.WriteLine("GUID: {0}", g);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("*** End of Buffer Reached - {0} ***", ex.Message);
                            Console.WriteLine("-------------------------------------------------");
                            Console.WriteLine("----- (DANGER) End Recovered Chunk (DANGER) -----");
                            Console.WriteLine("-------------------------------------------------");
                            Console.WriteLine();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Assumes user is only closing 1 Tab at a time. Opening/closing tabs in different combinations will mess up this approach. 
        /// </summary>
        public void Approach2() 
        {
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("----- Parsing slack space using Approach 2 ------");
            Console.WriteLine("-------------------------------------------------");

            if (data.Count >= 32)
            {
                switch (data.Count % 32)
                {
                    case 0:
                        {
                            using (MemoryStream memSt = new MemoryStream(data.ToArray()))
                            {
                                using (BinaryReader rdr = new BinaryReader(memSt))
                                {
                                    try
                                    {
                                        while (rdr.BaseStream.Length > rdr.BaseStream.Position)
                                        {
                                            Console.WriteLine("-------------------------------------------------");
                                            Console.WriteLine("---- (DANGER) Start Recovered Chunk (DANGER) ----");
                                            Console.WriteLine("-------------------------------------------------");

                                            var partGUID = rdr.ReadBytes(2);
                                            Console.WriteLine("Partial GUID: {0}", BytestoString(partGUID));

                                            var aTab = memSt.ReadLEB128Unsigned();
                                            Console.WriteLine("Active Tab: {0}", aTab);

                                            var c1 = rdr.ReadUInt32();
                                            var c2 = rdr.ReadUInt32();
                                            Console.WriteLine("Top Left Coordinate: ({0}, {1})", c1, c2);

                                            var c3 = rdr.ReadUInt32();
                                            var c4 = rdr.ReadUInt32();
                                            Console.WriteLine("Bottom Right Coordinate: ({0}, {1})", c3, c4);

                                            var c5 = rdr.ReadUInt32();
                                            var c6 = rdr.ReadUInt32();
                                            Console.WriteLine("Window Size: Width {0} Height {1}", c5, c6);

                                            var d = rdr.ReadBytes(1);

                                            var crc = rdr.ReadBytes(4);
                                            Console.WriteLine("Recovered CRC: {0}", BytestoString(crc));

                                            Console.WriteLine("-------------------------------------------------");
                                            Console.WriteLine("----- (DANGER) End Recovered Chunk (DANGER) -----");
                                            Console.WriteLine("-------------------------------------------------");
                                            Console.WriteLine();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("*** End of Buffer Reached - {0} ***", ex.Message);
                                        Console.WriteLine("-------------------------------------------------");
                                        Console.WriteLine("----- (DANGER) End Recovered Chunk (DANGER) -----");
                                        Console.WriteLine("-------------------------------------------------");
                                        Console.WriteLine();
                                    }
                                }
                            }
                        }
                        break;
                    case 1: //There might be a max to this... where it no longer applies
                        {
                            using (MemoryStream memSt = new MemoryStream(data.ToArray()))
                            {
                                using (BinaryReader rdr = new BinaryReader(memSt))
                                {
                                    try
                                    {
                                        for (int x = 0; x < (data.Count / 32) - 1; x++)
                                        {
                                            Console.WriteLine("-------------------------------------------------");
                                            Console.WriteLine("---- (DANGER) Start Recovered Chunk (DANGER) ----");
                                            Console.WriteLine("-------------------------------------------------");

                                            var lpartGUID = rdr.ReadBytes(2);
                                            Console.WriteLine("Partial GUID: {0}", BytestoString(lpartGUID));

                                            var laTab = memSt.ReadLEB128Unsigned();
                                            Console.WriteLine("Active Tab: {0}", laTab);

                                            var lc1 = rdr.ReadUInt32();
                                            var lc2 = rdr.ReadUInt32();
                                            Console.WriteLine("Top Left Coordinate: ({0}, {1})", lc1, lc2);

                                            var lc3 = rdr.ReadUInt32();
                                            var lc4 = rdr.ReadUInt32();
                                            Console.WriteLine("Bottom Right Coordinate: ({0}, {1})", lc3, lc4);

                                            var lc5 = rdr.ReadUInt32();
                                            var lc6 = rdr.ReadUInt32();
                                            Console.WriteLine("Window Size: Width {0} Height {1}", lc5, lc6);

                                            var ld = rdr.ReadBytes(1);

                                            var lcrc = rdr.ReadBytes(4);
                                            Console.WriteLine("Recovered CRC: {0}", BytestoString(lcrc));

                                            Console.WriteLine("-------------------------------------------------");
                                            Console.WriteLine("----- (DANGER) End Recovered Chunk (DANGER) -----");
                                            Console.WriteLine("-------------------------------------------------");
                                            Console.WriteLine();
                                        }

                                        Console.WriteLine("-------------------------------------------------");
                                        Console.WriteLine("---- (DANGER) Start Recovered Chunk (DANGER) ----");
                                        Console.WriteLine("-------------------------------------------------");

                                        var partGUID = rdr.ReadBytes(3);
                                        Console.WriteLine("Partial GUID: {0}", BytestoString(partGUID));

                                        var aTab = memSt.ReadLEB128Unsigned();
                                        Console.WriteLine("Active Tab: {0}", aTab);

                                        var c1 = rdr.ReadUInt32();
                                        var c2 = rdr.ReadUInt32();
                                        Console.WriteLine("Top Left Coordinate: ({0}, {1})", c1, c2);

                                        var c3 = rdr.ReadUInt32();
                                        var c4 = rdr.ReadUInt32();
                                        Console.WriteLine("Bottom Right Coordinate: ({0}, {1})", c3, c4);

                                        var c5 = rdr.ReadUInt32();
                                        var c6 = rdr.ReadUInt32();
                                        Console.WriteLine("Window Size: Width {0} Height {1}", c5, c6);

                                        var d = rdr.ReadBytes(1);

                                        var crc = rdr.ReadBytes(4);
                                        Console.WriteLine("Recovered CRC: {0}", BytestoString(crc));

                                        Console.WriteLine("-------------------------------------------------");
                                        Console.WriteLine("----- (DANGER) End Recovered Chunk (DANGER) -----");
                                        Console.WriteLine("-------------------------------------------------");
                                        Console.WriteLine();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("*** End of Buffer Reached - {0} ***", ex.Message);
                                        Console.WriteLine("-------------------------------------------------");
                                        Console.WriteLine("----- (DANGER) End Recovered Chunk (DANGER) -----");
                                        Console.WriteLine("-------------------------------------------------");
                                        Console.WriteLine();
                                    }
                                }
                            }
                        }
                        break;
                    case 2:
                        {
                            using (MemoryStream memSt = new MemoryStream(data.ToArray()))
                            {
                                using (BinaryReader rdr = new BinaryReader(memSt))
                                {
                                    try
                                    {
                                        for (int x = 0; x < (data.Count / 32) - 1; x++)
                                        {
                                            Console.WriteLine("-------------------------------------------------");
                                            Console.WriteLine("---- (DANGER) Start Recovered Chunk (DANGER) ----");
                                            Console.WriteLine("-------------------------------------------------");

                                            var lpartGUID = rdr.ReadBytes(2);
                                            Console.WriteLine("Partial GUID: {0}", BytestoString(lpartGUID));

                                            var laTab = memSt.ReadLEB128Unsigned();
                                            Console.WriteLine("Active Tab: {0}", laTab);

                                            var lc1 = rdr.ReadUInt32();
                                            var lc2 = rdr.ReadUInt32();
                                            Console.WriteLine("Top Left Coordinate: ({0}, {1})", lc1, lc2);

                                            var lc3 = rdr.ReadUInt32();
                                            var lc4 = rdr.ReadUInt32();
                                            Console.WriteLine("Bottom Right Coordinate: ({0}, {1})", lc3, lc4);

                                            var lc5 = rdr.ReadUInt32();
                                            var lc6 = rdr.ReadUInt32();
                                            Console.WriteLine("Window Size: Width {0} Height {1}", lc5, lc6);

                                            var ld = rdr.ReadBytes(1);

                                            var lcrc = rdr.ReadBytes(4);
                                            Console.WriteLine("Recovered CRC: {0}", BytestoString(lcrc));
                                            Console.WriteLine("-------------------------------------------------");
                                            Console.WriteLine("----- (DANGER) End Recovered Chunk (DANGER) -----");
                                            Console.WriteLine("-------------------------------------------------");
                                            Console.WriteLine();
                                        }

                                        Console.WriteLine("-------------------------------------------------");
                                        Console.WriteLine("---- (DANGER) Start Recovered Chunk (DANGER) ----");
                                        Console.WriteLine("-------------------------------------------------");

                                        var partGUID = rdr.ReadBytes(3);
                                        Console.WriteLine("Partial GUID: {0}", BytestoString(partGUID));

                                        var aTab = memSt.ReadLEB128Unsigned();
                                        Console.WriteLine("Active Tab: {0}", aTab);

                                        var c1 = rdr.ReadUInt32();
                                        var c2 = rdr.ReadUInt32();
                                        Console.WriteLine("Top Left Coordinate: ({0}, {1})", c1, c2);

                                        var c3 = rdr.ReadUInt32();
                                        var c4 = rdr.ReadUInt32();
                                        Console.WriteLine("Bottom Right Coordinate: ({0}, {1})", c3, c4);

                                        var c5 = rdr.ReadUInt32();
                                        var c6 = rdr.ReadUInt32();
                                        Console.WriteLine("Window Size: Width {0} Height {1}", c5, c6);

                                        var d = rdr.ReadBytes(1);

                                        var crc = rdr.ReadBytes(4);
                                        Console.WriteLine("Recovered CRC: {0}", BytestoString(crc));
                                        Console.WriteLine("-------------------------------------------------");
                                        Console.WriteLine("----- (DANGER) End Recovered Chunk (DANGER) -----");
                                        Console.WriteLine("-------------------------------------------------");
                                        Console.WriteLine();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("*** End of Buffer Reached - {0} ***", ex.Message);
                                        Console.WriteLine("-------------------------------------------------");
                                        Console.WriteLine("----- (DANGER) End Recovered Chunk (DANGER) -----");
                                        Console.WriteLine("-------------------------------------------------");
                                        Console.WriteLine();
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        {
                            Console.WriteLine("Unknown / Unexpected Slack Space Size");
                        }
                        break;
                }
            }
            else
            {
                using (MemoryStream memSt = new MemoryStream(data.ToArray()))
                {
                    using (BinaryReader rdr = new BinaryReader(memSt))
                    {
                        try
                        {
                            Console.WriteLine("-------------------------------------------------");
                            Console.WriteLine("---- (DANGER) Start Recovered Chunk (DANGER) ----");
                            Console.WriteLine("-------------------------------------------------");

                            var fullChunks = data.Count;
                            var u = rdr.ReadBytes(fullChunks);

                            var crc2 = new Byte[4];
                            Array.Copy(u, u.Length - 4, crc2, 0, 4);
                            Console.WriteLine("Recovered CRC: {0}", BytestoString(crc2));

                            var pc6 = new Byte[4];
                            Array.Copy(u, u.Length - 9, pc6, 0, 4);
                            Console.WriteLine("Window Size: Width __ Height {0}", BitConverter.ToInt32(pc6, 0));
                            var pc5 = new Byte[4];
                            Array.Copy(u, u.Length - 13, pc5, 0, 4);
                            Console.WriteLine("-Window Size: Width {0} Height {1}", BitConverter.ToInt32(pc5, 0), BitConverter.ToInt32(pc6, 0));

                            var pc4 = new Byte[4];
                            Array.Copy(u, u.Length - 17, pc4, 0, 4);
                            Console.WriteLine("Bottom Right Coordinate: (__, {0})", pc4);
                            var pc3 = new Byte[4];
                            Array.Copy(u, u.Length - 21, pc3, 0, 4);
                            Console.WriteLine("-Bottom Right Coordinate: ({0}, {1})", pc3, pc4);

                            var pc2 = new Byte[4];
                            Array.Copy(u, u.Length - 25, pc2, 0, 4);
                            Console.WriteLine("Bottom Right Coordinate: (__, {0})", pc2);
                            var pc1 = new Byte[4];
                            Array.Copy(u, u.Length - 29, pc1, 0, 4);
                            Console.WriteLine("-Bottom Right Coordinate: ({0}, {1})", pc1, pc2);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("*** End of Buffer Reached - {0} ***", ex.Message);
                            Console.WriteLine("-------------------------------------------------");
                            Console.WriteLine("----- (DANGER) End Recovered Chunk (DANGER) -----");
                            Console.WriteLine("-------------------------------------------------");
                            Console.WriteLine();
                        }
                    }
                }
            }
        }
    }
}
