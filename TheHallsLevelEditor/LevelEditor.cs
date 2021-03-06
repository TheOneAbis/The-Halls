using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TheHallsLevelEditor
{
    public partial class LevelEditor : Form
    {
        private PictureBox[,] levelMap;
        private int[,] mapImgIndices;
        private bool changes;
        private int noOfTiles = 20;
        

        /// <summary>
        /// Creates new level
        /// </summary>
        public LevelEditor()
        {
            levelMap = new PictureBox[30, 30];
            InitializeComponent();
            CreateTile(noOfTiles);
        }

        /// <summary>
        /// Loads level
        /// </summary>
        /// <param name="file"></param>
        public LevelEditor(OpenFileDialog file)
        {
            levelMap = new PictureBox[30, 30];
            InitializeComponent();
            CreateTile(noOfTiles);
            LoadFile(file);
        }

        /// <summary>
        /// creates tiles
        /// </summary>
        public void CreateTile(int tiles)
        {
            int tileWidth = Level.Size.Width/ tiles;
            int tileHeight = Level.Size.Width / tiles;
            mapImgIndices = new int[30, 30];
            changes = false;
            for (int i = 0; i < tiles; i++)
            {
                for (int j = 0; j < tiles; j++)
                {
                    levelMap[i, j] = new PictureBox();
                    levelMap[i, j].Width = tileWidth;
                    levelMap[i, j].Height = tileHeight;
                    levelMap[i, j].BackColor = Color.White;
                    levelMap[i, j].Location = new Point((i * tileWidth) + 10, (j * tileHeight) + 20);
                    levelMap[i, j].MouseClick += changeTile;
                    levelMap[i, j].MouseClick += PictureBox_RightClick;
                    levelMap[i, j].SizeMode = PictureBoxSizeMode.StretchImage;
                    levelMap[i, j].Image = SpriteList.Items[0].ImageList.Images[60];
                    Level.Controls.Add(levelMap[i, j]);
                    mapImgIndices[i, j] = 60;
                }
            }
        }

        /// <summary>
        /// Changes tile color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeTile(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (SpriteList.SelectedItems.Count > 0)
                {
                    ListViewItem item = SpriteList.SelectedItems[0];
                    Image img = item.ImageList.Images[item.ImageIndex];
                    ((PictureBox)sender).Image = img;

                    for (int i = 0; i < noOfTiles; i++)
                    {
                        for (int j = 0; j < noOfTiles; j++)
                        {
                            if (levelMap[i, j] == sender)
                            {
                                mapImgIndices[i, j] = item.ImageIndex;
                            }
                        }
                    }
                }

                if (changes == false)
                {
                    Text = Text + " *";
                    changes = true;
                }
            }
        }

        /// <summary>
        /// Saves room files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Room Files| *.room";
            saveFile.Title = "Save a Level file";
            if (saveFile.ShowDialog() == DialogResult.OK && saveFile.FileName != "")
            {
                BinaryWriter save = new BinaryWriter(File.OpenWrite(saveFile.FileName));
                for (int i = 0; i < noOfTiles; i++)
                {
                    for (int j = 0; j < noOfTiles; j++)
                    {
                        save.Write(mapImgIndices[i, j]);
                    }
                }
                save.Close();

                //Changes title
                string title = saveFile.FileName;
                string[] titleArr = title.Split('\\');
                string[] fileArr = titleArr.Last().Split('\\');
                string fileName = fileArr.Last().ToString();
                Text = "Level Editor - " + fileName;
                changes = false;

                //Displays message box
                MessageBox.Show("File saved successfully", "File Save");
            }
        }

        /// <summary>
        /// Loads file
        /// </summary>
        private void LoadFile(OpenFileDialog file)
        {
            BinaryReader load = new BinaryReader(file.OpenFile());
            for (int i = 0; i < noOfTiles; i++)
            {
                for (int j = 0; j < noOfTiles; j++)
                {

                    mapImgIndices[i, j] = load.ReadInt32();
                    if (mapImgIndices[i, j] != -1)
                    {
                        levelMap[i, j].Image = SpriteList.SmallImageList.Images[mapImgIndices[i, j]];
                    }
                    else
                    {
                        levelMap[i, j].Image = null;
                    }
                }
            }
            load.Close();
            MessageBox.Show("File Loaded successfully", "File Load");
        }

        /// <summary>
        /// Loads file upon click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Click(object sender, EventArgs e)
        {
            OpenFileDialog loadFile = new OpenFileDialog();
            loadFile.Title = "Open a Room File";
            loadFile.Filter = "Room Files| *.room";
            if (loadFile.ShowDialog() == DialogResult.OK)
            {
                LoadFile(loadFile);
            }
        }

        /// <summary>
        /// Erases the picturebox, setting it to no image and 
        /// the corresponding image index to -1
        /// </summary>
        /// <param name="sender">the clicked picturebox</param>
        /// <param name="e">Mouse right-click</param>
        private void PictureBox_RightClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) 
            {
                for (int i = 0; i < noOfTiles; i++)
                {
                    for (int j = 0; j < noOfTiles; j++)
                    {
                        if (levelMap[i, j] == ((PictureBox)sender))
                        {
                            ((PictureBox)sender).Image = null;
                            mapImgIndices[i, j] = -1;
                        }
                    }
                }
            }
        }

        private void SpriteList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
