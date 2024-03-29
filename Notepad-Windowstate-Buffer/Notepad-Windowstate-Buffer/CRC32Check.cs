﻿using Notepad_Windowstate_Buffer;
using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Notepad_Windowstate_Buffer
{
    internal class CRC32Check
    {
        public byte[] CRC32 { get; set; }

        private List<byte> data;

        public CRC32Check()
        {
            data = new List<byte>();
        }

        public void AddBytes(byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                data.Add(b);
            }
        }

        public void AddBytes(ulong value)
        {
            AddBytes(LEB128Converter.WriteLEB128Unsigned(value));
        }

        public void AddBytes(ushort value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }

        public void AddBytes(uint value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }

        public bool Check(byte[] crc32)
        {
            var crc32calculated = Crc32.Hash(data.ToArray());
            Array.Reverse(crc32calculated);

            return crc32calculated.SequenceEqual(crc32);
        }
    }
}
