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
    public partial class ObjectPropertiesDialog : Form, IObjectPropertiesDialog
    {

        public DataGridView dataGridView { get; set; }

        public ObjectPropertiesDialog()
        {
            InitializeComponent();
            dataGridView = new DataGridView();
            dataGridView.Dock = DockStyle.Fill;          
        }

        private void ObjectPropertiesDialog_Load(object sender, EventArgs e)
        {
            this.Controls.Add(dataGridView);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            this.dataGridView.Rows.Add(1);
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rows = dataGridView.SelectedRows;

            foreach(DataGridViewRow row in rows)
                this.dataGridView.Rows.Remove(row);
        }
    }
}
