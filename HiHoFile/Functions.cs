﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HoFile
{
    public static class Functions
    {
        public static void SendMessage()
        {
            Console.WriteLine();
        }

        public static void SendMessage(string message)
        {
            Console.WriteLine(message);
        }

        public static int Switch(int value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
        }

        public static uint Switch(uint value)
        {
            return BitConverter.ToUInt32(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
        }

        public static long Switch(long value)
        {
            return BitConverter.ToInt64(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
        }

        public static ulong Switch(ulong value)
        {
            return BitConverter.ToUInt64(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
        }

        public static float Switch(float value)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
        }

        public static AssetType AssetTypeFromHash(uint hash)
        {
            foreach (AssetType type in Enum.GetValues(typeof(AssetType)))
                if (BKDRHash(type.ToString()).Equals(hash))
                    return type;

            throw new ArgumentException("Unknown asset type found: " + hash.ToString("X8"));
        }

        public static uint BKDRHash(string str)
        {
            str = str.ToUpper();
            uint seed = 131;
            uint hash = 0;
            int length = str.Length;
            
            for (int i = 0; i < length; i++)
                hash = (hash * seed) + str[i];

            return hash;
        }

        public static string ReadString(BinaryReader binaryReader)
        {
            List<char> charList = new List<char>();
            do charList.Add((char)binaryReader.ReadByte());
            while (charList.Last() != '\0');
            charList.Remove('\0');
            
            return new string(charList.ToArray());
        }

        public static void AddString(this List<byte> listBytes, string writeString)
        {
            foreach (char i in writeString)
                listBytes.Add((byte)i);
            listBytes.Add(0);
        }

        public static void AddBigEndian(this List<byte> listBytes, float value)
        {
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, int value)
        {
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, uint value)
        {
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, short value)
        {
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, ushort value)
        {
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, byte value)
        {
            listBytes.Add(value);
        }

        public static void Align(this List<byte> listBytes, int alignment)
        {
            while (listBytes.Count % alignment != 0)
                listBytes.Add(0x33);
        }

        public static HoSection[] HipFileToHipArray(string fileName)
        {
            List<HoSection> hipFile = new List<HoSection>(4);

            BinaryReader binaryReader = new BinaryReader(new FileStream(fileName, FileMode.Open));

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                hipFile.Add(new Section_HEL(binaryReader));
            }

            binaryReader.Close();
            
            return hipFile.ToArray();
        }

        public static byte[] HipArrayToFile(HoSection[] hipFile)
        {
            List<byte> list = new List<byte>();
            foreach (HoSection i in hipFile)
                i.SetListBytes(ref list);

            return list.ToArray();
        }

        public static void HipArrayToIni(HoSection[] hipFile, string unpackFolder, bool multiFolder, bool alphabetical)
        {
            Directory.CreateDirectory(unpackFolder);
            
            string fileName = Path.Combine(unpackFolder, "Settings.ini");

            StreamWriter INIWriter = new StreamWriter(new FileStream(fileName, FileMode.Create));

            foreach (HoSection i in hipFile)
            {
                if (i is Section_PACK PACK)
                {
                    INIWriter.WriteLine("PACK.PVER=" + PACK.PVER.subVersion.ToString() + v1s + PACK.PVER.clientVersion.ToString() + v1s + PACK.PVER.compatible.ToString());
                    INIWriter.WriteLine("PACK.PFLG=" + PACK.PFLG.flags.ToString());
                    INIWriter.WriteLine("PACK.PCRT=" + PACK.PCRT.fileDate.ToString() + v1s + PACK.PCRT.dateString);
                    if (currentGame == Game.BFBB | currentGame == Game.Incredibles)
                    {
                        INIWriter.WriteLine("PACK.PLAT.Target=" + PACK.PLAT.helString);
                        INIWriter.WriteLine("PACK.PLAT.RegionFormat=" + PACK.PLAT.regionFormat);
                        INIWriter.WriteLine("PACK.PLAT.Language=" + PACK.PLAT.language);
                        INIWriter.WriteLine("PACK.PLAT.TargetGame=" + PACK.PLAT.targetGame);
                        if (currentGame == Game.BFBB)
                            INIWriter.WriteLine("PACK.PLAT.TargetPlatformName=" + PACK.PLAT.targetPlatformName);
                    }
                    else if (currentGame != Game.Scooby) throw new Exception("Unknown game");

                    INIWriter.WriteLine();
                }
                else if (i is Section_DICT DICT)
                {
                    ExtractAssetsToFolders(unpackFolder, ref DICT, multiFolder);

                    INIWriter.WriteLine("DICT.ATOC.AINF=" + DICT.ATOC.AINF.value.ToString());
                    INIWriter.WriteLine("DICT.LTOC.LINF=" + DICT.LTOC.LINF.value.ToString());
                    INIWriter.WriteLine();

                    Dictionary<uint, Section_AHDR> ahdrDictionary = new Dictionary<uint, Section_AHDR>();

                    foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                        ahdrDictionary.Add(AHDR.assetID, AHDR);

                    foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
                    {
                        if (currentGame == Game.Incredibles)
                            INIWriter.WriteLine("LayerType=" + LHDR.layerType + " " + ((LayerType_TSSM)LHDR.layerType).ToString());
                        else
                            INIWriter.WriteLine("LayerType=" + LHDR.layerType + " " + ((LayerType_BFBB)LHDR.layerType).ToString());

                        if (LHDR.assetIDlist.Count == 0)
                            INIWriter.WriteLine("AssetAmount=0");
                        INIWriter.WriteLine("LHDR.LDBG=" + LHDR.LDBG.value.ToString());

                        List<Section_AHDR> ahdrList = new List<Section_AHDR>(LHDR.assetIDlist.Count);
                        foreach (uint j in LHDR.assetIDlist)
                            ahdrList.Add(ahdrDictionary[j]);

                        if (alphabetical)
                            ahdrList = ahdrList.OrderBy(ahdr => ahdr.ADBG.assetName).ToList();

                        foreach (Section_AHDR AHDR in ahdrList)
                        {
                            string assetToString = AHDR.assetID.ToString("X8") + v1s + AHDR.assetType + v1s + ((int)AHDR.flags).ToString() + v1s + AHDR.ADBG.alignment.ToString() + v1s + AHDR.ADBG.assetName.Replace(v1s, '_') + v1s + AHDR.ADBG.assetFileName + v1s + AHDR.ADBG.checksum.ToString("X8");
                            INIWriter.WriteLine("Asset=" + assetToString);
                        }

                        INIWriter.WriteLine("EndLayer");

                        INIWriter.WriteLine();
                    }
                }
                else if (i is Section_STRM STRM)
                {
                    INIWriter.WriteLine("STRM.DHDR=" + STRM.DHDR.value.ToString());
                    INIWriter.WriteLine();
                }
            }

            INIWriter.Close();
        }

        public static void HipArrayToJson(HoSection[] hipFile, string unpackFolder, bool multiFolder)
        {
            Directory.CreateDirectory(unpackFolder);

            switch (currentGame)
            {
                case Game.Scooby:
                    SendMessage("Game: Scooby-Doo: Night of 100 Frights");
                    break;
                case Game.Incredibles:
                    SendMessage("Game: The Incredibles, The Spongebob Squarepants Movie, or Rise of the Underminer");
                    break;
                case Game.BFBB:
                    SendMessage("Game: Spongebob Squarepants: Battle For Bikini Bottom");
                    break;
                default:
                    SendMessage("Error: Unknown game.");
                    break;
            }
            
            HipSerializer serializer = new HipSerializer()
            {
                currentGame = currentGame
            };
            
            foreach (HoSection i in hipFile)
            {
                if (i is Section_PACK PACK)
                {
                    serializer.PACK_PVER_subVersion = PACK.PVER.subVersion;
                    serializer.PACK_PVER_clientVersion = PACK.PVER.clientVersion;
                    serializer.PACK_PVER_compatible = PACK.PVER.compatible;
                    serializer.PACK_PFLG_flags = PACK.PFLG.flags;
                    serializer.PACK_PCRT_fileDate = PACK.PCRT.fileDate;
                    serializer.PACK_PCRT_dateString = PACK.PCRT.dateString;

                    if (currentGame == Game.BFBB | currentGame == Game.Incredibles)
                    {
                        serializer.PACK_PLAT_TargetPlatform = PACK.PLAT.helString;
                        serializer.PACK_PLAT_RegionFormat = PACK.PLAT.regionFormat;
                        serializer.PACK_PLAT_Language = PACK.PLAT.language;
                        serializer.PACK_PLAT_TargetGame = PACK.PLAT.targetGame;

                        if (currentGame == Game.BFBB)
                            serializer.PACK_PLAT_TargetPlatformName = PACK.PLAT.targetPlatformName;
                    }
                    else if (currentGame != Game.Scooby)
                        throw new Exception("Unknown game");
                }
                else if (i is Section_DICT DICT)
                {
                    ExtractAssetsToFolders(unpackFolder, ref DICT, multiFolder);

                    serializer.DICT_ATOC_AINF_value = DICT.ATOC.AINF.value;
                    serializer.DICT_LTOC_LINF_value = DICT.LTOC.LINF.value;

                    Dictionary<uint, AssetSerializer> assetDictionary = new Dictionary<uint, AssetSerializer>();

                    foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                        assetDictionary.Add(AHDR.assetID, new AssetSerializer()
                        {
                            assetType = AHDR.assetType,
                            flags = AHDR.flags,
                            ADBG_alignment = AHDR.ADBG.alignment,
                            ADBG_assetName = AHDR.ADBG.assetName,
                            ADBG_assetFileName = AHDR.ADBG.assetFileName,
                            checksum = AHDR.ADBG.checksum
                        });

                    serializer.layers = new List<LayerSerializer>();
                    foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
                    {
                        Dictionary<uint, AssetSerializer> assets = new Dictionary<uint, AssetSerializer>();

                        foreach (uint j in LHDR.assetIDlist)
                            assets.Add(j, assetDictionary[j]);

                        serializer.layers.Add(new LayerSerializer()
                        {
                            layerType = LHDR.layerType,
                            LHDR_LDBG_value = LHDR.LDBG.value,
                            assets = assets
                        });
                    }
                }
                else if (i is Section_STRM STRM)
                {
                    serializer.SRTM_DHDR_value = STRM.DHDR.value;
                }
            }

            throw new NotImplementedException("Unable to create Settings.json");
            //File.WriteAllText(Path.Combine(unpackFolder, "Settings.json"), JsonConvert.SerializeObject(serializer, Formatting.Indented));
        }

        private static void ExtractAssetsToFolders(string unpackFolder, ref Section_DICT DICT, bool multiFolder)
        {
            string directoryToUnpack = Path.Combine(unpackFolder, "files");

            foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
            {
                if (multiFolder)
                    directoryToUnpack = Path.Combine(unpackFolder, AHDR.assetType.ToString());

                if (!Directory.Exists(directoryToUnpack))
                    Directory.CreateDirectory(directoryToUnpack);

                string assetFileName = "[" + AHDR.assetID.ToString("X8") + "] " + AHDR.ADBG.assetName;

                foreach (char c in Path.GetInvalidFileNameChars())
                {
                    assetFileName = assetFileName.Replace(c, '_');
                }

                File.WriteAllBytes(Path.Combine(directoryToUnpack, assetFileName), AHDR.data);
            }
        }

        private static readonly char v1s = ';';

        public static HoSection[] IniToHipArray(string INIFile)
        {
            // Let's load the data from the INI first

            string[] INI = File.ReadAllLines(INIFile);
            char sep = v1s;
                                 
            foreach (string s in INI)
            {
                
            }

            return GetFilesData(INIFile, ref HIPA, ref PACK, ref DICT, ref STRM);
        }
       
        private static HoSection[] GetFilesData(string INIFile, ref Section_HEL HEL)
        {
            // Let's get the data from the files now and put them in a dictionary
            Dictionary<uint, byte[]> assetDataDictionary = new Dictionary<uint, byte[]>();

            foreach (string f in Directory.GetDirectories(Path.GetDirectoryName(INIFile)))
                foreach (string i in Directory.GetFiles(f))
                {
                    byte[] file = File.ReadAllBytes(i);
                    try
                    {
                        assetDataDictionary.Add(Convert.ToUInt32(Path.GetFileName(i).Substring(1, 8), 16), file);
                    }
                    catch (Exception e)
                    {
                        SendMessage("Error importing asset " + Path.GetFileName(i) + ": " + e.Message);
                    }
                }

            // Now send all the data to the function that'll put the data in the AHDR then fill the STRM section with that
            return SetupStream(ref HIPA, ref PACK, ref DICT, ref STRM, false, assetDataDictionary);
        }

        public static Section_HEL SetupStream(ref Section_HEL HEL, bool alreadyHasData = true, Dictionary<uint, byte[]> assetDataDictionary = null)
        {
            // Let's go through each layer.
            foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
            {
                // Sort the LDBG asset IDs. The AHDR data will then be written in this order.
                LHDR.assetIDlist = LHDR.assetIDlist.OrderBy(i => i).ToList();

                for (int i = 0; i < LHDR.assetIDlist.Count; i++)
                {
                    Section_AHDR AHDR = assetDictionary[LHDR.assetIDlist[i]];

                    // Does the AHDR section already have the asset data, or should we get it from the dictionary?
                    // AHDRs from IP will already have data, but the ones from the INI builder won't!
                    if (!alreadyHasData) 
                    {
                        if (!assetDataDictionary.Keys.Contains(AHDR.assetID))
                        {
                            SendMessage($"Error: asset with ID [{AHDR.assetID.ToString("X8")}] was not found. The asset will be removed from the archive.");
                            DICT.ATOC.AHDRList.Remove(AHDR);
                            LHDR.assetIDlist.Remove(AHDR.assetID);
                            i--;
                            continue;
                        }
                        AHDR.data = assetDataDictionary[AHDR.assetID];
                    }

                    // Set stream dependant AHDR data...
                    AHDR.fileOffset = newStream.Count + STRM.DPAK.globalRelativeStartOffset;
                    AHDR.fileSize = AHDR.data.Length;

                    // And add the data to the stream.
                    newStream.AddRange(AHDR.data);

                    // Calculate alignment data which I don't understand, but hey it works.
                    AHDR.plusValue = 0;

                    int alignment = 16;
                    if (currentGame == Game.BFBB)
                    {
                        if (AHDR.assetType == AssetType.CSN |
                            AHDR.assetType == AssetType.SND |
                            AHDR.assetType == AssetType.SNDS)
                            alignment = 32;
                        else if (AHDR.assetType == AssetType.CRDT)
                            alignment = 4;
                    }

                    int value = AHDR.fileSize % alignment;
                    if (value != 0)
                        AHDR.plusValue = alignment - value;
                    for (int j = 0; j < AHDR.plusValue; j++)
                        newStream.Add(0x33);
                }

                while ((newStream.Count + STRM.DPAK.globalRelativeStartOffset) % 0x20 != 0)
                    newStream.Add(0x33);
            }
            
            // Assign list as stream! We're done with the worst part.
            STRM.DPAK.data = newStream.ToArray();

            // I'll create a new PCNT, because I'm sure you'll forget to do so.
            PACK.PCNT = new Section_PCNT(DICT.ATOC.AHDRList.Count, DICT.LTOC.LHDRList.Count, LargestSourceFileAsset, LargestLayer, LargestSourceVirtualAsset);

            // We're done!
            return new HoSection[] { HIPA, PACK, DICT, STRM };
        }
    }
}