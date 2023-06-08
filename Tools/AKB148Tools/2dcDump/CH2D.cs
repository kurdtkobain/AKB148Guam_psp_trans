using MiscUtil.IO;
using MiscUtil.Conversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2dcDump
{
    public class CH2D
    {
        public static byte[] ENDIBIGE = new byte[16];
        public class CH2DHEAD //32 bytes
        {
            public byte[] tag = new byte[8];
            public int ukn1;
            public int ukn2;
            public int ukn3;
            public int ukn4;
            public byte[] padding = new byte[8]; //always all 0xCC

            public CH2DHEAD(EndianBinaryReader br)
            {
                tag = br.ReadBytes(8);
                ukn1 = br.ReadInt32();
                ukn2 = br.ReadInt32();
                ukn3 = br.ReadInt32();
                ukn4 = br.ReadInt32();
                padding = br.ReadBytes(8);

            }

            public string getInfoString()
            {
                StringBuilder ret = new StringBuilder();
                ret.AppendLine(Encoding.ASCII.GetString(tag));
                ret.AppendLine("0x" + ukn1.ToString("X4") + " " + ukn1.ToString());
                ret.AppendLine("0x" + ukn2.ToString("X4") + " " + ukn2.ToString());
                ret.AppendLine("0x" + ukn3.ToString("X4") + " " + ukn3.ToString());
                ret.AppendLine("0x" + ukn4.ToString("X4") + " " + ukn4.ToString());
                ret.AppendLine(KAMY.Extensions.MiscExtensions.ToHexString(padding));
                return ret.ToString();
            }

        }
        public class CH2DHDET //32 bytes
        {
            public byte[] tag = new byte[8];
            public int ukn1;
            public int ukn2;
            public int ukn3;
            public int ukn4;
            public byte[] padding = new byte[8]; //always all 0xCC

            public CH2DHDET(EndianBinaryReader br)
            {
                tag = br.ReadBytes(8);
                ukn1 = br.ReadInt32();
                ukn2 = br.ReadInt32();
                ukn3 = br.ReadInt32();
                ukn4 = br.ReadInt32();
                padding = br.ReadBytes(8);
            }

            public string getInfoString()
            {
                StringBuilder ret = new StringBuilder();
                ret.AppendLine(Encoding.ASCII.GetString(tag));
                ret.AppendLine("0x" + ukn1.ToString("X4") + " " + ukn1.ToString());
                ret.AppendLine("0x" + ukn2.ToString("X4") + " " + ukn2.ToString());
                ret.AppendLine("0x" + ukn3.ToString("X4") + " " + ukn3.ToString());
                ret.AppendLine("0x" + ukn4.ToString("X4") + " " + ukn4.ToString());
                ret.AppendLine(KAMY.Extensions.MiscExtensions.ToHexString(padding));
                return ret.ToString();
            }

        }
        public class CH2DCHAR //48 bytes
        {
            public byte[] tag = new byte[8];
            public int ukn1;
            public int ukn2;
            public byte[] ukn3 = new byte[3];
            public byte[] padding = new byte[29]; //always all 0xCC

            public CH2DCHAR(EndianBinaryReader br)
            {
                tag = br.ReadBytes(8);
                ukn1 = br.ReadInt32();
                ukn2 = br.ReadInt32();
                ukn3 = br.ReadBytes(3);
                padding = br.ReadBytes(29);
            }

            public string getInfoString()
            {
                StringBuilder ret = new StringBuilder();
                ret.AppendLine(Encoding.ASCII.GetString(tag));
                ret.AppendLine("0x" + ukn1.ToString("X4") + " " + ukn1.ToString());
                ret.AppendLine("0x" + ukn2.ToString("X4") + " " + ukn2.ToString());
                ret.AppendLine(KAMY.Extensions.MiscExtensions.ToHexString(ukn3));
                ret.AppendLine(KAMY.Extensions.MiscExtensions.ToHexString(padding));
                return ret.ToString();
            }

        }
        public class CH2DPOSE
        {
            public byte[] tag = new byte[8];
            public int ukn1;
            public byte[] poseId = new byte[13];
            public byte[] padding = new byte[23]; //always all 0xCC
            public int ukn2;
            public int ukn3;
            public byte[] padding2 = new byte[8]; //always all 0xCC

            public CH2DPOSE(EndianBinaryReader br)
            {
                tag = br.ReadBytes(8);
                ukn1 = br.ReadInt32();
                poseId = br.ReadBytes(13);
                padding = br.ReadBytes(23);
                ukn2 = br.ReadInt32();
                ukn3 = br.ReadInt32();
                padding2 = br.ReadBytes(8);
            }

            public string getInfoString()
            {
                StringBuilder ret = new StringBuilder();
                ret.AppendLine(Encoding.ASCII.GetString(tag));
                ret.AppendLine("0x" + ukn1.ToString("X4") + " " + ukn1.ToString());
                ret.AppendLine(Encoding.ASCII.GetString(poseId).Trim((char)0x00));
                ret.AppendLine(KAMY.Extensions.MiscExtensions.ToHexString(padding));
                ret.AppendLine("0x" + ukn2.ToString("X4") + " " + ukn2.ToString());
                ret.AppendLine("0x" + ukn3.ToString("X4") + " " + ukn3.ToString());
                ret.AppendLine(KAMY.Extensions.MiscExtensions.ToHexString(padding2));
                return ret.ToString();
            }

        }
        public class CH2DPLAN
        {
            public byte[] tag;
            public int ukn1;
            public int ukn2;
            public short ukn3;
            public byte[] padding = new byte[30]; //always all 0xCC
            public int ukn4;
            public int ukn5;
            public int finalimageWidth;
            public int finalimageHeight;
            public byte[] padding2 = new byte[16]; //always all 0xCC

            public CH2DPLAN(EndianBinaryReader br)
            {
                tag = br.ReadBytes(8);
                ukn1 = br.ReadInt32();
                ukn2 = br.ReadInt32();
                ukn3 = br.ReadInt16();
                padding = br.ReadBytes(30);
                ukn4 = br.ReadInt32();
                ukn5 = br.ReadInt32();
                finalimageWidth = br.ReadInt32();
                finalimageHeight = br.ReadInt32();
                padding2 = br.ReadBytes(16);
            }

            public string getInfoString()
            {
                StringBuilder ret = new StringBuilder();
                ret.AppendLine(Encoding.ASCII.GetString(tag));
                ret.AppendLine("0x" + ukn1.ToString("X4") + " " + ukn1.ToString());
                ret.AppendLine("0x" + ukn2.ToString("X4") + " " + ukn2.ToString());
                ret.AppendLine("0x" + ukn3.ToString("X4") + " " + ukn3.ToString());
                ret.AppendLine(KAMY.Extensions.MiscExtensions.ToHexString(padding));
                ret.AppendLine("0x" + ukn4.ToString("X4") + " " + ukn4.ToString());
                ret.AppendLine("0x" + ukn5.ToString("X4") + " " + ukn5.ToString());
                ret.AppendLine("0x" + finalimageHeight.ToString("X4") + " " + finalimageHeight.ToString());
                ret.AppendLine("0x" + finalimageWidth.ToString("X4") + " " + finalimageWidth.ToString());
                ret.AppendLine(KAMY.Extensions.MiscExtensions.ToHexString(padding2));
                return ret.ToString();
            }

        }
        public class CH2DTEXI
        {
            public byte[] tag = new byte[8];
            public int size1;
            public int diff;
            public int size2;
            public byte[] padding = new byte[12]; //always all 0xCC

            public CH2DTEXI(EndianBinaryReader br)
            {
                tag = br.ReadBytes(8);
                size1 = br.ReadInt32();
                diff = br.ReadInt32();
                size2 = br.ReadInt32();
                padding = br.ReadBytes(12);

            }

            public string getInfoString()
            {
                StringBuilder ret = new StringBuilder();
                ret.AppendLine(Encoding.ASCII.GetString(tag));
                ret.AppendLine("0x" + size1.ToString("X4") + " " + size1.ToString());
                ret.AppendLine("0x" + diff.ToString("X4") + " " + diff.ToString());
                ret.AppendLine("0x" + size2.ToString("X4") + " " + size2.ToString());
                ret.AppendLine(KAMY.Extensions.MiscExtensions.ToHexString(padding));
                return ret.ToString();
            }

        }

        public class IMGDATA
        {
            public long startpos;
            public int zero1;
            public int zero2;
            public int ukn4;
            public int ukn5;
            public int ukn6;
            public int ukn7;
            public int ukn8;
            public short width;
            public short height;
            public short end; //always 0x8080
            public long endpos;
            public IMGDATA(string file, in long pos, out long pos2)
            {
                using (EndianBinaryReader br2 = new EndianBinaryReader(EndianBitConverter.Little, new FileStream(file, FileMode.Open)))
                {
                    br2.BaseStream.Position = startpos = pos;
                    zero1 = br2.ReadInt32();
                    zero2 = br2.ReadInt32();
                    ukn4 = br2.ReadInt32();
                    ukn5 = br2.ReadInt32();
                    ukn6 = br2.ReadInt32();
                    ukn7 = br2.ReadInt32();
                    ukn8 = br2.ReadInt32();
                    width = br2.ReadInt16();
                    height = br2.ReadInt16();
                    end = br2.ReadInt16();
                    pos2 = endpos = br2.BaseStream.Position;
                }
            }

            public string getInfoString()
            {
                StringBuilder ret = new StringBuilder();
                ret.AppendLine("IMGDATA");
                ret.AppendLine("0x" + startpos.ToString("X4") + " " + startpos.ToString());
                ret.AppendLine("0x" + zero1.ToString("X4"));
                ret.AppendLine("0x" + zero2.ToString("X4"));
                ret.AppendLine("0x" + ukn4.ToString("X4") + " " + ukn4.ToString());
                ret.AppendLine("0x" + ukn5.ToString("X4") + " " + ukn5.ToString());
                ret.AppendLine("0x" + ukn6.ToString("X4") + " " + ukn6.ToString());
                ret.AppendLine("0x" + ukn7.ToString("X4") + " " + ukn7.ToString());
                ret.AppendLine("0x" + ukn8.ToString("X4") + " " + ukn8.ToString());
                ret.AppendLine(width.ToString());
                ret.AppendLine(height.ToString());
                ret.AppendLine("0x" + end.ToString("X4") + " " + end.ToString());
                ret.AppendLine("0x" + endpos.ToString("X4") + " " + endpos.ToString());
                return ret.ToString();
            }
        }
    }
}
