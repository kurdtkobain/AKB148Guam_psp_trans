using AKB148GASBLib;
using DirectShowLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoryTest
{

    public partial class Form1 : Form
    {
        class scriptFileData
        {
            public string audioFilename;
            public string Text;
        }


        List<scriptFileData> Script = new List<scriptFileData>();
        public int indexer = 0;
        string DefaultDir = "C:/Development/PSP Trans/akbguam/ALL/AT3/";
        string ASBDir = "C:/Development/PSP Trans/akbguam/files/Asb/";
        List<string> files = new List<string>();
        private int selectedRowIndex = 0;

        public Form1()
        {
            InitializeComponent();
            files.AddRange(Directory.GetFiles(ASBDir));
            dataGridView1.DataSource = files.Select(x => new { Value = x }).ToList(); ;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            Thread pa = new Thread(playFileDS);
            pa.IsBackground = true;
            pa.Start();
            
        }

        private void playFileDS()
        {
            int hr;
            EventCode ev;
            FilterGraph fg;
            IFilterGraph2 ifg2;
            IMediaControl imc;
            IMediaEvent ime;

            fg = new FilterGraph();

            // Get the IFilterGraph2 interface from the fg object
            ifg2 = (IFilterGraph2)fg;

            // Get the IMediaControl interface from the fg object
            imc = (IMediaControl)fg;

            // Get the IMediaEvent interface from the fg object
            ime = (IMediaEvent)fg;

            // Build the graph
            hr = ifg2.RenderFile(Script[indexer].audioFilename, null);
            DsError.ThrowExceptionForHR(hr);

            // Run the graph
            hr = imc.Run();
            DsError.ThrowExceptionForHR(hr);

            // Wait for the entire file to finish playing
            hr = ime.WaitForCompletion(-1, out ev);
            DsError.ThrowExceptionForHR(hr);

            // Release the graph (and all its interfaces)
            Marshal.ReleaseComObject(fg);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadASB();
        }

        private void loadASB()
        {
            Script = new List<scriptFileData>();
            string filenm = dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString();
            List<ASBTools.ScriptData> sd = ASBTools.getScript(filenm);

            for (int i = 0; i < sd.Count; i++)
            {

                if (sd[i].OPCode.ToString("X4") == "4B29" && sd[i].paramNum == 3)
                {
                    scriptFileData sfd = new scriptFileData();
                    sfd.audioFilename = DefaultDir + ASBTools.getDialogFromOffset(filenm, sd[i].paramList[2], true) + ".at3";
                    sfd.Text = ASBTools.getDialogFromOffset(filenm, sd[i + 1].paramList[0], true);
                    sfd.Text = sfd.Text.Replace("<LINEEND>", Environment.NewLine);
                    Script.Add(sfd);
                }
            }

            if (Script.Count == 0)
            {
                label1.Text = "No Dialogue to display.";
            }
            else
            {
                label1.Text = Script[indexer].Text;
            }
            button2.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (indexer < Script.Count - 1)
            {
                indexer++;
            }
            else
            {
                indexer = 0;
            }
            label1.Text = Script[indexer].Text;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRowIndex = e.RowIndex;
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            loadASB();
        }
    }
}
