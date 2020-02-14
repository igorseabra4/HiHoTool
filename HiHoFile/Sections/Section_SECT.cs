using System;
using System.Collections.Generic;
using System.IO;
using static HiHoFile.Extensions;

namespace HiHoFile
{
    public class Section_SECT
    {
        public List<LayerEntry> layers;

        public Section_SECT()
        {
        }

        public Section_SECT(BinaryReader binaryReader)
        {
            int localOffset = (int)binaryReader.BaseStream.Position;
            int unknown1 = binaryReader.ReadInt32().Switch();
            int layerCount = binaryReader.ReadInt32().Switch();
            int unknown2 = binaryReader.ReadInt32().Switch();
            int sizeOfLayerData = binaryReader.ReadInt32().Switch();
            int unknown3 = binaryReader.ReadInt32().Switch();
            int secondLayerDataOffset = binaryReader.ReadInt32().Switch();
            int sizeOfSecondLayerData = binaryReader.ReadInt32().Switch();
            int unknown4 = binaryReader.ReadInt32().Switch();

            layers = new List<LayerEntry>(layerCount);
            for (int i = 0; i < layerCount; i++)
                layers.Add(new LayerEntry(binaryReader, localOffset));

            ReadString(binaryReader);

            while (binaryReader.BaseStream.Position % 0x10 != 0)
                binaryReader.BaseStream.Position++;

            binaryReader.BaseStream.Position = localOffset + secondLayerDataOffset + sizeOfSecondLayerData;
            
            foreach (LayerEntry layer in layers)
            {
                layer.subLayer.GetAssets(binaryReader, layer.offsetToStartOfLayerDataDividedBy0x800 * 0x800);
            }
        }

        public void SetListBytes(ref List<byte> listBytes)
        {
            throw new NotImplementedException();
        }
    }
}