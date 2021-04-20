﻿using System;
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
        private bool changes;
        

        /// <summary>
        /// Creates new level
        /// </summary>
        public LevelEditor()
        {
            levelMap = new PictureBox[30, 30];
            InitializeComponent();
            CreateTile();
        }

        /// <summary>
        /// Loads level
        /// </summary>
        /// <param name="file"></param>
        public LevelEditor(OpenFileDialog file)
        {
            levelMap = new PictureBox[30, 30];
            InitializeComponent();
            CreateTile();
            LoadFile(file);
        }

        /// <summary>
        /// creates tiles
        /// </summary>
        public void CreateTile()
        {
            int tileWidth = Level.Size.Width/10;
            int tileHeight = Level.Size.Height / 10;
            changes = false;
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    levelMap[i, j] = new PictureBox();
                    levelMap[i, j].Width = tileWidth;
                    levelMap[i, j].Height = tileHeight;
                    levelMap[i, j].BackColor = Color.White;
                    levelMap[i, j].Location = new Point((i * tileWidth) + 10, (j * tileHeight) + 20);
                    levelMap[i, j].Click += changeTile;
                    Level.Controls.Add(levelMap[i, j]);
                }
            }
        }

        /// <summary>
        /// Changes tile color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeTile(object sender, EventArgs e)
        {

            if (SpriteList.SelectedItems.Count > 0)
            {
                ListViewItem item = SpriteList.SelectedItems[0];
                Image img = item.ImageList.Images[item.ImageIndex];
                ((PictureBox)sender).Image = img;
            }


            if (changes == false)
            {
                Text = Text + " *";
                changes = true;
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
                for (int i = 0; i < 30; i++)
                {
                    for (int j = 0; j < 30; j++)
                    {
                        save.Write(levelMap[i, j].Image.ToString());
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
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    levelMap[i, j].Image = ;
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
    }
}
