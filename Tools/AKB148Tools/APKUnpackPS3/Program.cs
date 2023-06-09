#region copyright
// <copyright file="Program.cs" company="Kurdtkobain">
// Copyright (c) 2015-2017 All Rights Reserved
// </copyright>
// <author>Kurdtkobain</author>
// <date>2015/2/28 3:45:29 AM </date>
#endregion
using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace APKUnpack
{
    internal class Program
    {
        private class GENESTRT
        {
            public long offset;
            public long stringcnt;
            public long[] nameofflist;
            public long names_off;
            public string[] stringlist;
        }

        private class PACKHEDR
        {
            public long headerSize;
            public long dummy;
            public long zero;
            public long dummySize;
            public long dummy2;
            public string dummystring;
        }

        private class PACKTOC
        {
            public long size;
            public long offset;
            public int entrySize;
            public long files;
            public int folders;
            public long zero;
        }

        private class PACKFSLS
        {
            public long headerSize;
            public long headerOffset;
            public long archives;
            public long dummy;
            public long dummy2;
            public long zero;
        }
        /*
        class FOLDERS
        {
            public List<byte[]> dummystring = new List<byte[]>();
            public List<int> nameIdx = new List<int>();
            public List<int> fileNum = new List<int>();
            public List<int> subFiles = new List<int>();
            public string[] foldernames;
            public FOLDERS(int num)
            {
                foldernames = new string[num];
            }
        }
        */

        [STAThread]
        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                OpenFileDialog fd = new OpenFileDialog();
                fd.Filter = "AKB 1/148 PS3 apk files|*.apk";

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    fbd.SelectedPath = "C:\\";
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        extract(fd.FileName, fbd.SelectedPath);
                    }
                }
            }
            else if (args.Length == 2)
            {
                extract(args[0], args[1], false);
            }
            else
            {
                Console.WriteLine("Usage: APKUnpackPS3 [APK_FILE] [OUTPUT_FOLDER]");
            }
            return 1;
        }

        private static int extract(string infile, string outfol, bool logtofile = true)
        {
            if (logtofile)
            {
                FileStream filestream = new FileStream("out.txt", FileMode.Create);
                var streamwriter = new StreamWriter(filestream);
                streamwriter.AutoFlush = true;
                Console.SetOut(streamwriter);
                Console.SetError(streamwriter);
            }
            using (EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Little, File.Open(infile, FileMode.Open)))
            {
                reader.ReadBytes(16);
                if (Encoding.ASCII.GetString(reader.ReadBytes(8)) != "PACKHEDR")
                {
                    return -1;
                }
                PACKHEDR pkhdr = new PACKHEDR();
                pkhdr.headerSize = reader.ReadInt64();
                pkhdr.dummy = reader.ReadInt32();
                pkhdr.zero = reader.ReadInt32();
                pkhdr.dummySize = reader.ReadInt32();
                pkhdr.dummy2 = reader.ReadInt32();
                pkhdr.dummystring = Encoding.ASCII.GetString(reader.ReadBytes(16));
                if (Encoding.ASCII.GetString(reader.ReadBytes(8)) != "PACKTOC ")
                {
                    return -1;
                }
                PACKTOC pktoc = new PACKTOC();
                pktoc.size = reader.ReadInt64();
                pktoc.offset = reader.BaseStream.Position;
                pktoc.entrySize = reader.ReadInt32();
                pktoc.files = reader.ReadInt32();
                pktoc.folders = reader.ReadInt32();
                pktoc.zero = reader.ReadInt32();
                GENESTRT gene1 = GetGENESTRT(reader, pktoc.offset, pktoc.size);
                //FOLDERS fldrs = new FOLDERS(pktoc.folders);
                for (int k = 0; k < pktoc.folders; k++)
                {
                    /*
                    fldrs.dummystring.Add(reader.ReadBytes(pktoc.entrySize));
                    fldrs.nameIdx.Add(fldrs.dummystring[k].ElementAt(4));
                    fldrs.fileNum.Add(fldrs.dummystring[k].ElementAt(16));
                    fldrs.subFiles.Add(fldrs.dummystring[k].ElementAt(20));
                    fldrs.foldernames[k] = gene1.stringlist[fldrs.nameIdx[k]];
                    */

                    reader.ReadBytes(pktoc.entrySize);
                }
                for (int l = pktoc.folders; l < pktoc.files; l++)
                {
                    long tocpos = reader.BaseStream.Position;
                    reader.ReadInt32(); //always 0x200 (512)
                    int idx = reader.ReadInt32();
                    reader.ReadInt32(); //always 0
                    reader.ReadInt32(); //always 0
                    long offset = reader.ReadInt64();
                    long size = reader.ReadInt64();
                    long zsize = reader.ReadInt64();
                    Console.WriteLine("TOC offset: {4}  file: {0}  offset: {1}  size: {2}  zsize: {3}", gene1.stringlist[idx], offset, size, zsize, tocpos);
                    if (size != 0)
                    {
                        string fname = gene1.stringlist[idx];
                        string sortFol = Path.GetExtension(fname).Remove(0,1);
                        FileInfo fileInfo = new FileInfo(outfol + "\\" + sortFol + "\\");

                        if (!fileInfo.Exists)
                            Directory.CreateDirectory(fileInfo.Directory.FullName);
                        if (zsize == 0)
                        {
                            long tmpstreampos = reader.BaseStream.Position;
                            reader.BaseStream.Position = offset;
                            //Console.WriteLine("File: {0} is not compressed. Writing to {1}", fname, outfol + "\\" + fname);
                            
                            using (FileStream nfstrm = new FileStream(outfol + "\\" + sortFol + "\\" + fname, FileMode.Create))
                            {
                                nfstrm.WriteAsync(reader.ReadBytes((int)size), 0, (int)size);//why async ¯\(°_°)/¯ cuz I like the word.......
                                reader.BaseStream.Position = tmpstreampos;
                            }
                        }
                        else
                        {
                            long tempstrmpos = reader.BaseStream.Position;
                            reader.BaseStream.Position = offset;// + 2;
                            //Console.WriteLine("File: {0} is compressed. Writing to {1}", fname, outfol + "\\" + fname);
                            using (MemoryStream tmpmemstrm = new MemoryStream(reader.ReadBytes((int)zsize)))
                            {
                                DecompressWriteAsync(tmpmemstrm, outfol, sortFol + "\\" + fname);//why async ¯\(°_°)/¯ cuz I like the word.......
                            }
                            reader.BaseStream.Position = tempstrmpos;
                        }
                    }
                }
                reader.BaseStream.Position = pktoc.offset + pktoc.size;
                if (Encoding.ASCII.GetString(reader.ReadBytes(8)) != "PACKFSLS")
                {
                    return -1;
                }
                PACKFSLS pkfls = new PACKFSLS();
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
                    extractArchives(reader, archiveoffset, aname, outfol);
                    reader.BaseStream.Position = tmppos;
                }
            }
            return 1;
        }

        private static void extractArchives(EndianBinaryReader bread, long offset, string archname, string path)
        {
            bread.BaseStream.Position = offset;
            bread.ReadBytes(8);
            bread.ReadBytes(8);
            long tmppos = bread.BaseStream.Position;
            bread.ReadBytes(8);
            long header_size = bread.ReadInt64();
            long header_offset = bread.BaseStream.Position;
            bread.ReadInt32();
            int entry_size = bread.ReadInt32();
            int files = bread.ReadInt32();
            int entries_size = bread.ReadInt32();
            bread.ReadBytes(16);
            GENESTRT arcgene = GetGENESTRT(bread, header_offset, header_size);
            for (int i = 0; i < files; i++)
            {
                int name_idx = bread.ReadInt32();
                int zip = bread.ReadInt32();
                long f_offset = bread.ReadInt64();
                long f_size = bread.ReadInt64();
                long f_zsize = bread.ReadInt64();
                string f_name = archname + "/" + arcgene.stringlist[name_idx];
                f_offset += offset;
                FileInfo fileInfo = new FileInfo(path + "/" + f_name);

                if (!fileInfo.Exists)
                    Directory.CreateDirectory(fileInfo.Directory.FullName);
                if (f_zsize == 0)
                {
                    long tmpstreampos = bread.BaseStream.Position;
                    bread.BaseStream.Position = f_offset;
                    Console.WriteLine("File: {0} is not compressed. Writing to {1}", f_name, path + "\\" + f_name);
                    using (FileStream nfstrm = new FileStream(path + "/" + f_name, FileMode.Create))
                    {
                        nfstrm.WriteAsync(bread.ReadBytes((int)f_size), 0, (int)f_size);//why async ¯\(°_°)/¯ cuz I like the word.......
                        bread.BaseStream.Position = tmpstreampos;
                    }
                }
                else
                {
                    long tempstrmpos = bread.BaseStream.Position;
                    bread.BaseStream.Position = f_offset;// + 2;
                    Console.WriteLine("File: {0} is compressed. Writing to {1}", f_name, path + "\\" + f_name);
                    using (MemoryStream tmpmemstrm = new MemoryStream(bread.ReadBytes((int)f_zsize)))
                    {
                        DecompressWriteAsync(tmpmemstrm, path, f_name);//why async ¯\(°_°)/¯ cuz I like the word.......
                    }
                    bread.BaseStream.Position = tempstrmpos;
                }
            }
        }

        private static GENESTRT GetGENESTRT(EndianBinaryReader br, long headerOffset, long headerSize)
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
            br.ReadInt32();
            br.ReadInt32();
            long headend = br.BaseStream.Position;
            genetmp.stringcnt = br.ReadInt32();
            br.ReadInt32();
            long genehsize = br.ReadInt32();
            br.ReadInt32();
            genetmp.nameofflist = new long[genetmp.stringcnt];
            for (int i = 0; i < genetmp.stringcnt; i++)
            {
                genetmp.nameofflist[i] = br.ReadInt32();
            }
            br.BaseStream.Position = headend + genehsize;
            genetmp.names_off = br.BaseStream.Position;
            genetmp.stringlist = new string[genetmp.stringcnt];
            for (int j = 0; j < genetmp.stringcnt; j++)
            {
                genetmp.stringlist[j] = ReadStringZ(br);
            }
            br.BaseStream.Position = tmp;
            return genetmp;
        }

        public static string ReadStringZ(EndianBinaryReader reader)
        {
            string result = "";
            char c;
            for (int i = 0; i < 255; i++)
            {
                if ((c = (char)reader.ReadByte()) == 0)
                {
                    break;
                }
                result += c.ToString();
            }
            return result;
        }

        private static void DecompressWriteAsync(MemoryStream ms, string path, string f_name)
        {
            using (FileStream cpfs = new FileStream(path + "\\" + f_name, FileMode.Create))
            {
                EasyCompressor.LZMACompressor lzcomp = new EasyCompressor.LZMACompressor();
                lzcomp.DecompressAsync(ms, cpfs); //why async ¯\(°_°)/¯ cuz I like the word.......
                //cpfs.Flush();
            }
        }
    }
}