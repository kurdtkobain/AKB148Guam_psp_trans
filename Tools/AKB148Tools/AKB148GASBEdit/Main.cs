using AKB148GASBLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace AKB148GASBEdit
{
    public partial class Main : Form
    {
        private string currentFile;
        private int selectedRowIndex;

        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show("Are you sure you want to overwrite:\n'" + currentFile + "'?", "WARNING!!",
                    MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                var dlist = new List<dialog>();
                foreach (DataGridViewRow dr in dataGridView1.Rows)
                {
                    var item = new dialog();
                    item.offset = (long)dr.Cells["offset"].Value;
                    item.size = (int)dr.Cells["size"].Value;
                    item.text = dr.Cells["text"].Value.ToString();
                    item.text = item.text.Replace("<LINEEND>", Encoding.UTF8.GetString(new byte[] { 0x0A }));
                    item.text = item.text.Replace("<END>", Encoding.UTF8.GetString(new byte[] { 0x00 }));
                    dlist.Add(item);
                }

                if (!ASBTools.injectDialogList(currentFile, dlist))
                {
                    MessageBox.Show("An error occurred while inserting!", "ERROR", MessageBoxButtons.OK);
                }
            }
            MessageBox.Show("Done.", "Done.", MessageBoxButtons.OK);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            loadDialog();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRowIndex = e.RowIndex;
            textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["text"].Value.ToString();
            textBox1.MaxLength = (int)dataGridView1.Rows[e.RowIndex].Cells["size"].Value + 200;
            label1.Text = "Characters left: 0";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var tmps = textBox1.Text;
            tmps = tmps.Replace("<LINEEND>", Encoding.UTF8.GetString(new byte[] { 0x0A }));
            tmps = tmps.Replace("<END>", Encoding.UTF8.GetString(new byte[] { 0x00 }));
            label1.Text = "Characters left: " + ((textBox1.MaxLength - 200) - Encoding.UTF8.GetBytes(tmps).Length);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var tmps = textBox1.Text;
            tmps = tmps.Replace("<LINEEND>", Encoding.UTF8.GetString(new byte[] { 0x0A }));
            tmps = tmps.Replace("<END>", Encoding.UTF8.GetString(new byte[] { 0x00 }));
            if (((textBox1.MaxLength - 200) - Encoding.UTF8.GetBytes(tmps).Length) >= 0)
            {
                var obj = (dialog)dataGridView1.Rows[selectedRowIndex].DataBoundItem;
                obj.text = textBox1.Text;
                dataGridView1.Refresh();
            }
            else
            {
                MessageBox.Show("Can't insert text that is larger than original size.", "ERROR", MessageBoxButtons.OK);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                MessageBox.Show("For this to take effect you must reload file.", "Attention!", MessageBoxButtons.OK);
            }
        }

        private void loadDialog()
        {
            // Create an instance of the open file dialog box.
            var openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "AKB 1/48 Guam ASB File (.asb)|*.asb";
            openFileDialog1.FilterIndex = 1;

            // Process input if the user clicked OK.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFileDialog1.FileName;
                var dlist = ASBTools.getDialogList(openFileDialog1.FileName, true, checkBox1.Checked);
                dataGridView1.ReadOnly = true;
                dataGridView1.DataSource = dlist;
                dataGridView1.AutoResizeColumns();
            }
        }
    }
}