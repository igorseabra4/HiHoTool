using System;
using System.IO;
using System.Windows.Forms;
using HiHoFile;
using static HiHoFile.Extensions;

namespace HiHoTool
{
    public partial class HiHoToolForm : Form
    {
        public HiHoToolForm()
        {
            InitializeComponent();
        }

        HoFile hoFile;
        string fileName;
        byte[] editableHoFile;

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    editableHoFile = File.ReadAllBytes(openFile.FileName);
                    hoFile = new HoFile(openFile.FileName);
                    fileName = openFile.FileName;
                    saveToolStripMenuItem.Enabled = true;
                    saveAsToolStripMenuItem.Enabled = true;
                    FillLayerListBox();
                }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            File.WriteAllBytes(fileName, editableHoFile);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFile = new SaveFileDialog()
            {
                FileName = fileName
            })
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    fileName = saveFile.FileName;
                    saveToolStripMenuItem_Click(sender, e);
                }
        }

        private void FillLayerListBox()
        {
            label1.Text = "File: " + fileName;
            listBoxLayers.Items.Clear();
            foreach (var layer in hoFile.MAST.sectionSect2.layers)
                listBoxLayers.Items.Add(layer.layerType.Replace("\0", ""));
        }

        private void listBoxLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxAssets.BeginUpdate();
            listBoxAssets.Items.Clear();
            if (listBoxLayers.SelectedIndex > -1 && listBoxLayers.SelectedIndex < hoFile.MAST.sectionSect2.layers.Count)
            {
                if (hoFile.MAST.sectionSect2.layers[listBoxLayers.SelectedIndex].subLayer is SubLayer_PSL psl)
                {
                    foreach (var asset in psl.assets)
                    {
                        string type;
                        if (Enum.IsDefined(typeof(AssetTypeHashed), asset.assetType))
                            type = ((AssetTypeHashed)asset.assetType).ToString();
                        else
                            type = asset.assetType.ToString("X8");

                        string name = $"[{type}] [{asset.assetID.ToString("X16")}]";
                        listBoxAssets.Items.Add(name);
                    }
                }
            }
            listBoxAssets.EndUpdate();

            listBoxAssets_SelectedIndexChanged(sender, e);
        }

        private void buttonExtractAll_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                    hoFile.ExtractAssetsToFolders(folderBrowser.SelectedPath, true);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            dontSwitch = checkBox1.Checked;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("HiHoTool v0.1 is a Heavy Iron Studios .HO archive viewer/editor by igorseabra4.");
        }

        private void buttonExtract_Click(object sender, EventArgs e)
        {
            ulong assetID = GetSelectedAssetID();

            if (assetID != 0)
                if (listBoxLayers.SelectedIndex > -1 && listBoxLayers.SelectedIndex < hoFile.MAST.sectionSect2.layers.Count)
                    if (hoFile.MAST.sectionSect2.layers[listBoxLayers.SelectedIndex].subLayer is SubLayer_PSL psl)
                        if (listBoxAssets.SelectedIndex > -1 && listBoxAssets.SelectedIndex < psl.assets.Count)
                            using (SaveFileDialog saveFile = new SaveFileDialog()
                            {
                                FileName = psl.assets[listBoxAssets.SelectedIndex].assetID.ToString("X16")
                            })
                                if (saveFile.ShowDialog() == DialogResult.OK)
                                    File.WriteAllBytes(saveFile.FileName, psl.assets[listBoxAssets.SelectedIndex].data);
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            ulong assetID = GetSelectedAssetID();

            if (assetID != 0)
                if (listBoxLayers.SelectedIndex > -1 && listBoxLayers.SelectedIndex < hoFile.MAST.sectionSect2.layers.Count)
                    if (hoFile.MAST.sectionSect2.layers[listBoxLayers.SelectedIndex].subLayer is SubLayer_PSL psl)
                        if (listBoxAssets.SelectedIndex > -1 && listBoxAssets.SelectedIndex < psl.assets.Count)
                            using (OpenFileDialog openFile = new OpenFileDialog()
                            {
                                Title = "Choose a file to replace."
                            })
                                if (openFile.ShowDialog() == DialogResult.OK)
                                {
                                    byte[] file = File.ReadAllBytes(openFile.FileName);
                                    if (file.Length <= psl.assets[listBoxAssets.SelectedIndex].totalDataSize)
                                    {
                                        psl.assets[listBoxAssets.SelectedIndex].data = file;
                                        ReplaceInEditableArray(psl.assets[listBoxAssets.SelectedIndex].absoluteDataOffset, file, psl.assets[listBoxAssets.SelectedIndex].totalDataSize);

                                        psl.assets[listBoxAssets.SelectedIndex].actualSize = file.Length;
                                        WriteNewSizeInEditableArray(psl.assets[listBoxAssets.SelectedIndex].absoluteActualSizeOffset, file.Length);

                                        listBoxAssets_SelectedIndexChanged(sender, e);
                                    }
                                    else
                                    MessageBox.Show($"Please choose a file with at most the maximum size of the asset you want to replace.\n" +
                                        $"Maximum asset size: {psl.assets[listBoxAssets.SelectedIndex].totalDataSize} bytes\n" +
                                        $"Size of your file: {file.Length} bytes");
                                }
        }
        
        private ulong GetSelectedAssetID()
        {
            if (listBoxAssets.SelectedItem != null)
                return Convert.ToUInt64(listBoxAssets.SelectedItem.ToString().Substring(listBoxAssets.SelectedItem.ToString().LastIndexOf("[") + 1, 16), 16);

            return 0;
        }

        private void ReplaceInEditableArray(int offset, byte[] file, int maxSize)
        {
            for (int i = 0; i < file.Length; i++)
                editableHoFile[offset + i] = file[i];
            for (int i = file.Length; i < maxSize; i++)
                editableHoFile[offset + i] = 0x33;
        }

        private void WriteNewSizeInEditableArray(int offset, int length)
        {
            byte[] bytes = BitConverter.GetBytes(length.Switch());
            for (int i = 0; i < bytes.Length; i++)
                editableHoFile[offset + i] = bytes[i];
        }

        private void listBoxAssets_SelectedIndexChanged(object sender, EventArgs e)
        {
            label2.Text = "";
            ulong assetID = GetSelectedAssetID();

            if (assetID != 0)
                if (listBoxLayers.SelectedIndex > -1 && listBoxLayers.SelectedIndex < hoFile.MAST.sectionSect2.layers.Count)
                    if (hoFile.MAST.sectionSect2.layers[listBoxLayers.SelectedIndex].subLayer is SubLayer_PSL psl)
                        if (listBoxAssets.SelectedIndex > -1 && listBoxAssets.SelectedIndex < psl.assets.Count)
                            label2.Text = "Size: " + psl.assets[listBoxAssets.SelectedIndex].actualSize.ToString() + "\nMax Size: " + psl.assets[listBoxAssets.SelectedIndex].totalDataSize.ToString();
        }

        private void buttonDestroy_Click(object sender, EventArgs e)
        {
            foreach (var layer in hoFile.MAST.sectionSect2.layers)
                if (layer.subLayer is SubLayer_PSL psl)                
                    foreach (var asset in psl.assets)
                        for (int i = 0; i < asset.actualSize; i++)
                            editableHoFile[asset.absoluteDataOffset + i] = 0;
        }
    }
}
