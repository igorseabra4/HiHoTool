using System;
using System.Collections.Generic;
using System.IO;
using static HoFile.Functions;

namespace HoFile
{
    public class Section_SECT
    {
        private List<HoLayer> layers;

        public Section_SECT()
        {
        }

        public Section_SECT(BinaryReader binaryReader)
        {
            binaryReader.BaseStream.Position += 4;
            int layerCount = Switch(binaryReader.ReadInt32());
            binaryReader.BaseStream.Position += 0x18;

            layers = new List<HoLayer>(layerCount);
            for (int i = 0; i < layerCount; i++)
            {
                string layerType = new string(binaryReader.ReadChars(4));
                switch (layerType)
                {
                    case "P   ":
                        layers.Add(new HoLayer_P(binaryReader));
                        break;
                    case "PD  ":
                        layers.Add(new HoLayer_PD(binaryReader));
                        break;
                    case "PTEX":
                        layers.Add(new HoLayer_PTEX(binaryReader));
                        break;
                    case "PFST":
                        layers.Add(new HoLayer_PFST(binaryReader));
                        break;
                }
            }

            ReadString(binaryReader);

            while (binaryReader.BaseStream.Position % 0x20 != 0)
                binaryReader.BaseStream.Position++;

            for (int i = 0; i < layerCount; i++)
                layers[i].SecondPartSetup(binaryReader);
        }

        public void SetListBytes(ref List<byte> listBytes)
        {
            throw new NotImplementedException();
        }
    }
}