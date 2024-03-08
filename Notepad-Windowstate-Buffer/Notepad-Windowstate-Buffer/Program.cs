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

                            var un1 = reader.ReadBytes(19);
                            c.AddBytes(un1);
                            Console.WriteLine("Unknown bytes - un1: {0}", BytestoString(un1));

                            var chunk1 = reader.ReadBytes(4);
                            c.AddBytes(chunk1);
                            Console.WriteLine("Unknown bytes - chunk1: {0}", BytestoString(chunk1));
                            var chunk2 = reader.ReadBytes(4);
                            c.AddBytes(chunk2);
                            Console.WriteLine("Unknown bytes - chunk2: {0}", BytestoString(chunk2));
                            var chunk3 = reader.ReadBytes(4);
                            c.AddBytes(chunk3);
                            Console.WriteLine("Unknown bytes - chunk3: {0}", BytestoString(chunk3));
                            var chunk4 = reader.ReadBytes(4);
                            c.AddBytes(chunk4);
                            Console.WriteLine("Unknown bytes - chunk4: {0}", BytestoString(chunk4));
                            var chunk5 = reader.ReadBytes(4);
                            c.AddBytes(chunk5);
                            Console.WriteLine("Unknown bytes - chunk5: {0}", BytestoString(chunk5));
                            var chunk6 = reader.ReadBytes(4);
                            c.AddBytes(chunk6);
                            Console.WriteLine("Unknown bytes - chunk6: {0}", BytestoString(chunk6));

                            var un2 = reader.ReadBytes(2);
                            c.AddBytes(un2);
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
