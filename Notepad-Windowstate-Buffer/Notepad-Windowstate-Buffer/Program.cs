using System;
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

                            var typeFlag = reader.ReadBytes(1);
                            c.AddBytes(typeFlag);
                            Console.WriteLine("Flag: {0}", BytestoString(typeFlag));

                            var un1 = reader.ReadBytes(18);
                            c.AddBytes(un1);
                            Console.WriteLine("Unknown bytes - un1: {0}", BytestoString(un1));

                            //var chunk1a = reader.ReadBytes(1);
                            //c.AddBytes(chunk1a);
                            //Console.WriteLine("Unknown bytes - chunk1a: {0}", BytestoString(chunk1a));
                            //var coord1 = stream.ReadLEB128Unsigned(); 
                            //c.AddBytes(coord1);
                            //Console.WriteLine("coord1a {0}", coord1);
                            ////var coord1b = stream.ReadLEB128Unsigned();
                            ////c.AddBytes(coord1b);
                            ////Console.WriteLine("coord1b {0}", coord1b);
                            //var chunk1b = reader.ReadBytes(1);
                            //c.AddBytes(chunk1b);
                            //Console.WriteLine("Unknown bytes - chunk1b: {0}", BytestoString(chunk1b));

                            //Bottom Left Coord
                            Console.Write("First Coordinate: (");
                            for (int x = 1; x < 3; x++) //we have 4 bytes here
                            {
                                var chunka = reader.ReadBytes(1);
                                c.AddBytes(chunka);
                                //Console.WriteLine("Unknown bytes - chunk-{0}-a: {1}", x, BytestoString(chunka));

                                var coord = reader.ReadBytes(2);
                                c.AddBytes(coord);
                                Console.Write("{0}{1}", BitConverter.ToUInt16(coord, 0), x%2 == 0 ? "" : ", ");

                                var chunkb = reader.ReadBytes(1);
                                c.AddBytes(chunkb);
                                //Console.WriteLine("Unknown bytes - chunk-{0}-b: {1}", x, BytestoString(chunkb));
                            }

                            //Top Right Coord
                            Console.WriteLine(")");
                            Console.Write("Second Coordinate: (");
                            for (int x = 1; x < 3; x++) //we have 4 bytes here
                            {
                                var chunka = reader.ReadBytes(1);
                                c.AddBytes(chunka);
                                //Console.WriteLine("Unknown bytes - chunk-{0}-a: {1}", x, BytestoString(chunka));

                                var coord = reader.ReadBytes(2);
                                c.AddBytes(coord);
                                Console.Write("{0}{1}", BitConverter.ToUInt16(coord, 0), x % 2 == 0 ? "" : ", ");

                                var chunkb = reader.ReadBytes(1);
                                c.AddBytes(chunkb);
                                //Console.WriteLine("Unknown bytes - chunk-{0}-b: {1}", x, BytestoString(chunkb));
                            }

                            //Unknow Coord
                            Console.WriteLine(")");
                            Console.Write("Third Coordinate: (");
                            for (int x = 1; x < 3; x++) //we have 4 bytes here
                            {
                                var chunka = reader.ReadBytes(1);
                                c.AddBytes(chunka);
                                //Console.WriteLine("Unknown bytes - chunk-{0}-a: {1}", x, BytestoString(chunka));

                                var coord = reader.ReadBytes(2);
                                c.AddBytes(coord);
                                Console.Write("{0}{1}", BitConverter.ToUInt16(coord, 0), x % 2 == 0 ? "" : ", ");

                                var chunkb = reader.ReadBytes(1);
                                c.AddBytes(chunkb);
                                //Console.WriteLine("Unknown bytes - chunk-{0}-b: {1}", x, BytestoString(chunkb));
                            }

                            var un2 = reader.ReadBytes(2);
                            c.AddBytes(un2);
                            Console.WriteLine(")");
                            Console.WriteLine("Unknown bytes - un2: {0}", BytestoString(un2));

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
