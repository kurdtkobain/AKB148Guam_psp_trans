using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AKB148GASBLib;

namespace AKB148GDumpText
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] == "-d")
            {
                Dump(args[1], args[2]);
            }
            else if (args[0] == "-i")
            {
                Insert(args[1], args[2]);
            }
            else if (args[0] == "-d2")
            {
                Dump(args[1], args[2], true);
            }
            else
            {
                Console.WriteLine("Usage:\n");
                Console.WriteLine("Dump: AKB148GDumpText.exe -d FILE.asb Script.txt\n");
                Console.WriteLine("Dump (Only Event Dialog): AKB148GDumpText.exe -d2 FILE.asb Script.txt\n");
                Console.WriteLine("Insert: AKB148GDumpText.exe -i Script.txt FILE.asb \n");
            }
        }
        static void Dump(string inFile, string outFile, bool eventOnly = false)
        {
            List<dialog> dlist;
            if (eventOnly)
            {
                dlist = ASBTools.getDialogList(inFile, true, true);
            }
            else
            {
                dlist = ASBTools.getDialogList(inFile, true);
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open(outFile, FileMode.Create)))
            {
                foreach (dialog dl in dlist)
                {
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(dl.offset.ToString()));
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(";"));
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(dl.size.ToString()));
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(";"));
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(dl.text));
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(Environment.NewLine));
                }

            }
        }

        static void Insert(string cmdFile, string inFile)
        {
            List<dialog> dList = new List<dialog>();
            using (System.IO.StreamReader file = new System.IO.StreamReader(cmdFile))
            {
                while (!file.EndOfStream)
                {
                    dialog d1 = new dialog();

                    string s = file.ReadLine();
                    string[] split = s.Split(new char[] { ';' });
                    d1.offset = Convert.ToInt64(split[0]);
                    d1.size = Convert.ToInt32(split[1]);
                    d1.text = split[2];
                    d1.text = d1.text.Replace("<LINEEND>", System.Text.Encoding.UTF8.GetString(new byte[] { 0x0A }));
                    d1.text = d1.text.Replace("<END>", System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 }));
                    dList.Add(d1);
                }
            }
            Console.WriteLine("Writing to {0}", inFile);
            using (BinaryWriter writer = new BinaryWriter(File.Open(inFile, FileMode.Open)))
            {
                foreach (dialog d in dList)
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

    }
}
