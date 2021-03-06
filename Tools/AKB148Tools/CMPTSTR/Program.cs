﻿#region copyright
// <copyright file="Program.cs" company="Kurdtkobain">
// Copyright (c) 2015-2017 All Rights Reserved
// </copyright>
// <author>Kurdtkobain</author>
// <date>2015/2/24 8:31:39 PM </date>
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CMPTSTR
{
    public class chechfn
    {
        public string fName;
        public long offset;
        public int size;
    }

    internal class Program
    {
        private static object fLock = new Object();

        private static void Main(string[] args)
        {
            tst(args[0]);
        }

        private static void tst(string asbFolder)
        {
            FileStream filestream = new FileStream("out.txt", FileMode.Create);
            var streamwriter = new StreamWriter(filestream);
            streamwriter.AutoFlush = true;
            Console.SetOut(streamwriter);
            Console.SetError(streamwriter);
            List<chechfn> asblist = new List<chechfn>();
            string[] asbEntries = Directory.GetFiles(asbFolder);
            Parallel.ForEach(asbEntries, asbfileName =>
            {
                using (BinaryReader reader = new BinaryReader(File.Open(asbfileName, FileMode.Open, FileAccess.Read)))
                {
                    reader.BaseStream.Position = 52;
                    long tmpl;
                    int tmps;
                    if (BitConverter.IsLittleEndian)
                    {
                        tmpl = reader.ReadInt16();
                        reader.BaseStream.Position = 60;
                        tmps = reader.ReadInt16();
                    }
                    else
                    {
                        byte[] tmpb = new byte[4];
                        tmpb = reader.ReadBytes(2);
                        Array.Reverse(tmpb);
                        tmpl = BitConverter.ToInt16(tmpb, 0);
                        reader.BaseStream.Position = 60;
                        byte[] tmpb2 = new byte[4];
                        tmpb2 = reader.ReadBytes(2);
                        Array.Reverse(tmpb2);
                        tmps = BitConverter.ToInt16(tmpb2, 0);
                    }
                    chechfn asbtmp = new chechfn();
                    asbtmp.fName = Path.GetFileNameWithoutExtension(asbfileName);
                    asbtmp.offset = tmpl;
                    asbtmp.size = tmps;
                    lock (fLock)
                    {
                        asblist.Add(asbtmp);
                    }
                }
            });
            Parallel.ForEach(asblist, cfn =>
            {
                foreach (chechfn asbcfn in asblist)
                {
                    if (cfn.fName != asbcfn.fName)
                    {
                        if (cfn.offset == asbcfn.offset && cfn.size == asbcfn.size)
                        {
                            Console.WriteLine("{1} AND {0} MATCH!!!!!", asbcfn.fName, cfn.fName);
                        }
                        else
                        {
                        }
                    }
                }
            });
            filestream.Close();
        }
    }
}