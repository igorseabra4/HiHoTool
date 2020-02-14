using System;
using System.Collections.Generic;
using System.IO;
using static HiHoFile.Extensions;

namespace HiHoFile
{
    public class AssetEntry
    {
        public int totalDataSize; // includes padding
        public int relativeDataOffset;
        public int actualSize; // data size without padding
        public int unk0C;
        public ulong assetID;
        public uint assetType;
        public int unk1C;

        public byte[] data;
        public int absoluteDataOffset; // used only for replacement
        public int absoluteActualSizeOffset; // used only for replacement
        
        public AssetEntry(BinaryReader binaryReader, int localOffset)
        {
            totalDataSize = binaryReader.ReadInt32().Switch();
            relativeDataOffset = binaryReader.ReadInt32().Switch();
            absoluteActualSizeOffset = (int)binaryReader.BaseStream.Position;
            actualSize = binaryReader.ReadInt32().Switch();
            unk0C = binaryReader.ReadInt32().Switch();
            assetID = binaryReader.ReadUInt64().Switch();
            assetType = binaryReader.ReadUInt32().Switch();
            unk1C = binaryReader.ReadInt32().Switch();

            var position = binaryReader.BaseStream.Position;

            binaryReader.BaseStream.Position = localOffset + relativeDataOffset;

            absoluteDataOffset = (int)binaryReader.BaseStream.Position;
            data = binaryReader.ReadBytes(actualSize);

            binaryReader.BaseStream.Position = position;
        }

        public void SetListBytes(ref List<byte> listBytes)
        {
            throw new NotImplementedException();
        }
    }
}
