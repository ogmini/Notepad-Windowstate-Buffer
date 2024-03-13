using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Notepad_Windowstate_Buffer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("********** Starting *********");
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Packages\Microsoft.WindowsNotepad_8wekyb3d8bbwe\LocalState\WindowState");
            string pwd = Path.Combine(Directory.GetCurrentDirectory(), "results");

            Directory.CreateDirectory(pwd);

            Console.WriteLine("Copying files from: {0} to {1}", folder, pwd);
            foreach (var path in Directory.EnumerateFiles(folder, "*.bin"))
            {
                File.Copy(path, pwd + @"\" + Path.GetFileName(path), true); //TODO: Make flag for overwriting
            }

            foreach (var path in Directory.EnumerateFiles(pwd, "*.bin"))
            {
                ParseFile(path);
            }

            Console.WriteLine("********** Completed **********");
            Console.ReadLine();
        }


        private static void ParseFile(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                if (stream.Length > 0) //Check file actually has some data
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        string hdrType = Encoding.ASCII.GetString(reader.ReadBytes(2));

                        if (hdrType == "NP")
                        {
                            CRC32Check c = new CRC32Check();

                            Console.WriteLine("=========== Processing File ==========");
                            Console.WriteLine("{0}", Path.GetFileName(filePath));

                            var sequenceNumber = stream.ReadLEB128Unsigned();
                            c.AddBytes(sequenceNumber);
                            Console.WriteLine("Sequence Number: {0}", sequenceNumber.ToString());

                            var bytesToCRC = reader.ReadLEB128Unsigned();
                            c.AddBytes(bytesToCRC);
                            Console.WriteLine("Bytes to CRC: {0}", bytesToCRC);

                            var delim = reader.ReadBytes(1);
                            c.AddBytes(delim);
                            Console.WriteLine("Unknown bytes - delim: {0}", BytestoString(delim)); 

                            var numTabs = stream.ReadLEB128Unsigned();
                            c.AddBytes(numTabs);
                            Console.WriteLine("Tabs: {0}", numTabs);


                            for (int x = 0; x < (int)numTabs; x++)
                            {
                                var chunk = reader.ReadBytes(16);
                                c.AddBytes(chunk);
                                Guid g = new Guid(chunk);
                                Console.WriteLine("Tab #{0} GUID: {1}",x, g);
                            }

                            var un = stream.ReadLEB128Unsigned(); //Active Tab
                            c.AddBytes(un);
                            Console.WriteLine("Active Tab: {0}", un);

                            //Top Left Coord
                            Console.Write("Top Left Coordinate: (");
                            for (int x = 1; x < 3; x++) 
                            {
                                var coord = reader.ReadUInt32();
                                c.AddBytes(coord);
                                Console.Write("{0}{1}", coord, x % 2 == 0 ? "" : ", ");
                            }

                            //Bottom Right Coord
                            Console.WriteLine(")");
                            Console.Write("Bottom Right Coordinate: (");
                            for (int x = 1; x < 3; x++)
                            {
                                var coord = reader.ReadUInt32();
                                c.AddBytes(coord);
                                Console.Write("{0}{1}", coord, x % 2 == 0 ? "" : ", ");
                            }

                            //Window Width and Height
                            Console.WriteLine(")");
                            Console.Write("Window Size: ");
                            for (int x = 1; x < 3; x++) 
                            {
                                var coord = reader.ReadUInt32();
                                c.AddBytes(coord);
                                Console.Write("{0}{1}", x % 2 == 0 ? " Height " : "Width ", coord);
                            }

                            var delim2 = reader.ReadBytes(1);
                            c.AddBytes(delim2);
                            Console.WriteLine();
                            Console.WriteLine("Unknown bytes - delim2: {0}", BytestoString(delim2));

                            Console.WriteLine("CRC Match: {0}", c.Check(reader.ReadBytes(4)) ? "PASS" : "!!!FAIL!!!");


                            //TODO: Check for slack space
                            if (reader.BaseStream.Length > reader.BaseStream.Position)
                            {

                                Console.WriteLine("Slack Space Detected");
                                
                                List<byte> slackBuffer = new List<byte>();
                                //Byte[] slackArray = new Byte[0];

                                //TODO: Populate array with the leftover bytes.
                                // Bytes in Reverse Order
                                // 4 byte CRC 32
                                // 1 Byte Delim
                                // 6 int32 (4 Bytes each)
                                // uLEB128 Active Tab
                                // u128 GUIDS

                                //TODO: Above isn't possible. This data could get very munged as the file space changes over time... What options do we have here...

                                // ALL OF THIS MAY BE PARTIAL AND RELIES ON ENOUGH TABS BEING OPENED AT THE SAME TIME AND LATER CLOSED
                               

                                while (reader.BaseStream.Length > reader.BaseStream.Position)
                                {
                                    var u = reader.ReadByte();
                                    slackBuffer.Add(u);
                                }

                                Console.WriteLine(BytestoString(slackBuffer.ToArray()));
                                
                                
                                if (slackBuffer.Count > 42)
                                {


                                }
                                else
                                {
                                    //using (MemoryStream memSt = new MemoryStream(slackBuffer.ToArray()))
                                    //{
                                    //    using (BinaryReader rdr = new BinaryReader(memSt))
                                    //    {
                                    //        try
                                    //        {
                                    //            var aTab = memSt.ReadLEB128Unsigned();
                                    //            Console.WriteLine("Active Tab: {0}", un);

                                    //            var c1 = rdr.ReadUInt32();
                                    //            Console.WriteLine("{0}", c1);

                                    //            var c2 = rdr.ReadUInt32();
                                    //            Console.WriteLine("{0}", c1);

                                    //            var c3 = rdr.ReadUInt32();
                                    //            Console.WriteLine("{0}", c1);

                                    //            var c4 = rdr.ReadUInt32();
                                    //            Console.WriteLine("{0}", c1);

                                    //            var c5 = rdr.ReadUInt32();
                                    //            Console.WriteLine("{0}", c1);

                                    //            var c6 = rdr.ReadUInt32();
                                    //            Console.WriteLine("{0}", c1);

                                    //            var d = rdr.ReadBytes(1);

                                    //            var crc = rdr.ReadBytes(4);
                                    //            Array.Reverse(crc);
                                    //            Console.WriteLine("Recovered CRC: {0}", BytestoString(crc));
                                    //        }
                                    //        catch
                                    //        {

                                    //        }
                                    //    }
                                    //}
                                }

                                
                                //slackBuffer.Reverse();

                                ////Console.WriteLine(String.Join(" ", slackBuffer));
                                //Console.WriteLine(BytestoString(slackBuffer.ToArray()));
                            }



                            Console.WriteLine("End of Stream");
                     
                        }
                        else
                        {
                            Console.WriteLine("Invalid File");
                        }
                    }
                }
            }
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

        private static byte[] AddByteToArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 1);
            newArray[0] = newByte;
            return newArray;
        }
    }
}
