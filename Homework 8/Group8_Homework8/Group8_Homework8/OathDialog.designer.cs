﻿namespace Group8_Homework8
{
    partial class OathDialog
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
            this.oathUserControl = new FoundationControlLibrary.OathUserControl();
            this.dialogPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dialogPanel
            // 
            this.dialogPanel.Controls.Add(this.oathUserControl);
            this.dialogPanel.Size = new System.Drawing.Size(436, 112);
            // 
            // oathUserControl
            // 
            this.oathUserControl.BackColor = System.Drawing.Color.Transparent;
            this.oathUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.oathUserControl.Location = new System.Drawing.Point(0, 0);
            this.oathUserControl.Name = "oathUserControl";
            this.oathUserControl.Size = new System.Drawing.Size(436, 112);
            this.oathUserControl.TabIndex = 0;
            // 
            // OathDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(436, 391);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OathDialog";
            this.ShowIcon = false;
            this.Text = "Oath Dialog";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OathDialog_Paint);
            this.dialogPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private FoundationControlLibrary.OathUserControl oathUserControl;
    }
}
