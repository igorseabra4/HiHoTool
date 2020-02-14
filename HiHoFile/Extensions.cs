using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HiHoFile
{
    public static class Extensions
    {
        public static bool dontSwitch = false;

        public static void SendMessage()
        {
            Console.WriteLine();
        }

        public static void SendMessage(string message)
        {
            Console.WriteLine(message);
        }

        public static int Switch(this int value)
        {
            if (dontSwitch)
                return value;
            return BitConverter.ToInt32(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
        }

        public static uint Switch(this uint value)
        {
            if (dontSwitch)
                return value;
            return BitConverter.ToUInt32(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
        }

        public static long Switch(this long value)
        {
            if (dontSwitch)
                return value;
            return BitConverter.ToInt64(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
        }

        public static ulong Switch(this ulong value)
        {
            if (dontSwitch)
                return value;
            return BitConverter.ToUInt64(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
        }

        public static float Switch(this float value)
        {
            if (dontSwitch)
                return value;
            return BitConverter.ToSingle(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
        }
        
        public static uint BKDRHash(string str)
        {
            str = str.ToUpper();
            uint seed = 131;
            uint hash = 0;
            int length = str.Length;
            
            for (int i = 0; i < length; i++)
                hash = (hash * seed) + str[i];

            return hash;
        }

        public static string ReadString(BinaryReader binaryReader)
        {
            List<char> charList = new List<char>();
            do charList.Add((char)binaryReader.ReadByte());
            while (charList.Last() != '\0');
            charList.Remove('\0');
            
            return new string(charList.ToArray());
        }

        public static void AddString(this List<byte> listBytes, string writeString)
        {
            foreach (char i in writeString)
                listBytes.Add((byte)i);
            listBytes.Add(0);
        }

        public static void AddBigEndian(this List<byte> listBytes, float value)
        {
            if (dontSwitch)
                listBytes.AddRange(BitConverter.GetBytes(value));
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, int value)
        {
            if (dontSwitch)
                listBytes.AddRange(BitConverter.GetBytes(value));
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, uint value)
        {
            if (dontSwitch)
                listBytes.AddRange(BitConverter.GetBytes(value));
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, short value)
        {
            if (dontSwitch)
                listBytes.AddRange(BitConverter.GetBytes(value));
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, ushort value)
        {
            if (dontSwitch)
                listBytes.AddRange(BitConverter.GetBytes(value));
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, byte value)
        {
            if (dontSwitch)
                listBytes.AddRange(BitConverter.GetBytes(value));
            listBytes.Add(value);
        }

        public static void Align(this List<byte> listBytes, int alignment)
        {
            while (listBytes.Count % alignment != 0)
                listBytes.Add(0x33);
        }
    }
}