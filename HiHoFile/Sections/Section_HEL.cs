using System.Collections.Generic;
using System.IO;

namespace HiHoFile
{
    public class Section_HEL
    {
        private byte[] helData;

        public Section_HEL()
        {
            helData = new byte[0x800];
        }

        public Section_HEL(BinaryReader binaryReader)
        {
            helData = binaryReader.ReadBytes(0x800);
        }

        public void SetListBytes(ref List<byte> listBytes)
        {
            listBytes.AddRange(helData);
        }
    }
}
