using System.Collections.Generic;
using System.IO;

namespace HiHoFile
{
    public enum PSLSectionType
    {
        AssetList = 0,
        AssetData = 1,
        Padding = 2
    }

    public class PSLSection
    {
        public PSLSectionType type;
        public int startOffset;
        public int size;
        public int minusOne;

        public PSLSection(BinaryReader binaryReader)
        {
            type = (PSLSectionType)binaryReader.ReadInt32().Switch();
            startOffset = binaryReader.ReadInt32().Switch();
            size = binaryReader.ReadInt32().Switch();
            minusOne = binaryReader.ReadInt32().Switch();
        }
    }

    public class SubLayer_PSL : SubLayer
    {
        public int amountOfSubSections;
        public int unknown;

        public List<PSLSection> subSections;
        public List<AssetEntry> assets;

        public SubLayer_PSL(BinaryReader binaryReader) : base(binaryReader)
        {
            amountOfSubSections = binaryReader.ReadInt32().Switch();
            unknown = binaryReader.ReadInt32().Switch();

            subSections = new List<PSLSection>();

            for (int i = 0; i < amountOfSubSections; i++)
                subSections.Add(new PSLSection(binaryReader));
        }

        public override void GetAssets(BinaryReader binaryReader, int localLayerStartOffset)
        {
            assets = new List<AssetEntry>();

            foreach (PSLSection sec in subSections)
                if (sec.type == PSLSectionType.AssetList)
                {
                    binaryReader.BaseStream.Position = localLayerStartOffset + sec.startOffset;
                    int assetCount = binaryReader.ReadInt32().Switch();
                    binaryReader.BaseStream.Position += 0x1C;

                    for (int i = 0; i < assetCount; i++)
                        assets.Add(new AssetEntry(binaryReader, localLayerStartOffset));
                }
        }
    }
}