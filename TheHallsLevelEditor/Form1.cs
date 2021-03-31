using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheHallsLevelEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates new level on click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newRoom_Click(object sender, EventArgs e)
        {
            LevelEditor level = new LevelEditor();
            level.ShowDialog();
        }

        /// <summary>
        /// Loads a level on click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadRoom_Click(object sender, EventArgs e)
        {
            OpenFileDialog loadFile = new OpenFileDialog();
            loadFile.Title = "Open a Room File";
            loadFile.Filter = "Room Files| *.room";
            if (loadFile.ShowDialog() == DialogResult.OK)
            {
                LevelEditor level = new LevelEditor(loadFile);
                level.ShowDialog();
            }
        }
    }
}
