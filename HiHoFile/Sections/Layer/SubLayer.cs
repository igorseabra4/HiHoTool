using System.IO;

namespace HiHoFile
{
    public abstract class SubLayer
    {
        public int sectionSize;

        public SubLayer(BinaryReader binaryReader)
        {
            sectionSize = binaryReader.ReadInt32().Switch();
        }

        public abstract void GetAssets(BinaryReader binaryReader, int offset);
    }
}