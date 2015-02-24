using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScriptEditor
{
    public partial class Main : Form
    {
        int selectedRowIndex;
        class dialog : INotifyPropertyChanged
        {
            public long offset { get; set; }
            public int size { get; set; }
            public string text { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd1 = new SaveFileDialog();
            sfd1.Filter = "Text Files (.txt)|*.txt";
            sfd1.FilterIndex = 1;

            if (sfd1.ShowDialog() == DialogResult.OK)
            {
                List<dialog> items = new List<dialog>();
                foreach (DataGridViewRow dr in dataGridView1.Rows)
                {
                    dialog item = new dialog();
                    item.offset = (long)dr.Cells["offset"].Value;
                    item.size = (int)dr.Cells["size"].Value;
                    item.text = dr.Cells["text"].Value.ToString();
                    items.Add(item);
                }

                using (BinaryWriter writer = new BinaryWriter(File.Open(sfd1.FileName, FileMode.Create)))
                {
                    foreach (dialog d in items)
                    {
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(d.offset.ToString()));
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(";"));
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(d.size.ToString()));
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(";"));
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(d.text.ToString()));
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(Environment.NewLine));
                    }

                }

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "Text Files (.txt)|*.txt";
            openFileDialog1.FilterIndex = 1;

            // Process input if the user clicked OK.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = "";
                label1.Text = "Characters left: 0";
                selectedRowIndex = 0;
                System.IO.Stream fileStream = openFileDialog1.OpenFile();
                List<dialog> dList = new List<dialog>();
                System.IO.StreamReader file = new System.IO.StreamReader(fileStream);
                while (!file.EndOfStream)
                {
                    dialog d1 = new dialog();

                    string s = file.ReadLine();
                    string[] split = s.Split(new char[] { ';' });
                    d1.offset = Convert.ToInt64(split[0]);
                    d1.size = Convert.ToInt32(split[1]);
                    d1.text = split[2];
                    dList.Add(d1);
                }
                file.Close();
                dataGridView1.ReadOnly = true;
                dataGridView1.DataSource = dList;
                dataGridView1.AutoResizeColumns();
            }
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
            string tmps = textBox1.Text;
            tmps = tmps.Replace("<LINEEND>", System.Text.Encoding.UTF8.GetString(new byte[] { 0x0A }));
            tmps = tmps.Replace("<END>", System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 }));
            label1.Text = "Characters left: " + ((textBox1.MaxLength - 200) - System.Text.Encoding.UTF8.GetBytes(tmps).Length);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dialog obj = (dialog)dataGridView1.Rows[selectedRowIndex].DataBoundItem;
            obj.text = textBox1.Text;
            dataGridView1.Refresh();

        }
    }
}
