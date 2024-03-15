using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Notepad_Windowstate_Buffer
{
    internal static class Program
    {
        public class Options
        {
            [Option('o', "output", Required = false, Default = "results", HelpText = "Output folder")]
            public string OutputFolder { get; set; }

            [Option('i', "input", Required = false, HelpText = "Input folder")]
            public string InputFolder { get; set; }

            [Option('r', "recover", Required = false, Default = -1, HelpText = "Attempt parsing of slack space")]
            public int AttemptRecovery { get; set; }

            [Option('v', "verbose", Required = false, HelpText = "Verbose output")]
            public bool Verbose { get; set; }

            [Usage(ApplicationAlias = "Notepad-Windowstate-Buffer.exe")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Default Options", new Options { });
                    yield return new Example("Attempt parsing of slack space using all approaches", new Options { AttemptRecovery = 0 });
                    yield return new Example("Attempt parsing of slack space using Approach 1", new Options { AttemptRecovery = 1 });
                }
            }
        }

        private static int attemptRecovery { get; set; }
        private static bool verbose { get; set; }
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {

                Console.WriteLine("********** Starting *********");
                string folder = (string.IsNullOrWhiteSpace(o.InputFolder) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Packages\Microsoft.WindowsNotepad_8wekyb3d8bbwe\LocalState\WindowState") : o.InputFolder);
                string pwd = Path.Combine(Directory.GetCurrentDirectory(), o.OutputFolder);

                Directory.CreateDirectory(pwd);

                attemptRecovery = o.AttemptRecovery;
                Console.WriteLine("Attempt parsing of slack space: {0}", (attemptRecovery >= 0 ? "Yes" : "No"));
                if (attemptRecovery >= 0)
                    Console.WriteLine("Approach {0}", attemptRecovery == 0 ? "All" : attemptRecovery.ToString());
                verbose = o.Verbose;
                Console.WriteLine("Verbose Output: {0}", verbose);

                Console.WriteLine("Copying files from: {0} to {1}", folder, pwd);
                foreach (var path in Directory.EnumerateFiles(folder, "*.bin"))
                {
                    File.Copy(path, pwd + @"\" + Path.GetFileName(path), true);
                }

                foreach (var path in Directory.EnumerateFiles(pwd, "*.bin"))
                {
                    ParseFile(path);
                }

                Console.WriteLine("********** Completed **********");
            });
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

                            Console.WriteLine("============= Processing File ============");
                            Console.WriteLine("{0}", Path.GetFileName(filePath));
                            Console.WriteLine("==========================================");
                            var sequenceNumber = stream.ReadLEB128Unsigned();
                            c.AddBytes(sequenceNumber);
                            Console.WriteLine("Sequence Number: {0}", sequenceNumber.ToString());
                            if (verbose)
                                Console.WriteLine("Sequence Number Bytes: {0}", BytestoString(LEB128Converter.WriteLEB128Unsigned(sequenceNumber)));

                            var bytesToCRC = reader.ReadLEB128Unsigned();
                            c.AddBytes(bytesToCRC);
                            Console.WriteLine("Bytes to CRC: {0}", bytesToCRC);
                            if (verbose)
                                Console.WriteLine("Bytes to CRC Bytes: {0}", BytestoString(LEB128Converter.WriteLEB128Unsigned(bytesToCRC)));

                            var delim = reader.ReadBytes(1);
                            c.AddBytes(delim);
                            Console.WriteLine("Delimeter: {0}", BytestoString(delim)); 

                            var numTabs = stream.ReadLEB128Unsigned();
                            c.AddBytes(numTabs);
                            Console.WriteLine("Number of Tabs: {0}", numTabs);
                            if (verbose)
                                Console.WriteLine("Number of Tabs Bytes: {0}", BytestoString(LEB128Converter.WriteLEB128Unsigned(numTabs)));


                            for (int x = 0; x < (int)numTabs; x++)
                            {
                                var chunk = reader.ReadBytes(16);
                                c.AddBytes(chunk);
                                Guid g = new Guid(chunk);
                                Console.WriteLine("Tab #{0} GUID: {1}",x, g);
                                if (verbose)
                                    Console.WriteLine("Tab #{0} GUID Bytes: {1}", x, BytestoString(chunk));
                            }

                            var un = stream.ReadLEB128Unsigned(); //Active Tab
                            c.AddBytes(un);
                            Console.WriteLine("Active Tab: {0}", un);
                            if (verbose)
                                Console.WriteLine("Active Tab Bytes: {0}", BytestoString(LEB128Converter.WriteLEB128Unsigned(un)));

                            var tlc1 = reader.ReadUInt32();
                            c.AddBytes(tlc1);
                            var tlc2 = reader.ReadUInt32();
                            c.AddBytes(tlc2);
                            Console.WriteLine("Top Left Coordinate: ({0}, {1})", tlc1, tlc2);
                            if (verbose)
                                Console.WriteLine("Top Left Coordinate Bytes: {0}, {1}", BytestoString(BitConverter.GetBytes(tlc1)), BytestoString(BitConverter.GetBytes(tlc2)));

                            var brc3 = reader.ReadUInt32();
                            c.AddBytes(brc3);
                            var brc4 = reader.ReadUInt32();
                            c.AddBytes(brc4);
                            Console.WriteLine("Bottom Right Coordinate: ({0}, {1})", brc3, brc4);
                            if (verbose)
                                Console.WriteLine("Bottom Right Coordinate Bytes: {0}, {1}", BytestoString(BitConverter.GetBytes(brc3)), BytestoString(BitConverter.GetBytes(brc4)));

                            var wsc5 = reader.ReadUInt32();
                            c.AddBytes(wsc5);
                            var wsc6 = reader.ReadUInt32();
                            c.AddBytes(wsc6);
                            Console.WriteLine("Window Size: Width {0} Height {1}", wsc5, wsc6);
                            if (verbose)
                                Console.WriteLine("Window Size Bytes: Width {0} Height {1}", BytestoString(BitConverter.GetBytes(wsc5)), BytestoString(BitConverter.GetBytes(wsc6)));

                            var delim2 = reader.ReadBytes(1);
                            c.AddBytes(delim2);
                            Console.WriteLine("Delimeter: {0}", BytestoString(delim2));

                            var crc32 = reader.ReadBytes(4);
                            Console.WriteLine("CRC Match: {0}", c.Check(crc32) ? "PASS" : "!!!FAIL!!!");
                            if (verbose)
                                Console.WriteLine("CRC Bytes: {0}", BytestoString(crc32));

                            // Check for slack space
                            if (reader.BaseStream.Length > reader.BaseStream.Position)
                            {
                                List<byte> slackBuffer = new List<byte>();
                                while (reader.BaseStream.Length > reader.BaseStream.Position)
                                {
                                    var u = reader.ReadByte();
                                    slackBuffer.Add(u);
                                }

                                Console.WriteLine();
                                Console.WriteLine("Slack Space Detected of {0} bytes", slackBuffer.Count);
                                if (verbose)
                                {
                                    Console.WriteLine(BytestoString(slackBuffer.ToArray()));
                                    Console.WriteLine();
                                }

                                string slackBIN = Path.GetDirectoryName(filePath) + @"\" + Path.GetFileNameWithoutExtension(filePath) + ".slack.bin"; 
                                Console.WriteLine("Slack Space written to: {0}", slackBIN);
                                File.WriteAllBytes(slackBIN, slackBuffer.ToArray());

                                SlackSpaceRecovery ssr = new SlackSpaceRecovery(slackBuffer);

                                switch (attemptRecovery)
                                {
                                    case 0:
                                        ssr.Approach1();
                                        ssr.Approach2();
                                        break;
                                    case 1:
                                        ssr.Approach1();
                                        break;
                                    case 2:
                                        ssr.Approach2();
                                        break;
                                    default:
                                        break;
                                }
                            }
                            Console.WriteLine("==========================================");
                            Console.WriteLine("============== End of Stream =============");
                            Console.WriteLine("==========================================");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("!!!! Invalid File: {0} !!!!", Path.GetFileName(filePath));
                            Console.WriteLine();
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
