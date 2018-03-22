using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Group8_Homework8
{
    public partial class TextOptionsDialog : Form, ITextOptionsDialog
    {
        public event EventHandler OK;
        public event EventHandler Apply;
        public event EventHandler Cancel;
        public event EventHandler SendToBack;
        public event EventHandler BringToFront;

        public BindingSource BindingSource { get; set; }

        public TextOptionsDialog()
        {
            InitializeComponent();
            BindingSource = new BindingSource();
            BindingSource.DataSource = new BindingList<Text>();
            InitializeHelp();
        }

        private void InitializeDataBindings()
        {
            colorPanel.DataBindings.Add("BackColor", this.BindingSource, "Color", true, DataSourceUpdateMode.OnPropertyChanged);
            textRichTextBox.DataBindings.Add("Text", this.BindingSource, "Txt", true, DataSourceUpdateMode.OnPropertyChanged);
            xNumericUpDown.DataBindings.Add("Value", this.BindingSource, "X", true, DataSourceUpdateMode.OnPropertyChanged);
            yNumericUpDown.DataBindings.Add("Value", this.BindingSource, "Y", true, DataSourceUpdateMode.OnPropertyChanged);
            fontNameTextBox.DataBindings.Add("Text", this.BindingSource, "FontName", true, DataSourceUpdateMode.OnPropertyChanged);
            fontSizeNumericUpDown.DataBindings.Add("Value", this.BindingSource, "FontSize", true, DataSourceUpdateMode.OnPropertyChanged); 
            rotationNumericUpDown.DataBindings.Add("Value", this.BindingSource, "Rotation", true, DataSourceUpdateMode.OnPropertyChanged);
            bindingNavigator.BindingSource = this.BindingSource;
        }

        private void InitializeHelp()
        {
            this.helpProvider.SetHelpString(textRichTextBox, "The string that displays from the text object.");
            this.helpProvider.SetHelpString(xNumericUpDown, "The X-coordinate of the text object.");
            this.helpProvider.SetHelpString(yNumericUpDown, "The Y-coordinate of the text object.");
            this.helpProvider.SetHelpString(fontNameTextBox, "The name of the font for the text object.");
            this.helpProvider.SetHelpString(fontSizeNumericUpDown, "The size of the font for the text object.");
            this.helpProvider.SetHelpString(rotationNumericUpDown, "The rotation of the text object in degrees.");
            this.helpProvider.SetHelpString(okButton, "Save all applied changes to text objects.");
            this.helpProvider.SetHelpString(applyButton, "Apply the changes to the current text object. Note that you must click 'OK' for changes to be saved.");
            this.helpProvider.SetHelpString(cancelButton, "Cancel and revert all applied changes made to text objects.");
            this.helpProvider.SetHelpString(sendToBackButton, "Change Z-order of the current text object such that it is behind other text objects on the canvas.");
            this.helpProvider.SetHelpString(bringToFrontButton, "Change Z-order of the current text object such that it is in front of other text objects on the canvas.");
            this.helpProvider.SetHelpString(fontButton, "Open font dialog to pick select a font for the current text object.");
            this.helpProvider.SetHelpString(colorButton, "Open color dialog to pick select a color for the current text object.");
        }

        private void TextOptionsDialog_Load(object sender, EventArgs e)
        {
            InitializeDataBindings();
        }

        public void setBoundaries(float minX, float minY, float maxX, float maxY)
        {
            xNumericUpDown.Minimum = (decimal) minX;
            xNumericUpDown.Maximum = (decimal) maxX;
            yNumericUpDown.Minimum = (decimal) minY;
            yNumericUpDown.Maximum = (decimal) maxY;
        } 

        private void fontButton_Click(object sender, EventArgs e)
        {
            using (FontDialog fontDialog = new FontDialog())
            {

                fontDialog.Font = new Font(this.fontNameTextBox.Text, (float)this.fontSizeNumericUpDown.Value);

                DialogResult result = fontDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                   
                    this.fontNameTextBox.Text = fontDialog.Font.Name;
                    this.fontSizeNumericUpDown.Value = (decimal)fontDialog.Font.Size;
                }
            }

        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {

                colorDialog.Color = this.colorPanel.BackColor;

                DialogResult result = colorDialog.ShowDialog();
                
                if(result == DialogResult.OK)
                { 
                    this.colorPanel.BackColor = colorDialog.Color;
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if(OK != null)
                OK(this, e);

            this.Close();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if(Apply != null)
                Apply(this, e);                    
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (Cancel != null)
                Cancel(this, e);

            this.Close();
        }

        private void sendToBackButton_Click(object sender, EventArgs e)
        {
            if (SendToBack != null)
                SendToBack(this, e);
        }

        private void bringToFrontButton_Click(object sender, EventArgs e)
        {
            if (BringToFront != null)
                BringToFront(this, e);
        }

    }
}
