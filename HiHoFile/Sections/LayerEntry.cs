using System;
using System.Collections.Generic;
using System.IO;

namespace HiHoFile
{
    public class LayerEntry
    {
        public string layerType;
        private int flags;
        private int unknown2;
        private int unknown3;
        private int unknown4;
        private int unknown5;
        private int unknown6;
        public int offsetToStartOfLayerDataDividedBy0x800;
        public int totalLayerSize;
        public int totalLayerSize2;
        private int unknown10;
        private int unknown11;
        private int unknown12;
        private int unknown13;
        private int offsetToLayerPart2;
        private int unknown15;

        public SubLayer subLayer;
        
        public LayerEntry(BinaryReader binaryReader, int localOffset)
        {
            layerType = new string(binaryReader.ReadChars(4));
            flags = binaryReader.ReadInt32().Switch();
            unknown2 = binaryReader.ReadInt32().Switch();
            unknown3 = binaryReader.ReadInt32().Switch();
            unknown4 = binaryReader.ReadInt32().Switch();
            unknown5 = binaryReader.ReadInt32().Switch();
            unknown6 = binaryReader.ReadInt32().Switch();
            offsetToStartOfLayerDataDividedBy0x800 = binaryReader.ReadInt32().Switch();
            totalLayerSize = binaryReader.ReadInt32().Switch();
            totalLayerSize2 = binaryReader.ReadInt32().Switch();
            unknown10 = binaryReader.ReadInt32().Switch();
            unknown11 = binaryReader.ReadInt32().Switch();
            unknown12 = binaryReader.ReadInt32().Switch();
            unknown13 = binaryReader.ReadInt32().Switch();
            offsetToLayerPart2 = binaryReader.ReadInt32().Switch();
            unknown15 = binaryReader.ReadInt32().Switch();

            var previousPosition = binaryReader.BaseStream.Position;

            binaryReader.BaseStream.Position = localOffset + offsetToLayerPart2;

            string subLayerType = new string(binaryReader.ReadChars(4));

            switch (subLayerType)
            {
                case "PSL\0":
                    subLayer = new SubLayer_PSL(binaryReader);
                    break;
                case "PSLD":
                    subLayer = new SubLayer_PSLD(binaryReader);
                    break;
                default:
                    throw new Exception();
            }

            binaryReader.BaseStream.Position = previousPosition;
        }
    }
}