
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
            this.Level = new System.Windows.Forms.GroupBox();
            this.ObjectBox = new System.Windows.Forms.GroupBox();
            this.Save = new System.Windows.Forms.Button();
            this.Load = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Level
            // 
            this.Level.Location = new System.Drawing.Point(385, 52);
            this.Level.Name = "Level";
            this.Level.Size = new System.Drawing.Size(901, 826);
            this.Level.TabIndex = 0;
            this.Level.TabStop = false;
            this.Level.Text = "Level";
            // 
            // ObjectBox
            // 
            this.ObjectBox.Location = new System.Drawing.Point(29, 103);
            this.ObjectBox.Name = "ObjectBox";
            this.ObjectBox.Size = new System.Drawing.Size(315, 559);
            this.ObjectBox.TabIndex = 1;
            this.ObjectBox.TabStop = false;
            this.ObjectBox.Text = "Objects";
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(29, 677);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(296, 123);
            this.Save.TabIndex = 0;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Load
            // 
            this.Load.Location = new System.Drawing.Point(29, 806);
            this.Load.Name = "Load";
            this.Load.Size = new System.Drawing.Size(296, 123);
            this.Load.TabIndex = 2;
            this.Load.Text = "Load";
            this.Load.UseVisualStyleBackColor = true;
            this.Load.Click += new System.EventHandler(this.Load_Click);
            // 
            // LevelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1362, 1024);
            this.Controls.Add(this.Load);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.ObjectBox);
            this.Controls.Add(this.Level);
            this.Name = "LevelEditor";
            this.Text = "LevelEditor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox Level;
        private System.Windows.Forms.GroupBox ObjectBox;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button Load;
    }
}