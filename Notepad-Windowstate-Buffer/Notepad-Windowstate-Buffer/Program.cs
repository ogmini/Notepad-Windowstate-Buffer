﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                            Console.WriteLine("Unknown bytes - delim: {0}", BytestoString(delim)); //TODO: This is wrong. Need to figure out the format of the chunks

                            var numTabs = stream.ReadLEB128Unsigned();
                            c.AddBytes(numTabs);
                            Console.WriteLine("Tabs: {0}", numTabs);
                            //var un1 = reader.ReadBytes(2); // Maybe number of tabs?? is in here?
                            //c.AddBytes(un1);
                            //Console.WriteLine("Unknown bytes - un1: {0}", BytestoString(un1));

                            //var clength = (bytesToCRC - 1)

                            //var un2 = reader.ReadBytes((int)clength);
                            //c.AddBytes(un2);
                            //Console.WriteLine("Unknown bytes - un2: {0}", BytestoString(un2)); //TODO: This is wrong. Need to figure out the format of the chunks

                            for (int x = 0; x < (int)numTabs; x++)
                            {
                                var chunk = reader.ReadBytes(16);
                                c.AddBytes(chunk);
                                Console.WriteLine("Unknown bytes - chunk{0}: {1}", x, BytestoString(chunk));

                                Guid g = new Guid(chunk);
                                Console.WriteLine("GUID: {0}", g);

                                //var tabNumber = reader.ReadLEB128Unsigned();
                                //c.AddBytes(tabNumber);
                                //Console.WriteLine("Tab Number: {0}", tabNumber);
                            }




                            //var un1 = reader.ReadBytes(18);
                            //c.AddBytes(un1);
                            //Console.WriteLine("Unknown bytes - un1: {0}", BytestoString(un1));
                            //TODO: 2nd byte appears to increment with added Tabs. 

                            var un = stream.ReadLEB128Unsigned(); //reader.ReadBytes(1);  //TODO: This might be a uLEB128 again...
                            c.AddBytes(un);
                            Console.WriteLine("Unknown bytes - un: {0} {1}", BytestoString(LEB128Converter.WriteLEB128Unsigned(un)), un);


                            //Top Left Coord
                            Console.Write("Top Left Coordinate: (");
                            for (int x = 1; x < 3; x++) //we have 4 bytes here
                            {
                                var coord = reader.ReadBytes(2);
                                c.AddBytes(coord);
                                Console.Write("{0}{1}", BitConverter.ToUInt16(coord, 0), x%2 == 0 ? "" : ", ");

                                var chunkb = reader.ReadBytes(2);
                                c.AddBytes(chunkb);
                                //Console.WriteLine("Unknown bytes - chunk-{0}-b: {1}", x, BytestoString(chunkb));
                            }

                            //Bottom Right Coord
                            Console.WriteLine(")");
                            Console.Write("Bottom Right Coordinate: (");
                            for (int x = 1; x < 3; x++) //we have 4 bytes here
                            {
                                var coord = reader.ReadBytes(2);
                                c.AddBytes(coord);
                                Console.Write("{0}{1}", BitConverter.ToUInt16(coord, 0), x % 2 == 0 ? "" : ", ");

                                var chunkb = reader.ReadBytes(2);
                                c.AddBytes(chunkb);
                                //Console.WriteLine("Unknown bytes - chunk-{0}-b: {1}", x, BytestoString(chunkb));
                            }

                            //Window Width and Height
                            Console.WriteLine(")");
                            Console.Write("Window Size: ");
                            for (int x = 1; x < 3; x++) //we have 4 bytes here
                            {
                                var coord = reader.ReadBytes(2);
                                c.AddBytes(coord);
                                Console.Write("{0}{1}", x % 2 == 0 ? " Height " : "Width ", BitConverter.ToUInt16(coord, 0));

                                var chunkb = reader.ReadBytes(2);
                                c.AddBytes(chunkb);
                                //Console.WriteLine("Unknown bytes - chunk-{0}-b: {1}", x, BytestoString(chunkb));
                            }

                            var delim2 = reader.ReadBytes(1);
                            c.AddBytes(delim2);
                            Console.WriteLine();
                            Console.WriteLine("Unknown bytes - delim2: {0}", BytestoString(delim2));

                            Console.WriteLine("CRC Match: {0}", c.Check(reader.ReadBytes(4)) ? "PASS" : "!!!FAIL!!!");

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
    }
}