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
using static APKUnpack.APK;

namespace APKUnpack
{
    internal class Program
    {

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
                PACKHEDR pkhdr = new PACKHEDR(reader);
                PACKTOC pktoc = new PACKTOC(reader);
                reader.BaseStream.Position = pktoc.TOCoffset + pktoc.headerSize;
                PACKFSLS pkfls = new PACKFSLS(reader);

                foreach (var entry in pktoc.entries)
                {
                    Console.WriteLine("TOC flags: {4} file: {0}  offset: {1}  size: {2}  zsize: {3}", pkfls.strList.stringlist[entry.stringIndex], entry.offset, entry.decompSize, entry.compSize, entry.flags.ToString("X4"));
                    if (entry.flags == 0x01)//directory
                    {
                        continue;
                    }
                    else
                    {
                        extractArchives(reader, entry, pkfls.strList, outfol);
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
            bread.BaseStream.Position = entry.offset;
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