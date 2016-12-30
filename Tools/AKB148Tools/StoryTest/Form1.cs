#region copyright
// <copyright file="Form1.cs" company="Kurdtkobain">
// Copyright (c) 2015-2017 All Rights Reserved
// </copyright>
// <author>Kurdtkobain</author>
// <date>2015/9/19 7:03:28 AM </date>
#endregion
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
            int hr;
            EventCode ev;
            FilterGraph fg;
            IFilterGraph2 ifg2;
            IMediaControl imc;
            IMediaEvent ime;
            IBasicAudio iba;

            fg = new FilterGraph();

            // Get the IFilterGraph2 interface from the fg object
            ifg2 = (IFilterGraph2)fg;

            // Get the IMediaControl interface from the fg object
            imc = (IMediaControl)fg;

            // Get the IMediaEvent interface from the fg object
            ime = (IMediaEvent)fg;

            // Get the IBasicAudio interface from the fg object
            iba = (IBasicAudio)fg;

            // Build the graph
            hr = ifg2.RenderFile(Script[indexer].audioFilename, null);
            DsError.ThrowExceptionForHR(hr);

            int vol = (10000) - System.Math.Abs(trackBar1.Value * 100);
            int finalvol = vol * (-1);

            //Set volume
            iba.put_Volume(finalvol);

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

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label3.Text = "Volume: " + trackBar1.Value.ToString() + "%";
        }
    }
}
