using System;
using System.Collections.Generic;
using System.IO;
using static HoFile.Functions;

namespace HoFile
{
    public class Section_AHDR
    {
        private int totalDataSize; // includes padding
        private int relativeDataOffset;
        private int actualSize; // data size without padding
        private int unk0C;
        private ulong assetID;
        private AssetType assetType;
        private int unk1C;

        private byte[] data;

        public Section_AHDR() { }

        public Section_AHDR(BinaryReader binaryReader)
        {
            totalDataSize = Switch(binaryReader.ReadInt32());
            relativeDataOffset = Switch(binaryReader.ReadInt32());
            actualSize = Switch(binaryReader.ReadInt32());
            unk0C = Switch(binaryReader.ReadInt32());
            assetID = Switch(binaryReader.ReadUInt64());
            assetType = AssetTypeFromHash(Switch(binaryReader.ReadUInt32()));
            unk1C = Switch(binaryReader.ReadInt32());

            throw new NotImplementedException();
        }

        public void SetListBytes(ref List<byte> listBytes)
        {
            throw new NotImplementedException();
        }
    }
}
