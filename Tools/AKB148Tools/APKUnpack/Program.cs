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
using System.Text;
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
            public string dummyFill;
        }

        private class PACKTOC
        {
            public long headerSize;
            public long TOCoffset;
            public int entrySize;
            public long count;
            public uint offset;
            public long zero;
        }

        private class PACKTOCENTRY
        {
            public int flags;
            public int stringIndex;
            public int headerIndex;
            public int zero0;
            public uint offset;
            public int count;
            public long decompSize;
            public long compSize;
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
                Console.WriteLine("Usage: APKUnpack [APK_FILE] [OUTPUT_FOLDER]");
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
                reader.ReadBytes(16); //"ENDILTLE"
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
                pkhdr.dummyFill = Encoding.ASCII.GetString(reader.ReadBytes(16));
                if (Encoding.ASCII.GetString(reader.ReadBytes(8)) != "PACKTOC ")
                {
                    return -1;
                }
                PACKTOC pktoc = new PACKTOC();
                pktoc.headerSize = reader.ReadInt64();
                pktoc.TOCoffset = reader.BaseStream.Position;
                pktoc.entrySize = reader.ReadInt32();
                pktoc.count = reader.ReadInt32();
                pktoc.offset = reader.ReadUInt32();
                pktoc.zero = reader.ReadInt32();
                List<PACKTOCENTRY> entries = new List<PACKTOCENTRY>();
                //reader.BaseStream.Position = pktoc.TOCoffset;
                for (int l = 0; l < pktoc.count; l++)
                {
                    var entry = new PACKTOCENTRY();
                    //long tocpos = reader.BaseStream.Position;
                    entry.flags = reader.ReadInt32(); //always 0x200 (512)
                    entry.stringIndex = reader.ReadInt32();
                    entry.headerIndex = reader.ReadInt32(); //always 0
                    entry.zero0 = reader.ReadInt32(); //always 0
                    entry.offset = reader.ReadUInt32();
                    entry.count = reader.ReadInt32();
                    entry.decompSize = reader.ReadInt64();
                    entry.compSize = reader.ReadInt64();
                    entries.Add(entry);
                }
                reader.BaseStream.Position = pktoc.TOCoffset + pktoc.headerSize;
                if (Encoding.ASCII.GetString(reader.ReadBytes(8)) != "PACKFSLS")
                {
                    return -1;
                }
                PACKFSLS pkfls = new PACKFSLS();
                pkfls.headerSize = reader.ReadInt64();
                pkfls.headerOffset = reader.BaseStream.Position;
                pkfls.dummy = reader.ReadInt32();
                pkfls.archives = reader.ReadInt32();
                pkfls.dummy2 = reader.ReadInt32();
                pkfls.zero = reader.ReadInt32();
                GENESTRT gene1 = GetGENESTRT(reader);

                foreach (var entry in entries)
                {
                    Console.WriteLine("TOC flags: {4} file: {0}  offset: {1}  size: {2}  zsize: {3}", gene1.stringlist[entry.stringIndex], entry.offset, entry.decompSize, entry.compSize, entry.flags.ToString("X4"));
                    if (entry.flags == 0x01)//directory
                    {
                        continue;
                    }
                    else
                    {
                        extractArchives(reader, entry, gene1, outfol);
                    }
                }
            }
            return 1;
        }

        private static void extractArchives(EndianBinaryReader bread, PACKTOCENTRY entry, GENESTRT arcgene, string outfol)
        {
            string fname = arcgene.stringlist[entry.stringIndex];
            string sortFol = Path.GetExtension(fname).Remove(0, 1);
            FileInfo fileInfo = new FileInfo(outfol + "\\" + sortFol + "\\");

            if (!fileInfo.Exists)
                Directory.CreateDirectory(fileInfo.Directory.FullName);

            if (entry.flags == 0x01)//directory
            {
                return;
            }
            if (entry.flags == 0x0300)
            {
                using (MemoryStream tmpmemstrm = new MemoryStream(bread.ReadBytes((int)entry.compSize)))
                {
                    DecompressWriteAsync(tmpmemstrm, outfol, sortFol + "\\" + fname);//why async ¯\(°_°)/¯ cuz I like the word.......
                }
            }
            else if (entry.flags == 0x0200)
            {
                using (MemoryStream tmpmemstrm = new MemoryStream(bread.ReadBytes((int)entry.compSize)))
                {
                    DecompressWriteAsyncZLib(tmpmemstrm, outfol, sortFol + "\\" + fname);//why async ¯\(°_°)/¯ cuz I like the word.......
                }
            }
            else
            {
                using (FileStream nfstrm = new FileStream(outfol + "\\" + sortFol + "\\" + fname, FileMode.Create))
                {
                    nfstrm.WriteAsync(bread.ReadBytes((int)entry.decompSize), 0, (int)entry.decompSize);//why async ¯\(°_°)/¯ cuz I like the word.......
                }
            }
        }

        private static GENESTRT GetGENESTRT(EndianBinaryReader br)
        {
            GENESTRT genetmp = new GENESTRT();
            if (Encoding.ASCII.GetString(br.ReadBytes(8)) != "GENESTRT")
            {
                throw new Exception();
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
                lzcomp.Decompress(ms, cpfs);
            }
        }

        private static void DecompressWriteAsyncZLib(MemoryStream ms, string path, string f_name)
        {
            using (FileStream cpfs = new FileStream(path + "\\" + f_name, FileMode.Create))
            {
                using (DeflateStream dfstrm = new DeflateStream(ms, CompressionMode.Decompress))
                {
                    dfstrm.CopyTo(cpfs);
                }
            }
        }
    }
}