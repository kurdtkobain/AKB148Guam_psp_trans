using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AKB148GDumpText
{
    class Program
    {
        struct dialog
        {
            public long offset;
            public int size;
            public string text;

        }
        static void Main(string[] args)
        {
            if (args[0] == "-d")
            {
                Dump(args[1], args[2]);
                //Console.ReadKey();
            }
            else if (args[0] == "-i")
            {
                Insert(args[1], args[2]);
            }
            else if (args[0] == "-d2")
            {
                Dump2(args[1], args[2]);
            }
            else
            {
                Console.WriteLine("Usage:\n");
                Console.WriteLine("Dump: AKB148GDumpText.exe -d FILE.asb Script.txt\n");
                Console.WriteLine("Insert: AKB148GDumpText.exe -i Script.txt FILE.asb \n");
            }
        }
        static void Dump(string inFile, string outFile)
        {
            List<dialog> dlist = new List<dialog>();
            using (BinaryReader reader = new BinaryReader(File.Open(inFile, FileMode.Open, FileAccess.Read)))
            {
                reader.BaseStream.Position = 52;
                long tmpl;
                if (BitConverter.IsLittleEndian)
                {
                    tmpl = reader.ReadInt16();
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
            using (BinaryWriter writer = new BinaryWriter(File.Open(outFile, FileMode.Create)))
            {
                foreach (dialog dl in dlist)
                {
                    if (dl.text.StartsWith("@") || dl.text.StartsWith(System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 })) || dl.text.StartsWith("//") || dl.text.StartsWith("env") || dl.text.StartsWith("pow( x,") || dl.text.StartsWith("__main") || dl.text.StartsWith("main") || dl.text.StartsWith("se") || Regex.IsMatch(dl.text,"([0-9]{2}_[0-9]{2}_[0-9]{4})"))
                    {

                    }
                    else
                    {
                        string tmps = dl.text;
                        tmps = tmps.Replace(System.Text.Encoding.UTF8.GetString(new byte[] { 0x0A }), "<LINEEND>");
                        tmps = tmps.Replace(System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 }), "<END>");
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(dl.offset.ToString()));
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(";"));
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(dl.size.ToString()));
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(";"));
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(tmps));
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(Environment.NewLine));
                    }

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

        static void Dump2(string inFile, string outFile)
        {
            List<dialog> dlist = new List<dialog>();
            using (BinaryReader reader = new BinaryReader(File.Open(inFile, FileMode.Open, FileAccess.Read)))
            {
                reader.BaseStream.Position = 52;
                long tmpl;
                if (BitConverter.IsLittleEndian)
                {
                    tmpl = reader.ReadInt16();
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
            using (BinaryWriter writer = new BinaryWriter(File.Open(outFile, FileMode.Create)))
            {
                bool write = false;
                string filename = "@" + Path.GetFileNameWithoutExtension(inFile);
                foreach (dialog dl in dlist)
                {
                    if (write)
                    {
                        if (dl.text.StartsWith(System.Text.Encoding.UTF8.GetString(new byte[] { 0x40 })) || dl.text.StartsWith(System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 })) || dl.text.StartsWith("//") || dl.text.StartsWith("env") || dl.text.StartsWith("pow( x,") || dl.text.StartsWith("__main") || dl.text.StartsWith("main") || dl.text.StartsWith("se") || Regex.IsMatch(dl.text, "([0-9]{2}_[0-9]{2}_[0-9]{4})"))
                        {

                        }
                        else
                        {
                            string tmps = dl.text;
                            tmps = tmps.Replace(System.Text.Encoding.UTF8.GetString(new byte[] { 0x0A }), "<LINEEND>");
                            tmps = tmps.Replace(System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 }), "<END>");
                            writer.Write(System.Text.Encoding.UTF8.GetBytes(dl.offset.ToString()));
                            writer.Write(System.Text.Encoding.UTF8.GetBytes(";"));
                            writer.Write(System.Text.Encoding.UTF8.GetBytes(dl.size.ToString()));
                            writer.Write(System.Text.Encoding.UTF8.GetBytes(";"));
                            writer.Write(System.Text.Encoding.UTF8.GetBytes(tmps));
                            writer.Write(System.Text.Encoding.UTF8.GetBytes(Environment.NewLine));
                        }
                    }
                    else
                    {
                        if (dl.text.Contains(filename))
                        {
                            write = true;
                        }
                    }
                }
            }

        }

    }
}
