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

namespace AKB148GASBEdit
{
    public partial class Main : Form
    {
        int selectedRowIndex;
        string currentFile;
        bool onlyEvent;
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
            if (MessageBox.Show("Are you sure you want to overwrite:\n'" + currentFile + "'?", "WARNING!!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                List<dialog> dlist = new List<dialog>();
                foreach (DataGridViewRow dr in dataGridView1.Rows)
                {
                    dialog item = new dialog();
                    item.offset = (long)dr.Cells["offset"].Value;
                    item.size = (int)dr.Cells["size"].Value;
                    item.text = dr.Cells["text"].Value.ToString();
                    item.text = item.text.Replace("<LINEEND>", System.Text.Encoding.UTF8.GetString(new byte[] { 0x0A }));
                    item.text = item.text.Replace("<END>", System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 }));
                    dlist.Add(item);
                }

                using (BinaryWriter writer = new BinaryWriter(File.Open(currentFile, FileMode.Open)))
                {
                    foreach (dialog d in dlist)
                    {
                        writer.BaseStream.Position = d.offset;
                        if (System.Text.Encoding.UTF8.GetBytes(d.text).Length < d.size)
                        {
                            string s = d.text.Replace("\0", string.Empty);
                            int pad = d.size - System.Text.Encoding.UTF8.GetBytes(d.text).Length;
                            var mahByteArray = new List<byte>();
                            mahByteArray.AddRange(System.Text.Encoding.UTF8.GetBytes(s));
                            for (int i = 0; i < pad; i++)
                            {
                                mahByteArray.AddRange(System.Text.Encoding.ASCII.GetBytes(" "));
                            }
                            mahByteArray.Add(0x00);
                            writer.Write(mahByteArray.ToArray(), 0, d.size);
                        }
                        else if (System.Text.Encoding.UTF8.GetBytes(d.text).Length > d.size)
                        {
                            var mahByteArray = new List<byte>();
                            mahByteArray.AddRange(System.Text.Encoding.UTF8.GetBytes(d.text));
                            mahByteArray.Insert(d.size, 0x00);
                            writer.Write(mahByteArray.ToArray(), 0, d.size);
                        }
                        else
                        {
                            writer.Write(System.Text.Encoding.UTF8.GetBytes(d.text), 0, d.size);
                        }
                    }
                }
            }
            MessageBox.Show("Done.", "Done.", MessageBoxButtons.OK);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (onlyEvent)
            {
                loadEvent();
            }
            else
            {
                loadAll();
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
            string tmps = textBox1.Text;
            tmps = tmps.Replace("<LINEEND>", System.Text.Encoding.UTF8.GetString(new byte[] { 0x0A }));
            tmps = tmps.Replace("<END>", System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 }));
            if (((textBox1.MaxLength - 200) - System.Text.Encoding.UTF8.GetBytes(tmps).Length) >= 0)
            {
                dialog obj = (dialog)dataGridView1.Rows[selectedRowIndex].DataBoundItem;
                obj.text = textBox1.Text;
                dataGridView1.Refresh();
            }
            else {
                MessageBox.Show("Can't insert text that is larger than original size.","ERROR",MessageBoxButtons.OK);

            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                MessageBox.Show("For this to take effect you must reload file.", "Attention!", MessageBoxButtons.OK);
            }
            onlyEvent = checkBox1.Checked;
        }


        private void loadAll()
        {
            List<dialog> dlist = new List<dialog>();
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "AKB 1/48 Guam ASB File (.asb)|*.asb";
            openFileDialog1.FilterIndex = 1;

            // Process input if the user clicked OK.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFileDialog1.FileName;
                using (BinaryReader reader = new BinaryReader(File.Open(openFileDialog1.FileName, FileMode.Open, FileAccess.Read)))
                {
                    reader.BaseStream.Position = 52;
                    long tmpl;
                    if (BitConverter.IsLittleEndian)
                    {
                        tmpl = reader.ReadInt32();
                    }
                    else
                    {
                        byte[] tmpb = new byte[4];
                        tmpb = reader.ReadBytes(2);
                        Array.Reverse(tmpb);
                        tmpl = BitConverter.ToInt16(tmpb, 0);
                    }
                    Console.WriteLine("Start position is at offset {0}", tmpl);
                    reader.BaseStream.Position = tmpl;
                    List<byte> mahByteArray = new List<byte>();
                    dialog d = new dialog();
                    d.offset = reader.BaseStream.Position;
                    while (reader.PeekChar() != -1)
                    {
                        if (reader.PeekChar() == 0x00)
                        {
                            mahByteArray.Add(reader.ReadByte());
                            d.text = System.Text.Encoding.UTF8.GetString(mahByteArray.ToArray());
                            d.size = mahByteArray.Count;
                            dlist.Add(d);
                            mahByteArray.Clear();
                            d = new dialog();
                            d.offset = reader.BaseStream.Position;
                        }
                        else
                        {
                            mahByteArray.Add(reader.ReadByte());
                        }
                    }
                }
                List<dialog> final = new List<dialog>();
                foreach (dialog dl in dlist)
                {
                    int tmpnum;
                    if (dl.text.StartsWith(System.Text.Encoding.UTF8.GetString(new byte[] { 0x40 })) || dl.text.StartsWith(System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 })) || dl.text.StartsWith("//") || dl.text.StartsWith("pow( x,") || dl.text.StartsWith("env") || dl.text.StartsWith("__main") || dl.text.StartsWith("main") || dl.text.StartsWith("se") || int.TryParse(dl.text.Substring(0, 2), out tmpnum))
                    {

                    }
                    else
                    {
                        dialog tmp = new dialog();
                        tmp.offset = dl.offset;
                        tmp.size = dl.size;
                        string tmps = dl.text;
                        tmps = tmps.Replace(System.Text.Encoding.UTF8.GetString(new byte[] { 0x0A }), "<LINEEND>");
                        tmps = tmps.Replace(System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 }), "<END>");
                        tmp.text = tmps;
                        final.Add(tmp);
                    }
                }
                dlist.Clear();
                dataGridView1.ReadOnly = true;
                dataGridView1.DataSource = final;
                dataGridView1.AutoResizeColumns();
            }
        }

        private void loadEvent()
        {
            List<dialog> dlist = new List<dialog>();
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "AKB 1/48 Guam ASB File (.asb)|*.asb";
            openFileDialog1.FilterIndex = 1;

            // Process input if the user clicked OK.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFileDialog1.FileName;
                using (BinaryReader reader = new BinaryReader(File.Open(openFileDialog1.FileName, FileMode.Open, FileAccess.Read)))
                {
                    reader.BaseStream.Position = 52;
                    long tmpl;
                    if (BitConverter.IsLittleEndian)
                    {
                        tmpl = reader.ReadInt32();
                    }
                    else
                    {
                        byte[] tmpb = new byte[4];
                        tmpb = reader.ReadBytes(2);
                        Array.Reverse(tmpb);
                        tmpl = BitConverter.ToInt16(tmpb, 0);
                    }
                    Console.WriteLine("Start position is at offset {0}", tmpl);
                    reader.BaseStream.Position = tmpl;
                    List<byte> mahByteArray = new List<byte>();
                    dialog d = new dialog();
                    d.offset = reader.BaseStream.Position;
                    while (reader.PeekChar() != -1)
                    {
                        if (reader.PeekChar() == 0x00)
                        {
                            mahByteArray.Add(reader.ReadByte());
                            d.text = System.Text.Encoding.UTF8.GetString(mahByteArray.ToArray());
                            d.size = mahByteArray.Count;
                            dlist.Add(d);
                            mahByteArray.Clear();
                            d = new dialog();
                            d.offset = reader.BaseStream.Position;
                        }
                        else
                        {
                            mahByteArray.Add(reader.ReadByte());
                        }
                    }
                }
                List<dialog> final = new List<dialog>();
                bool write = false;
                string filename = "@" + Path.GetFileNameWithoutExtension(currentFile);
                foreach (dialog dl in dlist)
                {

                    int tmpnum;
                    if (write)
                    {
                        if (dl.text.StartsWith(System.Text.Encoding.UTF8.GetString(new byte[] { 0x40 })) || dl.text.StartsWith(System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 })) || dl.text.StartsWith("//") || dl.text.StartsWith("env") || dl.text.StartsWith("pow( x,") || dl.text.StartsWith("__main") || dl.text.StartsWith("main") || dl.text.StartsWith("se") || int.TryParse(dl.text.Substring(0, 2), out tmpnum))
                        {

                        }
                        else
                        {
                            dialog tmp = new dialog();
                            tmp.offset = dl.offset;
                            tmp.size = dl.size;
                            string tmps = dl.text;
                            tmps = tmps.Replace(System.Text.Encoding.UTF8.GetString(new byte[] { 0x0A }), "<LINEEND>");
                            tmps = tmps.Replace(System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 }), "<END>");
                            tmp.text = tmps;
                            final.Add(tmp);
                        }
                    }
                    else
                    {
                        if (dl.text.Contains(filename.Substring(0,11)))
                        {
                            write = true;
                        }
                    }
                }
                dlist.Clear();
                dataGridView1.ReadOnly = true;
                dataGridView1.DataSource = final;
                dataGridView1.AutoResizeColumns();
            }
        }
    }
}
