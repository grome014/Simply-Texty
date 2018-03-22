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
    public partial class AddTextDialog : Form, IAddTextDialog
    {
        public event EventHandler OK;
        public event EventHandler Cancel;

        public string Text
        {
            get { return this.textRichTextBox.Text; }
            set
            {
                this.textRichTextBox.Text = value;
            }
        }

        public AddTextDialog()
        {
            InitializeComponent();
            this.textRichTextBox.Text = "";
            InitializeHelp();
        }

        private void InitializeHelp()
        {
            this.helpProvider.SetHelpString(textRichTextBox, "The string that will be broken up into tokens - each for an individual text object.");
            this.helpProvider.SetHelpString(okButton, "Add text objects to canvas.");
            this.helpProvider.SetHelpString(cancelButton, "Cancel adding text objects.");
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (OK != null)
                OK(this, e);

            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (Cancel != null)
                Cancel(this, e);

            this.Close();
        }

    }
}
