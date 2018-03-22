using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Group8_Homework8
{
    public partial class AboutDialog : FoundationControlLibrary.BaseDialogForm
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void AboutDialog_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Color startColor = Color.DarkBlue;
            Color endColor = Color.White;
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, startColor, endColor, LinearGradientMode.ForwardDiagonal))
            {
                ColorBlend blend = new ColorBlend();
                blend.Colors = new Color[] { Color.Blue, Color.BlueViolet, Color.Yellow };
                blend.Positions = new float[] { 0.0f, 0.5f, 1.0f };
                brush.InterpolationColors = blend;
                using (Pen pen = new Pen(brush, 30))
                {
                    g.DrawRectangle(pen, this.ClientRectangle);
                }
            }
        }
    }
}
