using AKB148GASBLib;
using DirectShowLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace StoryTest
{
    public partial class ASB : Form
    {

        class scriptFileData
        {
            public string audioFilename;
            public string Speaker;
            public string Text;
        }


        List<scriptFileData> Script = null;
        scriptFileData lastSelected = null;
        int volume = 83;
        string AT3Dir = "E:/For Sorting/Development/PSP Trans/akbguam/ALL/AT3/";
        string ASBDir = "E:/For Sorting/Development/PSP Trans/akbguam/files/Asb/";
        List<string> files = new List<string>();
        public ASB()
        {
            InitializeComponent();
            files.AddRange(Directory.GetFiles(ASBDir));
        }

        private void ASB_Load(object sender, EventArgs e)
        {
            ASBTView.Nodes.Clear();
            ASBTView.BeginUpdate();
            foreach (string file in files)
            {
                string member = Members.getMemberName(int.Parse(Path.GetFileNameWithoutExtension(file).Split('_')[1].Remove(0, 3)));
                if (!ASBTView.Nodes.ContainsKey(member))
                {
                    ASBTView.Nodes.Add(member, member);
                }
                ASBTView.Nodes[member].Nodes.Add(file, Path.GetFileNameWithoutExtension(file));
            }
            ASBTView.EndUpdate();
            trackBar1.Value = volume;
        }

        private void ASBTView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (ASBTView.SelectedNode.Level == 0)
            {
                return;
            }
            loadASB(ASBTView.SelectedNode.Name);
            treeView1.Nodes.Clear();
            treeView1.BeginUpdate();
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (var sd in Script)
            {
                TreeNode node = new TreeNode(sd.Text);
                node.Name = Path.GetFileName(sd.audioFilename);
                nodes.Add(node);
            }
            treeView1.Nodes.AddRange(nodes.ToArray());
            treeView1.EndUpdate();
        }

        private void BuildGraphAndPlay()
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
            hr = ifg2.RenderFile(AT3Dir + lastSelected.audioFilename, null);
            DsError.ThrowExceptionForHR(hr);

            int vol = (10000) - System.Math.Abs(volume * 100);
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

        private void loadASB(string asb)
        {
            Script = new List<scriptFileData>();
            List<ASBTools.ScriptData> sd = ASBTools.getScript(asb);
            for (int i = 0; i < sd.Count; i++)
            {

                if (sd[i].OPCode.ToString("X4") == "4B29" && sd[i].paramNum == 3)
                {
                    scriptFileData sfd = new scriptFileData();
                    sfd.audioFilename = ASBTools.getDialogFromOffset(asb, sd[i].paramList[2], true) + ".at3";
                    sfd.Speaker = Members.getMemberName(sd[i].paramList[1]);
                    sfd.Text = ASBTools.getDialogFromOffset(asb, sd[i + 1].paramList[0], true);
                    sfd.Text = sfd.Text.Replace("<LINEEND>", Environment.NewLine);
                    Script.Add(sfd);
                }
                else if (sd[i].OPCode.ToString("X4") == "4B29" && sd[i].paramNum != 3)
                {
                    scriptFileData sfd = new scriptFileData();
                    sfd.audioFilename = null;
                    sfd.Speaker = Members.getMemberName(sd[i].paramList[1]);
                    sfd.Text = ASBTools.getDialogFromOffset(asb, sd[i + 1].paramList[0], true);
                    sfd.Text = sfd.Text.Replace("<LINEEND>", Environment.NewLine);
                    Script.Add(sfd);
                }
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label3.Text = "Volume: " + trackBar1.Value.ToString() + "%";
            volume = trackBar1.Value;
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            lastSelected = Script.Where(x => x.Text == treeView1.SelectedNode.Text).FirstOrDefault();
            label1.Text = lastSelected.Text;
            label2.Text = lastSelected.Speaker;
            if (lastSelected.audioFilename != null)
            {
                System.Threading.Thread thread = new Thread(BuildGraphAndPlay);
                thread.Start();
            }
        }
    }
}
