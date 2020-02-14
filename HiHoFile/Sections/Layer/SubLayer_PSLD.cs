using System.Collections.Generic;
using System.IO;

namespace HiHoFile
{
    public class SubLayer_PSLD : SubLayer
    {
        public int amountOfEntries;
        public int unknown0;
        public int unknown1;
        public int unknown2;
        public int unknown3;
        public int unknown4;
        
        public SubLayer_PSLD(BinaryReader binaryReader) : base(binaryReader)
        {
            amountOfEntries = binaryReader.ReadInt32().Switch();
            unknown0 = binaryReader.ReadInt32().Switch();
            unknown1 = binaryReader.ReadInt32().Switch();
            unknown2 = binaryReader.ReadInt32().Switch();
            unknown3 = binaryReader.ReadInt32().Switch();
            unknown4 = binaryReader.ReadInt32().Switch();
        }

        public override void GetAssets(BinaryReader binaryReader, int localLayerStartOffset)
        {
            binaryReader.BaseStream.Position = localLayerStartOffset + 0x20 + amountOfEntries * 0x40;
        }
    }
}