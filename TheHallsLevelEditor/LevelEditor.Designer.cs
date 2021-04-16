
namespace TheHallsLevelEditor
{
    partial class LevelEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelEditor));
            this.Level = new System.Windows.Forms.GroupBox();
            this.ObjectBox = new System.Windows.Forms.GroupBox();
            this.Save = new System.Windows.Forms.Button();
            this.Load = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ObjectBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Level
            // 
            this.Level.Location = new System.Drawing.Point(144, 22);
            this.Level.Margin = new System.Windows.Forms.Padding(1);
            this.Level.Name = "Level";
            this.Level.Padding = new System.Windows.Forms.Padding(1);
            this.Level.Size = new System.Drawing.Size(338, 346);
            this.Level.TabIndex = 0;
            this.Level.TabStop = false;
            this.Level.Text = "Level";
            // 
            // ObjectBox
            // 
            this.ObjectBox.Controls.Add(this.pictureBox1);
            this.ObjectBox.Location = new System.Drawing.Point(29, 52);
            this.ObjectBox.Name = "ObjectBox";
            this.ObjectBox.Size = new System.Drawing.Size(315, 525);
            this.ObjectBox.TabIndex = 1;
            this.ObjectBox.TabStop = false;
            this.ObjectBox.Text = "Objects";
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(29, 732);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(111, 52);
            this.Save.TabIndex = 0;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Load
            // 
            this.Load.Location = new System.Drawing.Point(29, 861);
            this.Load.Name = "Load";
            this.Load.Size = new System.Drawing.Size(111, 52);
            this.Load.TabIndex = 2;
            this.Load.Text = "Load";
            this.Load.UseVisualStyleBackColor = true;
            this.Load.Click += new System.EventHandler(this.Load_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(6, 115);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(239, 213);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // LevelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 429);
            this.Controls.Add(this.Load);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.ObjectBox);
            this.Controls.Add(this.Level);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "LevelEditor";
            this.Text = "LevelEditor";
            this.ObjectBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox Level;
        private System.Windows.Forms.GroupBox ObjectBox;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button Load;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}