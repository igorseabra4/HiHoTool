using System;
using System.Collections.Generic;
using System.IO;
using static HoFile.Functions;

namespace HoFile
{
    public class Section_MAST
    {
        private int sectSize;
        private string filePath;
        private Section_SECT sectionSect;

        public Section_MAST() { }

        public Section_MAST(BinaryReader binaryReader)
        {
            binaryReader.BaseStream.Position += 0x40;
            sectSize = Switch(binaryReader.ReadInt32());
            binaryReader.BaseStream.Position += 0x1C;
            filePath = ReadString(binaryReader);
            binaryReader.BaseStream.Position = 0x1000;

            sectionSect = new Section_SECT(binaryReader);
        }

        public void SetListBytes(ref List<byte> listBytes)
        {
            throw new NotImplementedException();
        }
    }
}
