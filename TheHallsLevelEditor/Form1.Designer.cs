﻿
namespace TheHallsLevelEditor
{
    partial class Form1
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
            this.LoadRoom = new System.Windows.Forms.Button();
            this.newRoom = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LoadRoom
            // 
            this.LoadRoom.Location = new System.Drawing.Point(268, 80);
            this.LoadRoom.Name = "LoadRoom";
            this.LoadRoom.Size = new System.Drawing.Size(633, 279);
            this.LoadRoom.TabIndex = 0;
            this.LoadRoom.Text = "Load Room";
            this.LoadRoom.UseVisualStyleBackColor = true;
            this.LoadRoom.Click += new System.EventHandler(this.LoadRoom_Click);
            // 
            // newRoom
            // 
            this.newRoom.Location = new System.Drawing.Point(268, 497);
            this.newRoom.Name = "newRoom";
            this.newRoom.Size = new System.Drawing.Size(633, 293);
            this.newRoom.TabIndex = 1;
            this.newRoom.Text = "Create New Room";
            this.newRoom.UseVisualStyleBackColor = true;
            this.newRoom.Click += new System.EventHandler(this.newRoom_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1239, 1006);
            this.Controls.Add(this.newRoom);
            this.Controls.Add(this.LoadRoom);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button LoadRoom;
        private System.Windows.Forms.Button newRoom;
    }
}

