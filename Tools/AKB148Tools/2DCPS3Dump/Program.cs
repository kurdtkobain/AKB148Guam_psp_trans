using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace _2DCPS3Dump
{
    internal class Program
    {

        private class CH2DTEXI
        {
            public byte[] tag = new byte[8]; //CH2DTEXI
            public int size1;
            public int diff;
            public int size2;
            public int ukn1;
            public int ukn2;
            public int ukn3;
            public int ukn4;
            public int ukn5;
            public int ukn6;
            public int ukn7;
        };
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                if (args[0] == "-f")
                {
                    dump(args[1], args[2]);
                }
                if (args[0] == "-d")
                {
                    string[] files = Directory.GetFiles(args[1], "*.2dc");
                    foreach (string file in files)
                    {
                        dump(file, args[2]);
                    }
                }
            }
        }

        private static void dump(string file, string outDir)
        {
            Console.WriteLine($"Processing: {file}");
            BoyerMooreBinarySearch bm = new BoyerMooreBinarySearch(Encoding.ASCII.GetBytes("CH2DTEXI"));
            int idx = 0;

            using (BinaryReader br = new BinaryReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                
                var offsets = bm.GetMatchIndexes(br.BaseStream);
                foreach (long offset in offsets)
                {
                    string outFile;
                    if (Path.GetFileNameWithoutExtension(file).StartsWith("tc") || Path.GetFileNameWithoutExtension(file).StartsWith("krm"))
                    {
                        outFile = $"{outDir}\\{Path.GetFileNameWithoutExtension(file)}\\{Path.GetFileNameWithoutExtension(file)}-{idx}.jpga";
                    }
                    else
                    {
                        outFile = $"{outDir}\\{Path.GetFileNameWithoutExtension(file).Split('_')[0]}\\{Path.GetFileNameWithoutExtension(file).Split('_')[1]}\\{Path.GetFileNameWithoutExtension(file)}-{idx}.jpga";

                    }
                    if (File.Exists(outFile))
                    {
                        Console.WriteLine($"{outFile} exists skipping.....");
                        idx++;
                        continue;
                    }
                    br.BaseStream.Position = offset;
                    CH2DTEXI head = new CH2DTEXI();
                    head.tag = br.ReadBytes(8);
                    head.size1 = br.ReadInt32();
                    head.diff = br.ReadInt32();
                    head.size2 = br.ReadInt32();
                    head.ukn1 = br.ReadInt32();
                    head.ukn2 = br.ReadInt32();
                    head.ukn3 = br.ReadInt32();
                    head.ukn4 = br.ReadInt32();
                    head.ukn5 = br.ReadInt32();
                    head.ukn6 = br.ReadInt32();
                    head.ukn7 = br.ReadInt32();

                    byte[] jpgaData = br.ReadBytes(head.size2 + 2);
                    

                    Console.WriteLine($"Writing file: {outFile}");

                    if (!Directory.Exists(outFile))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(outFile));
                    }
                    File.WriteAllBytes(outFile, jpgaData);
                    idx++;
                }
                
            }

        }
    }
}
