using System;
using System.Collections.Generic;
using System.IO;
using static HiHoFile.Extensions;

namespace HiHoFile
{
    public class Section_MAST
    {
        public Section_SECT sectionSect2;

        public Section_MAST() { }
        
        public Section_MAST(BinaryReader binaryReader)
        {
            int mast = binaryReader.ReadInt32().Switch();
            int unknown1 = binaryReader.ReadInt32().Switch();
            int unknown2 = binaryReader.ReadInt32().Switch();
            int unknown3 = binaryReader.ReadInt32().Switch();
            int unknown4 = binaryReader.ReadInt32().Switch();
            int unknown5 = binaryReader.ReadInt32().Switch();
            int unknown6 = binaryReader.ReadInt32().Switch();
            int unknown7 = binaryReader.ReadInt32().Switch();

            int sect1Start = (int)binaryReader.BaseStream.Position;
            int sect = binaryReader.ReadInt32().Switch();
            int sect1Unknown1 = binaryReader.ReadInt32().Switch();
            int sect1Unknown2 = binaryReader.ReadInt32().Switch();
            int sect1Unknown3 = binaryReader.ReadInt32().Switch();
            int sect1Size = binaryReader.ReadInt32().Switch();
            int sect1Unknown5 = binaryReader.ReadInt32().Switch();
            int sect1Unknown6 = binaryReader.ReadInt32().Switch();
            int sect2OffsetDividedBy0x800 = binaryReader.ReadInt32().Switch();
            int sect2Size1 = binaryReader.ReadInt32().Switch();
            int sect2Size2 = binaryReader.ReadInt32().Switch();
            int sect1Unknown10 = binaryReader.ReadInt32().Switch();
            int sect1Unknown11 = binaryReader.ReadInt32().Switch();
            int sect1Unknown12 = binaryReader.ReadInt32().Switch();
            int sect1Unknown13 = binaryReader.ReadInt32().Switch();
            int sect1Unknown14 = binaryReader.ReadInt32().Switch();
            int sect1Unknown15 = binaryReader.ReadInt32().Switch();

            binaryReader.BaseStream.Position = sect2OffsetDividedBy0x800 * 0x800;

            sectionSect2 = new Section_SECT(binaryReader);
        }

        public void SetListBytes(ref List<byte> listBytes)
        {
            throw new NotImplementedException();
        }
    }
}
