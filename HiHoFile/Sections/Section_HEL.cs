using System.Collections.Generic;
using System.IO;

namespace HoFile
{
    public class Section_HEL
    {
        private byte[] helData;
        private Section_MAST sectionMast;

        public Section_HEL()
        {
            helData = new byte[0x800];
            sectionMast = new Section_MAST();
        }

        public Section_HEL(BinaryReader binaryReader)
        {
            helData = binaryReader.ReadBytes(0x800);
            sectionMast = new Section_MAST(binaryReader);
        }

        public void SetListBytes(ref List<byte> listBytes)
        {
            listBytes.AddRange(helData);
            sectionMast.SetListBytes(ref listBytes);
        }
    }
}
