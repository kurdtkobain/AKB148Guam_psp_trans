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
                CheckStart(args[1], args[2]);
                //Console.ReadKey();
            }
            else if (args[0] == "-i")
            {
                Insert(args[1], args[2]);
                //Console.ReadKey();
            }
        }

        static void CheckStart(string inFile, string outFile)
        {

            byte[] ByteBuffer = File.ReadAllBytes(inFile);
            byte[] StringBytes = Encoding.UTF8.GetBytes("__m");
            for (int i = 0; i <= (ByteBuffer.Length - StringBytes.Length); i++)
            {
                if (ByteBuffer[i] == StringBytes[0])
                {
                    if (ByteBuffer[i + 1] == StringBytes[1])
                    {
                        if (ByteBuffer[i + 2] == StringBytes[2])
                        {
                            Console.WriteLine("String was found at offset {0}", i);
                            Dump(inFile, i, outFile);
                        }
                    }
                }
            }
        }
        static void Dump(string inFile, int offset, string outFile)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(inFile, FileMode.Open, FileAccess.Read)))
            {
                reader.BaseStream.Position = offset;
                //List<string> offlst = new List<string>();
                long[] offlst = new long[1000];
                int[] size = new int[1000];
                List<byte[]> textLst = new List<byte[]>();
                List<byte> mahByteArray = new List<byte>();
                int j = 0;
                offlst[j] = reader.BaseStream.Position;
                while (reader.PeekChar() != -1)
                {
                    if (reader.PeekChar() == 0x00)
                    {
                        mahByteArray.Add(reader.ReadByte());
                        //mahByteArray.AddRange(System.Text.Encoding.UTF8.GetBytes("<END>"));
                        textLst.Add(mahByteArray.ToArray());
                        size[j] = mahByteArray.Count;
                        mahByteArray.Clear();
                        j++;
                        offlst[j] = reader.BaseStream.Position;
                    }
                    else if (reader.PeekChar() == 0x0A)
                    {

                        //mahByteArray.AddRange(System.Text.Encoding.UTF8.GetBytes("<LINEEND>"));
                        mahByteArray.Add(reader.ReadByte());

                    }
                    else
                    {

                        mahByteArray.Add(reader.ReadByte());
                    }
                }
                using (BinaryWriter writer = new BinaryWriter(File.Open(outFile, FileMode.Create)))
                {
                    for(int i=2;i<textLst.Count-1;i++)
                    {
                        if (textLst[i][0] == 0x40|| textLst[i][0] == 0x00 || Regex.IsMatch(System.Text.Encoding.UTF8.GetString(new byte[] { textLst[i][0], textLst[i][1] }), @"^\d+$"))
                        {

                        }
                        else
                        {
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(offlst[i].ToString()));
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(";"));
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(size[i].ToString()));
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(";"));
                        
                            foreach (byte b in textLst[i])
                            {
                                if (b == 0x00)
                                {
                                    writer.Write(System.Text.Encoding.UTF8.GetBytes("<END>"));
                                }
                                else if (b == 0x0A)
                                {
                                    writer.Write(System.Text.Encoding.UTF8.GetBytes("<LINEEND>"));
                                }
                                else
                                {
                                    writer.Write(b);
                                }

                            }
                            writer.Write(System.Text.Encoding.UTF8.GetBytes(Environment.NewLine));
                        }
                    }
                }

            }

        }


        static void Insert(string cmdFile, string inFile)
        {
            string line;
            List<dialog> dList = new List<dialog>();
            System.IO.StreamReader file = new System.IO.StreamReader(cmdFile);
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
            file.Close();
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
