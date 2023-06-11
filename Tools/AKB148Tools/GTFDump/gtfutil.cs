using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static GTFDump.gtfinfo;

namespace GTFDump
{
    internal class gtfutil
    {

        public static bool gtfIsSwizzle(byte format)
        {
            return Convert.ToBoolean(format & CELL_GCM_TEXTURE_LN);
        }

        public static int gtfGetPitch(byte format, int width)
        {
            int depth = 0;
            byte raw_format = gtfGetRawFormat(format);
            switch (raw_format)
            {
                case CELL_GCM_TEXTURE_B8:
                    depth = 1;
                    break;
                case CELL_GCM_TEXTURE_A1R5G5B5:
                case CELL_GCM_TEXTURE_A4R4G4B4:
                case CELL_GCM_TEXTURE_R5G6B5:
                case CELL_GCM_TEXTURE_G8B8:
                case CELL_GCM_TEXTURE_R6G5B5:
                case CELL_GCM_TEXTURE_DEPTH16:
                case CELL_GCM_TEXTURE_DEPTH16_FLOAT:
                case CELL_GCM_TEXTURE_X16:
                case CELL_GCM_TEXTURE_D1R5G5B5:
                case CELL_GCM_TEXTURE_R5G5B5A1:
                case CELL_GCM_TEXTURE_COMPRESSED_HILO8:
                case CELL_GCM_TEXTURE_COMPRESSED_HILO_S8:
                case CELL_GCM_TEXTURE_COMPRESSED_B8R8_G8R8_RAW:
                case CELL_GCM_TEXTURE_COMPRESSED_R8B8_R8G8_RAW:
                    depth = 2;
                    break;
                case CELL_GCM_TEXTURE_A8R8G8B8:
                case CELL_GCM_TEXTURE_DEPTH24_D8:
                case CELL_GCM_TEXTURE_DEPTH24_D8_FLOAT:
                case CELL_GCM_TEXTURE_Y16_X16:
                case CELL_GCM_TEXTURE_X32_FLOAT:
                case CELL_GCM_TEXTURE_D8R8G8B8:
                case CELL_GCM_TEXTURE_Y16_X16_FLOAT:
                    depth = 4;
                    break;
                case CELL_GCM_TEXTURE_W16_Z16_Y16_X16_FLOAT:
                    depth = 8;
                    break;
                case CELL_GCM_TEXTURE_W32_Z32_Y32_X32_FLOAT:
                    depth = 16;
                    break;
                case CELL_GCM_TEXTURE_COMPRESSED_DXT1:
                case CELL_GCM_TEXTURE_COMPRESSED_DXT23:
                case CELL_GCM_TEXTURE_COMPRESSED_DXT45:
                    depth = 0;
                    break;
                default:
                    depth = 4;
                    break;
            }

            int pitch = width * depth;
            if (raw_format == CELL_GCM_TEXTURE_COMPRESSED_DXT1)
            {
                pitch = ((width + 3) / 4) * 8;
            }
            else if (raw_format == CELL_GCM_TEXTURE_COMPRESSED_DXT23 || raw_format == CELL_GCM_TEXTURE_COMPRESSED_DXT45)
            {
                pitch = ((width + 3) / 4) * 16;
            }
            else if (raw_format == CELL_GCM_TEXTURE_COMPRESSED_B8R8_G8R8_RAW ||
                raw_format == CELL_GCM_TEXTURE_COMPRESSED_R8B8_R8G8_RAW)
            {
                pitch = ((width + 1) / 2) * 4;
            }

            return pitch;
        }

        public static ushort gtfGetDepth(byte format)
        {
            ushort depth = 0;
            byte raw_format = gtfGetRawFormat(format);
            switch (raw_format)
            {
                case CELL_GCM_TEXTURE_B8:
                    depth = 1;
                    break;
                case CELL_GCM_TEXTURE_A1R5G5B5:
                case CELL_GCM_TEXTURE_A4R4G4B4:
                case CELL_GCM_TEXTURE_R5G6B5:
                case CELL_GCM_TEXTURE_G8B8:
                case CELL_GCM_TEXTURE_R6G5B5:
                case CELL_GCM_TEXTURE_DEPTH16:
                case CELL_GCM_TEXTURE_DEPTH16_FLOAT:
                case CELL_GCM_TEXTURE_X16:
                case CELL_GCM_TEXTURE_D1R5G5B5:
                case CELL_GCM_TEXTURE_R5G5B5A1:
                case CELL_GCM_TEXTURE_COMPRESSED_HILO8:
                case CELL_GCM_TEXTURE_COMPRESSED_HILO_S8:
                case CELL_GCM_TEXTURE_COMPRESSED_B8R8_G8R8_RAW:
                case CELL_GCM_TEXTURE_COMPRESSED_R8B8_R8G8_RAW:
                    depth = 2;
                    break;
                case CELL_GCM_TEXTURE_A8R8G8B8:
                case CELL_GCM_TEXTURE_DEPTH24_D8:
                case CELL_GCM_TEXTURE_DEPTH24_D8_FLOAT:
                case CELL_GCM_TEXTURE_Y16_X16:
                case CELL_GCM_TEXTURE_X32_FLOAT:
                case CELL_GCM_TEXTURE_D8R8G8B8:
                case CELL_GCM_TEXTURE_Y16_X16_FLOAT:
                    depth = 4;
                    break;
                case CELL_GCM_TEXTURE_W16_Z16_Y16_X16_FLOAT:
                    depth = 8;
                    break;
                case CELL_GCM_TEXTURE_W32_Z32_Y32_X32_FLOAT:
                    depth = 16;
                    break;
                case CELL_GCM_TEXTURE_COMPRESSED_DXT1:
                    depth = 8;
                    break;
                case CELL_GCM_TEXTURE_COMPRESSED_DXT23:
                case CELL_GCM_TEXTURE_COMPRESSED_DXT45:
                    depth = 16;
                    break;
                default:
                    depth = 4;
                    break;
            }

            return depth;
        }

        private class sFormatString
        {
            public byte format;
            public string name;
            public sFormatString(byte format, string name)
            {
                this.format = format;
                this.name = name;
            }
        }
        public static string gtfGetFormatName(byte format)
        {


            List<sFormatString> sfs = new List<sFormatString>(){
                new sFormatString( 0xFF, "Unknown Format" ),
                new sFormatString( 0x81, "CELL_GCM_TEXTURE_B8" ),
                new sFormatString( 0x82, "CELL_GCM_TEXTURE_A1R5G5B5" ),
                new sFormatString( 0x83, "CELL_GCM_TEXTURE_A4R4G4B4" ),
                new sFormatString( 0x84, "CELL_GCM_TEXTURE_R5G6B5" ),
                new sFormatString( 0x85, "CELL_GCM_TEXTURE_A8R8G8B8" ),
                new sFormatString( 0x86, "CELL_GCM_TEXTURE_COMPRESSED_DXT1" ),
                new sFormatString( 0x87, "CELL_GCM_TEXTURE_COMPRESSED_DXT23" ),
                new sFormatString( 0x88, "CELL_GCM_TEXTURE_COMPRESSED_DXT45" ),
                new sFormatString( 0x8B, "CELL_GCM_TEXTURE_G8B8" ),
                new sFormatString( 0x8D, "CELL_GCM_TEXTURE_COMPRESSED_B8R8_G8R8"),
                new sFormatString( 0x8E, "CELL_GCM_TEXTURE_COMPRESSED_R8B8_R8G8"),
                new sFormatString( 0x8F, "CELL_GCM_TEXTURE_R6G5B5" ),
                new sFormatString( 0x90, "CELL_GCM_TEXTURE_DEPTH24_D8" ),
                new sFormatString( 0x91, "CELL_GCM_TEXTURE_DEPTH24_D8_FLOAT"),
                new sFormatString( 0x92, "CELL_GCM_TEXTURE_DEPTH16" ),
                new sFormatString( 0x93, "CELL_GCM_TEXTURE_DEPTH16_FLOAT" ),
                new sFormatString( 0x94, "CELL_GCM_TEXTURE_X16" ),
                new sFormatString( 0x95, "CELL_GCM_TEXTURE_Y16_X16" ),
                new sFormatString( 0x97, "CELL_GCM_TEXTURE_R5G5B5A1" ),
                new sFormatString( 0x98, "CELL_GCM_TEXTURE_COMPRESSED_HILO8" ),
                new sFormatString( 0x99, "CELL_GCM_TEXTURE_COMPRESSED_HILO_S8" ),
                new sFormatString( 0x9A, "CELL_GCM_TEXTURE_W16_Z16_Y16_X16_FLOAT" ),
                new sFormatString( 0x9B, "CELL_GCM_TEXTURE_W32_Z32_Y32_X32_FLOAT" ),
                new sFormatString( 0x9C, "CELL_GCM_TEXTURE_X32_FLOAT" ),
                new sFormatString( 0x9D, "CELL_GCM_TEXTURE_D1R5G5B5" ),
                new sFormatString( 0x9E, "CELL_GCM_TEXTURE_D8R8G8B8" ),
                new sFormatString( 0x9F, "CELL_GCM_TEXTURE_Y16_X16_FLOAT" )
            };

            int FORMAT_NUM = sfs.Count;
            byte rawfor = gtfGetRawFormat(format);
            for (int i = 0; i < FORMAT_NUM; ++i)
            {
                if (rawfor == sfs[i].format)
                {
                    return sfs[i].name;
                }
            }

            return sfs[0].name;
        }

        public static void gtfCheckSpec(CellGcmTexture tex)
        {
            Console.Write("  [GTF Spec]\n");

            byte raw_format = gtfGetRawFormat(tex.format);
            bool is_dxt = (
        raw_format == CELL_GCM_TEXTURE_COMPRESSED_DXT1 ||
        raw_format == CELL_GCM_TEXTURE_COMPRESSED_DXT23 ||
        raw_format == CELL_GCM_TEXTURE_COMPRESSED_DXT45);

            if (tex.dimension == CELL_GCM_TEXTURE_DIMENSION_3)
            {
                Console.WriteLine($"    Size: {tex.width}x{tex.height}x{tex.depth}");
            }
            else
            {
                Console.WriteLine($"    Size: {tex.width}x{tex.height}");
            }
            string normalized = "false";
            if (Convert.ToBoolean(tex.format & CELL_GCM_TEXTURE_UN)) {
                normalized = "true";
            }
            Console.WriteLine($"    Info: mipmap={tex.mipmap}, cube={tex.cubemap}, pitch={tex.pitch}, normalized={normalized}");
            Console.WriteLine($"    Format: {gtfGetFormatName(tex.format)}" );

            Console.Write("    Layout: ");
            if (is_dxt)
            {
                if (Convert.ToBoolean(tex.format &  CELL_GCM_TEXTURE_LN))
                {
                    Console.Write("Not Power of 2\n");
                }
                else
                {
                    Console.Write("Power of 2\n");
                }
            }
            else
            {
                if (Convert.ToBoolean(tex.format & CELL_GCM_TEXTURE_LN))
                {
                    Console.Write("Linear\n");
                }
                else
                {
                    Console.Write("Swizzle\n");
                }
            }

            Console.WriteLine($"    Remap: {utilRemap2Str(tex.remap)} ({tex.remap.ToString("X4")})");
        }

        public static string utilRemap2Str(int remap)
        {
            char[] strRemap = { 'A', 'R', 'G', 'B' };

            for (int i = 0; i < 4; ++i)
            {
                int src = (remap >> (i * 2)) & 0x3;

                char c = 'N';

                if (src == CELL_GCM_TEXTURE_REMAP_FROM_A)
                {
                    c = 'A';
                }
                else if (src == CELL_GCM_TEXTURE_REMAP_FROM_R)
                {
                    c = 'R';
                }
                else if (src == CELL_GCM_TEXTURE_REMAP_FROM_G)
                {
                    c = 'G';
                }
                else if (src == CELL_GCM_TEXTURE_REMAP_FROM_B)
                {
                    c = 'B';
                }

                int opp = (remap >> (i * 2 + 8)) & 0x3; // default: CELL_GCM_TEXTURE_REMAP_REMAP
                if (opp == CELL_GCM_TEXTURE_REMAP_ZERO)
                {
                    c = '0';
                }
                else if (opp == CELL_GCM_TEXTURE_REMAP_ONE)
                {
                    c = '1';
                }

                strRemap[i] = c;
            }

            return Encoding.ASCII.GetString(strRemap.Select(c => (byte)c).ToArray());

        }

        public static byte gtfGetRawFormat(byte texture_format)
        {
            return (byte)(texture_format & ~(CELL_GCM_TEXTURE_LN | CELL_GCM_TEXTURE_UN));
        }

        public static bool gtfIsDxtn(byte format)
        {
            byte raw_format = gtfGetRawFormat(format);
            return (raw_format == CELL_GCM_TEXTURE_COMPRESSED_DXT1 ||
                    raw_format == CELL_GCM_TEXTURE_COMPRESSED_DXT23 ||
                    raw_format == CELL_GCM_TEXTURE_COMPRESSED_DXT45);
        }


    }
}
