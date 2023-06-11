using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static GTFDump.gtfinfo;
using static GTFDump.ddsinfo;
using static GTFDump.utils;

namespace GTFDump
{
    internal class Program
    {
        //info from here https://github.com/Zarh/ManaGunZ/tree/master/MGZ/lib/libgtfconv

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }
            if (args[0] == "-d")
            {

                string[] files = Directory.GetFiles(args[1]);
                foreach (string file in files)
                {
                    if (file.Contains("font"))
                    {
                        continue;
                    }
                    gtfddscov(args[1], file);
                }

            }
            else if (args[0] == "-f")
            {
                gtfddscov(new FileInfo(args[1]).Directory.FullName, args[1]);

            }
            //Console.WriteLine("Done. Press any key to exit.");
            //Console.ReadLine();

        }

        unsafe static void gtfddscov(string path, string file)
        {
            Console.WriteLine($"Converting {file}");
            using (MemoryStream filems = new MemoryStream())
            {
                using (FileStream fs = File.OpenRead(file))
                {
                    fs.CopyTo(filems);
                }
                filems.Position = 0;
                using (EndianBinaryReader br = new EndianBinaryReader(EndianBitConverter.Big, filems))
                {
                    CellGtfFileHeader header = new CellGtfFileHeader();
                    List<CellGtfTextureAttribute> attribs = new List<CellGtfTextureAttribute>();
                    header.version = br.ReadUInt32();
                    header.size = br.ReadUInt32();
                    header.numTexture = br.ReadUInt32();
                    for (int i = 0; i < header.numTexture; i++)
                    {
                        CellGtfTextureAttribute attrb = new CellGtfTextureAttribute();
                        attrb.id = br.ReadUInt32();
                        attrb.offsetToTex = br.ReadUInt32();
                        attrb.textureSize = br.ReadUInt32();
                        attrb.tex.format = br.ReadByte();
                        attrb.tex.mipmap = br.ReadByte();
                        attrb.tex.dimension = br.ReadByte();
                        attrb.tex.cubemap = br.ReadByte();
                        attrb.tex.remap = br.ReadUInt32();
                        attrb.tex.width = br.ReadUInt16();
                        attrb.tex.height = br.ReadUInt16();
                        attrb.tex.depth = br.ReadUInt16();
                        attrb.tex.location = br.ReadByte();
                        attrb.tex._padding = br.ReadByte();
                        attrb.tex.pitch = br.ReadUInt32();
                        attrb.tex.offset = br.ReadUInt32();
                        attribs.Add(attrb);
                    }
                    foreach (CellGtfTextureAttribute attrb in attribs)
                    {
                        gtfutil.gtfCheckSpec(attrb.tex);
                        br.BaseStream.Position = attrb.offsetToTex;
                        byte[] image = br.ReadBytes((int)attrb.textureSize);
                        CellUtilDDSHeader cellUtilDDSHeader = new CellUtilDDSHeader();
                        gtf2dds.gtf2ddsConvHeader(ref cellUtilDDSHeader, attrb.tex);

                        ddsutil.ddsCheckSpec(cellUtilDDSHeader);

                        // calc layout num
                        byte cube = 1;
                        if (attrb.tex.cubemap == CELL_GCM_TRUE)
                        {
                            cube = 6;
                        }

                        uint layout_num = (uint)(cube * attrb.tex.mipmap);

                        layout_t[] layout_array = new layout_t[layout_num];

                        convert.convertLayOutBuffer(ref layout_array, attrb.tex, cellUtilDDSHeader);

                        byte[] gtf = filems.ToArray();

                        Stream filestr = File.OpenWrite(path + "\\dds\\" + Path.GetFileNameWithoutExtension(file) + ".dds");

                        byte[] ddsfile;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (BinaryWriter memoryStream = new BinaryWriter(ms))
                            {
                                memoryStream.Write(cellUtilDDSHeader.magic);
                                memoryStream.Write(cellUtilDDSHeader.size);
                                memoryStream.Write(cellUtilDDSHeader.flags);
                                memoryStream.Write(cellUtilDDSHeader.height);
                                memoryStream.Write(cellUtilDDSHeader.width);
                                memoryStream.Write(cellUtilDDSHeader.pitchOrLinearSize);
                                memoryStream.Write(cellUtilDDSHeader.depth);
                                memoryStream.Write(cellUtilDDSHeader.mipMapCount);
                                memoryStream.Write(cellUtilDDSHeader.reserved1.SelectMany(BitConverter.GetBytes).ToArray());
                                memoryStream.Write(cellUtilDDSHeader.ddspf.size);
                                memoryStream.Write(cellUtilDDSHeader.ddspf.flags);
                                memoryStream.Write(cellUtilDDSHeader.ddspf.fourCC);
                                memoryStream.Write(cellUtilDDSHeader.ddspf.rgbBitCount);
                                memoryStream.Write(cellUtilDDSHeader.ddspf.rbitMask);
                                memoryStream.Write(cellUtilDDSHeader.ddspf.gbitMask);
                                memoryStream.Write(cellUtilDDSHeader.ddspf.bbitMask);
                                memoryStream.Write(cellUtilDDSHeader.ddspf.abitMask);
                                memoryStream.Write(cellUtilDDSHeader.caps1);
                                memoryStream.Write(cellUtilDDSHeader.caps2);
                                memoryStream.Write(cellUtilDDSHeader.reserved2.SelectMany(BitConverter.GetBytes).ToArray());
                                memoryStream.Write(image);
                                memoryStream.Flush();
                                ddsfile = ms.ToArray();
                            }
                        }

                        convert.convertBufferByLayout(ref gtf,ref ddsfile,ref layout_array, layout_num, attrb.tex, false);



                        using (BinaryWriter bwr = new BinaryWriter(filestr))
                        {
                            bwr.Write(ddsfile);
                        }
                    }
                }
            }
        }

    }
}
