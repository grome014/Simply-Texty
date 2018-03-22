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
    public partial class OathDialog : FoundationControlLibrary.BaseDialogForm
    {
        public OathDialog()
        {
            InitializeComponent();
        }

        private void OathDialog_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int width = this.ClientRectangle.Width;
            int height = this.ClientRectangle.Height;

            Point[] quadPoints = new Point[]
            {
                new Point(0,0),
                new Point(width, 0),
                new Point(width, height),
                new Point(0, width)
            };
            using (PathGradientBrush brush = new PathGradientBrush(quadPoints))
            {
                brush.SurroundColors = new Color[] { Color.Blue, Color.BlueViolet, Color.Yellow};
                brush.CenterColor = Color.Purple;
                brush.CenterPoint = new Point(0, 0);
                g.FillRectangle(brush, this.ClientRectangle);
            }
        }
    }
}
