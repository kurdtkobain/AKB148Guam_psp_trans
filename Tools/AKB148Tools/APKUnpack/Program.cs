using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APKUnpack
{
    class Program
    {
        class GENESTRT
        {
            public long offset;
            public long stringcnt;
            public long[] nameofflist;
            public long names_off;
            public string[] stringlist;

        }
        class PACKHEDR
        {
            public long headerSize;
            public long dummy;
            public long zero;
            public long dummySize;
            public long dummy2;
            public string dummystring;
        }
        class PACKTOC
        {
            public long size;
            public long offset;
            public int entrySize;
            public long files;
            public int folders;
            public long zero;
        }
        class PACKFSLS
        {
            public long headerSize;
            public long headerOffset;
            public long archives;
            public long dummy;
            public long dummy2;
            public long zero;
        }
        class FOLDERS
        {
            public List<byte[]> dummystring = new List<byte[]>();
            public List<int> nameIdx = new List<int>();
            public List<int> fileNum = new List<int>();
            public List<int> subFiles = new List<int>();
        }
        [STAThread]
        static int Main(string[] args)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "AKB 1/48 Guam all.apk|all.apk";

            if (fd.ShowDialog() == DialogResult.OK)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    FileStream filestream = new FileStream("out.txt", FileMode.Create);
                    var streamwriter = new StreamWriter(filestream);
                    streamwriter.AutoFlush = true;
                    Console.SetOut(streamwriter);
                    Console.SetError(streamwriter);
                    using (BinaryReader reader = new BinaryReader(File.Open(fd.FileName, FileMode.Open)))
                    {
                        reader.ReadBytes(16);
                        char[] tst = reader.ReadChars(8);
                        if (tst.ToString() != new char[] { 'P', 'A', 'C', 'K', 'H', 'E', 'D', 'R' }.ToString())
                        {
                            return 0;
                        }
                        PACKHEDR pkhdr = new PACKHEDR();
                        pkhdr.headerSize = reader.ReadInt64();
                        pkhdr.dummy = reader.ReadInt32();
                        pkhdr.zero = reader.ReadInt32();
                        pkhdr.dummySize = reader.ReadInt32();
                        pkhdr.dummy2 = reader.ReadInt32();
                        pkhdr.dummystring = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(16));
                        PACKTOC pktoc = new PACKTOC();
                        reader.ReadBytes(8);
                        pktoc.size = reader.ReadInt64();
                        pktoc.offset = reader.BaseStream.Position;
                        pktoc.entrySize = reader.ReadInt32();
                        pktoc.files = reader.ReadInt32();
                        pktoc.folders = reader.ReadInt32();
                        pktoc.zero = reader.ReadInt32();
                        GENESTRT gene1 = GetGENESTRT(reader, pktoc.offset, pktoc.size);
                        FOLDERS fldrs = new FOLDERS();
                        for (int k = 0; k < pktoc.folders; k++)
                        {
                            fldrs.dummystring.Add(reader.ReadBytes(pktoc.entrySize));
                            fldrs.nameIdx.Add(fldrs.dummystring[k].ElementAt(4));
                            fldrs.fileNum.Add(fldrs.dummystring[k].ElementAt(16));
                            fldrs.subFiles.Add(fldrs.dummystring[k].ElementAt(20));
                        }
                        for (int l = pktoc.folders; l < pktoc.files; l++)
                        {
                            reader.ReadInt32();
                            int idx = reader.ReadInt32();
                            reader.ReadInt32();
                            reader.ReadInt32();
                            long offset = reader.ReadInt64();
                            long size = reader.ReadInt64();
                            long zsize = reader.ReadInt64();
                            if (size != 0)
                            {
                                string fname = gene1.stringlist[idx];
                                if (zsize == 0)
                                {
                                    long tmpstreampos = reader.BaseStream.Position;
                                    reader.BaseStream.Position = offset;
                                    Console.WriteLine("File: {0} is not compressed. Writing to {1}", fname, fbd.SelectedPath + "\\" + fname);
                                    using (FileStream nfstrm = new FileStream(fbd.SelectedPath + "\\" + fname, FileMode.Create))
                                    {
                                        nfstrm.Write(reader.ReadBytes((int)size), 0, (int)size);
                                        reader.BaseStream.Position = tmpstreampos;
                                    }
                                }
                                else
                                {
                                    long tempstrmpos = reader.BaseStream.Position;
                                    reader.BaseStream.Position = offset + 2;
                                    Console.WriteLine("File: {0} is compressed. Writing to {1}", fname, fbd.SelectedPath + "\\" + fname);
                                    MemoryStream tmpmemstrm = new MemoryStream(reader.ReadBytes((int)zsize));
                                    using (FileStream cpfs = new FileStream(fbd.SelectedPath + "\\" + fname, FileMode.Create))
                                    {
                                        using (DeflateStream dfstrm = new DeflateStream(tmpmemstrm, CompressionMode.Decompress))
                                        {
                                            dfstrm.CopyTo(cpfs);
                                        }
                                    }
                                    reader.BaseStream.Position = tempstrmpos;
                                }
                            }
                        }
                        reader.BaseStream.Position = pktoc.offset + pktoc.size;
                        PACKFSLS pkfls = new PACKFSLS();
                        reader.ReadBytes(8);
                        pkfls.headerSize = reader.ReadInt64();
                        pkfls.headerOffset = reader.BaseStream.Position;
                        pkfls.archives = reader.ReadInt32();
                        pkfls.dummy = reader.ReadInt32();
                        pkfls.dummy2 = reader.ReadInt32();
                        pkfls.zero = reader.ReadInt32();
                        for (int archive = 0; archive < pkfls.archives; archive++)
                        {
                            int tmpidx = reader.ReadInt32();
                            reader.ReadInt32();
                            long archiveoffset = reader.ReadInt64();
                            long archivesize = reader.ReadInt64();
                            reader.ReadBytes(16);
                            string aname = gene1.stringlist[tmpidx];
                            long tmppos = reader.BaseStream.Position;
                            Console.WriteLine("Archive: {0}", aname);
                            reader.BaseStream.Position = tmppos;

                        }
                        //Console.Write("DEBUG");

                    }
                }
            }
            return 1;
        }

        static void extractArchives(long offset)
        {
            //TODO!!!
        }


        static GENESTRT GetGENESTRT(BinaryReader br, long headerOffset, long headerSize)
        {
            GENESTRT genetmp = new GENESTRT();
            long tmp = br.BaseStream.Position;
            long tmp2 = headerOffset + headerSize;
            br.BaseStream.Position = tmp2;
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                if (br.ReadByte() == '\0')
                {

                }
                else
                {
                    br.BaseStream.Position -= 1;
                    char tmpchar = (char)br.ReadByte();
                    if (tmpchar == 'G')
                    {
                        tmpchar = (char)br.ReadByte();
                        if (tmpchar == 'E')
                        {
                            br.BaseStream.Position -= 2;
                            if (Encoding.ASCII.GetString(br.ReadBytes(8)) == "GENESTRT")
                            {
                                genetmp.offset = br.BaseStream.Position - 8;
                                break;
                            }
                        }
                    }
                }
            }
            br.ReadBytes(8);
            genetmp.stringcnt = br.ReadInt32();
            br.ReadBytes(12);
            genetmp.nameofflist = new long[genetmp.stringcnt];
            for (int i = 0; i < genetmp.stringcnt; i++)
            {
                genetmp.nameofflist[i] = br.ReadInt32();
            }
            br.ReadBytes(12);
            genetmp.names_off = br.BaseStream.Position;
            genetmp.stringlist = new string[genetmp.stringcnt];
            for (int j = 0; j < genetmp.stringcnt; j++)
            {
                genetmp.stringlist[j] = ReadStringZ(br);
            }
            br.BaseStream.Position = tmp;
            return genetmp;
        }

        public static string ReadStringZ(BinaryReader reader)
        {
            string result = "";
            char c;
            for (int i = 0; i < reader.BaseStream.Length; i++)
            {
                if ((c = (char)reader.ReadByte()) == 0)
                {
                    break;
                }
                result += c.ToString();
            }
            return result;
        }
    }
}
