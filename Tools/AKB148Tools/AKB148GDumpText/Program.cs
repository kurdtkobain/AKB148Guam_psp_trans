#region copyright
// <copyright file="Program.cs" company="Kurdtkobain">
// Copyright (c) 2015-2017 All Rights Reserved
// </copyright>
// <author>Kurdtkobain</author>
// <date>2015/2/24 8:05:37 AM </date>
#endregion
using AKB148GASBLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AKB148GDumpText
{
    internal class Program
    {
        private static object fLock = new object();

        private static void Main(string[] args)
        {
            switch (args[0])
            {
                case "-d":
                    Dump(args[1], args[2]);
                    break;

                case "-i":
                    Insert(args[1], args[2]);
                    break;

                case "-de":
                    Dump(args[1], args[2], true);
                    break;

                case "-raw":
                    Dump(args[1], args[2], false, true);
                    break;

                case "-f":
                    if (args[1] == "-d") { FolderDump(args[2], args[3]); }
                    else if (args[1] == "-de") { FolderDump(args[2], args[3], true); }
                    else if (args[1] == "-i") { FolderInsert(args[2], args[3]); }
                    else if (args[1] == "-raw") { FolderDump(args[2], args[3], false, true); }
                    else
                    {
                        Console.WriteLine("Usage:\n");
                        Console.WriteLine("Dump: AKB148GDumpText.exe -d FILE.asb Script.txt\n");
                        Console.WriteLine("Dump (Only Event Dialog): AKB148GDumpText.exe -de FILE.asb Script.txt\n");
                        Console.WriteLine("Insert: AKB148GDumpText.exe -i Script.txt FILE.asb\n");
                    }

                    break;

                default:
                    Console.WriteLine("Usage:\n");
                    Console.WriteLine("Dump: AKB148GDumpText.exe -d FILE.asb Script.txt\n");
                    Console.WriteLine("Dump (Only Event Dialog): AKB148GDumpText.exe -d2 FILE.asb Script.txt\n");
                    Console.WriteLine("Insert: AKB148GDumpText.exe -i Script.txt FILE.asb \n");
                    break;
            }
        }

        private static void FolderInsert(string cmdFolder, string asbFolder)
        {
            string[] cmdFiles = Directory.GetFiles(cmdFolder);
            int total = cmdFiles.Count();
            int current = 0;
            Parallel.ForEach(cmdFiles, cmdfileName =>
            {
                try
                {
                    List<dialog> dList = new List<dialog>();
                    using (StreamReader file = new StreamReader(cmdfileName))
                    {
                        while (!file.EndOfStream)
                        {
                            dialog d1 = new dialog();

                            string s = file.ReadLine();
                            string[] split = s.Split(';');
                            d1.offset = Convert.ToInt64(split[0]);
                            d1.size = Convert.ToInt32(split[1]);
                            d1.text = split[2];
                            d1.text = d1.text.Replace("<LINEEND>", Encoding.UTF8.GetString(new byte[] { 0x0A }));
                            d1.text = d1.text.Replace("<END>", Encoding.UTF8.GetString(new byte[] { 0x00 }));
                            dList.Add(d1);
                        }
                    }
                    string outfile = asbFolder + "\\" + Path.GetFileNameWithoutExtension(cmdfileName) + ".asb";
                    Console.WriteLine("Writing to {0}", outfile);

                    if (!ASBTools.injectDialogList(outfile, dList))
                    {
                        Console.WriteLine("ERROR!!!!!! - Inject failed...\n");
                    }
                }
                finally
                {
                    Interlocked.Increment(ref current);
                    Console.SetCursorPosition(0, 0);
                    Console.CursorTop = 0;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("FILE: {0}\r", cmdfileName);
                    DrawProgressBar(current, total, 36, '■');
                }
            });
        }

        private static void FolderDump(string asbFolder, string outFolder, bool eventOnly = false, bool raw = false)
        {
            string[] asbFiles = Directory.GetFiles(asbFolder);
            int total = asbFiles.Count();
            int current = 1;
            Parallel.ForEach(asbFiles, asbfileName =>
            {
                try
                {
                    List<dialog> dlist;
                    if (eventOnly)
                    {
                        dlist = ASBTools.getDialogList(asbfileName, true, true);
                    }
                    else
                    {
                        if (raw)
                        {
                            dlist = ASBTools.getDialogListRAW(asbfileName, true);
                        }
                        else
                        {
                            dlist = ASBTools.getDialogList(asbfileName, true);
                        }
                    }
                    string outfile = outFolder + "\\" + Path.GetFileNameWithoutExtension(asbfileName) + ".txt";
                    using (BinaryWriter writer = new BinaryWriter(File.Open(outfile, FileMode.Create)))
                    {
                        foreach (dialog dl in dlist)
                        {
                            writer.Write(Encoding.UTF8.GetBytes(dl.offset.ToString()));
                            writer.Write(Encoding.UTF8.GetBytes(";"));
                            writer.Write(Encoding.UTF8.GetBytes(dl.size.ToString()));
                            writer.Write(Encoding.UTF8.GetBytes(";"));
                            writer.Write(Encoding.UTF8.GetBytes(dl.text));
                            writer.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                        }
                    }
                }
                finally
                {
                    Interlocked.Increment(ref current);
                    lock (fLock)
                    {
                        Console.SetCursorPosition(0, 0);
                        Console.CursorTop = 0;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("FILE: {0}\r", asbfileName);
                        DrawProgressBar(current, total, 36, '■');
                    }
                }
            });
        }

        private static void Dump(string inFile, string outFile, bool eventOnly = false, bool raw = false)
        {
            List<dialog> dlist;
            if (eventOnly)
            {
                dlist = ASBTools.getDialogList(inFile, true, true);
            }
            else
            {
                if (raw)
                {
                    dlist = ASBTools.getDialogListRAW(inFile, true);
                }
                else
                {
                    dlist = ASBTools.getDialogList(inFile, true);
                }
            }
            using (BinaryWriter writer = new BinaryWriter(File.Open(outFile, FileMode.Create)))
            {
                foreach (dialog dl in dlist)
                {
                    writer.Write(Encoding.UTF8.GetBytes(dl.offset.ToString()));
                    writer.Write(Encoding.UTF8.GetBytes(";"));
                    writer.Write(Encoding.UTF8.GetBytes(dl.size.ToString()));
                    writer.Write(Encoding.UTF8.GetBytes(";"));
                    writer.Write(Encoding.UTF8.GetBytes(dl.text));
                    writer.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                }
            }
        }

        private static void Insert(string cmdFile, string inFile)
        {
            List<dialog> dList = new List<dialog>();
            using (StreamReader file = new StreamReader(cmdFile))
            {
                while (!file.EndOfStream)
                {
                    dialog d1 = new dialog();

                    string s = file.ReadLine();
                    string[] split = s.Split(';');
                    d1.offset = Convert.ToInt64(split[0]);
                    d1.size = Convert.ToInt32(split[1]);
                    d1.text = split[2];
                    d1.text = d1.text.Replace("<LINEEND>", Encoding.UTF8.GetString(new byte[] { 0x0A }));
                    d1.text = d1.text.Replace("<END>", Encoding.UTF8.GetString(new byte[] { 0x00 }));
                    dList.Add(d1);
                }
            }
            Console.WriteLine("Writing to {0}", inFile);
            if (!ASBTools.injectDialogList(inFile, dList))
            {
                Console.WriteLine("ERROR!!!!!! - Inject failed...\n");
            }
        }

        private static void DrawProgressBar(int complete, int maxVal, int barSize, char progressCharacter)
        {
            //Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.CursorTop = 1;
            Console.CursorVisible = false;
            int left = Console.CursorLeft;
            decimal perc = complete / (decimal)maxVal;
            int chars = (int)Math.Floor(perc / (1 / (decimal)barSize));
            string p1 = String.Empty, p2 = String.Empty;

            for (int i = 0; i < chars; i++) p1 += progressCharacter;
            for (int i = 0; i < barSize - chars; i++) p2 += progressCharacter;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(p1);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(p2);

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" {0}%", (perc * 100).ToString("N2"));
            Console.CursorLeft = left;
        }
    }
}