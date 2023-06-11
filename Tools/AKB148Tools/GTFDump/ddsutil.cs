using System;
using static GTFDump.ddsinfo;

namespace GTFDump
{
    internal class ddsutil
    {
        public static bool ddsCheckSpec(CellUtilDDSHeader ddsh)
        {
            if (ddsh.magic != FOURCC_DDS) return false;

            Console.Write("  [DDS Spec]\n");
            if (Convert.ToBoolean(ddsh.caps2 & DDSCAPS2_VOLUME))
            {
                Console.Write(String.Format("    Size: {0:d}x{1:d}x{2:d}\n", ddsh.width, ddsh.height, ddsh.depth));
            }
            else
            {
                Console.Write(String.Format("    Size: {0:d}x{1:d}\n", ddsh.width, ddsh.height));
            }
            {
                uint mipmap = 0;
                if (Convert.ToBoolean(ddsh.flags & DDSD_MIPMAPCOUNT))
                {
                    mipmap = ddsh.mipMapCount;
                }

                uint cube = 0;
                if (Convert.ToBoolean(ddsh.caps2 & DDSCAPS2_CUBEMAP))
                {
                    if (Convert.ToBoolean(ddsh.caps2 & DDSCAPS2_CUBEMAP_POSITIVEX)) ++cube;
                    if (Convert.ToBoolean(ddsh.caps2 & DDSCAPS2_CUBEMAP_NEGATIVEX) ) ++cube;
                    if (Convert.ToBoolean(ddsh.caps2 & DDSCAPS2_CUBEMAP_POSITIVEY)) ++cube;
                    if (Convert.ToBoolean(ddsh.caps2 & DDSCAPS2_CUBEMAP_NEGATIVEY)) ++cube;
                    if (Convert.ToBoolean(ddsh.caps2 & DDSCAPS2_CUBEMAP_POSITIVEZ)) ++cube;
                    if (Convert.ToBoolean(ddsh.caps2 & DDSCAPS2_CUBEMAP_NEGATIVEZ)) ++cube;
                }

                if (Convert.ToBoolean(ddsh.flags & DDSD_LINEARSIZE))
                {
                    Console.Write(String.Format("    Info: mipmap={0:d}, cube={1:d}, linear_size={2:d}\n", mipmap, cube, ddsh.pitchOrLinearSize));
                }
                else
                {
                    Console.Write(String.Format("    Info: mipmap={0:d}, cube={1:d}, pitch={2:d}\n", mipmap, cube, ddsh.pitchOrLinearSize));
                }
            }

            Console.Write("    Flags: ");
            if (Convert.ToBoolean(ddsh.flags & DDSD_CAPS)) Console.Write("DDSD_CAPS, ");
            if (Convert.ToBoolean(ddsh.flags & DDSD_PIXELFORMAT)) Console.Write("DDSD_PIXELFORMAT, ");
            if (Convert.ToBoolean(ddsh.flags & DDSD_WIDTH)) Console.Write("DDSD_WIDTH, ");
            if (Convert.ToBoolean(ddsh.flags & DDSD_HEIGHT)) Console.Write("DDSD_HEIGHT, ");
            if (Convert.ToBoolean(ddsh.flags & DDSD_PITCH)) Console.Write("DDSD_PITCH, ");
            if (Convert.ToBoolean(ddsh.flags & DDSD_MIPMAPCOUNT)) Console.Write("DDSD_MIPMAPCOUNT, ");
            if (Convert.ToBoolean(ddsh.flags & DDSD_LINEARSIZE)) Console.Write("DDSD_LINEARSIZE, ");
            if (Convert.ToBoolean(ddsh.flags & DDSD_DEPTH)) Console.Write("DDSD_DEPTH, ");

            Console.Write("\n    Caps1: ");
            if (Convert.ToBoolean(ddsh.caps1 & DDSCAPS_ALPHA)) Console.Write("DDSCAPS_ALPHA, ");
            if (Convert.ToBoolean(ddsh.caps1 & DDSCAPS_COMPLEX)) Console.Write("DDSCAPS_COMPLEX, ");
            if (Convert.ToBoolean(ddsh.caps1 & DDSCAPS_TEXTURE)) Console.Write("DDSCAPS_TEXTURE, ");
            if (Convert.ToBoolean(ddsh.caps1 & DDSCAPS_MIPMAP)) Console.Write("DDSCAPS_MIPMAP, ");

            Console.Write("\n    Caps2: ");
            if (Convert.ToBoolean(ddsh.caps2 & DDSCAPS2_VOLUME)) Console.Write("DDSCAPS2_VOLUME, ");
            if (Convert.ToBoolean(ddsh.caps2 & DDSCAPS2_CUBEMAP)) Console.Write("DDSCAPS2_CUBEMAP, ");

            Console.Write("\n    PixelFlag: ");
            if (Convert.ToBoolean(ddsh.ddspf.flags & DDPF_ALPHAPIXELS)) Console.Write("DDPF_ALPHAPIXELS, ");
            if (Convert.ToBoolean(ddsh.ddspf.flags & DDPF_ALPHA)) Console.Write("DDPF_ALPHA, ");
            if (Convert.ToBoolean(ddsh.ddspf.flags & DDPF_FOURCC)) Console.Write("DDPF_FOURCC, ");
            if (Convert.ToBoolean(ddsh.ddspf.flags & DDPF_RGB)) Console.Write("DDPF_RGB, ");
            if (Convert.ToBoolean(ddsh.ddspf.flags & DDPF_PALETTEINDEXED1)) Console.Write("DDPF_PALETTEINDEXED1, ");
            if (Convert.ToBoolean(ddsh.ddspf.flags & DDPF_PALETTEINDEXED2)) Console.Write("DDPF_PALETTEINDEXED2, ");
            if (Convert.ToBoolean(ddsh.ddspf.flags & DDPF_PALETTEINDEXED4)) Console.Write("DDPF_PALETTEINDEXED4, ");
            if (Convert.ToBoolean(ddsh.ddspf.flags & DDPF_PALETTEINDEXED8)) Console.Write("DDPF_PALETTEINDEXED8, ");
            if (Convert.ToBoolean(ddsh.ddspf.flags & DDPF_LUMINANCE)) Console.Write("DDPF_LUMINANCE, ");
            if (Convert.ToBoolean(ddsh.ddspf.flags & DDPF_ALPHAPREMULT)) Console.Write("DDPF_ALPHAPREMULT, ");
            if (Convert.ToBoolean(ddsh.ddspf.flags & DDPF_NORMAL)) Console.Write("DDPF_NORMAL(NV Custom flag), ");

            if (Convert.ToBoolean(ddsh.ddspf.flags & DDPF_FOURCC))
            {
                Console.Write("\n    FOURCC: ");
                if (ddsh.ddspf.fourCC == FOURCC_DDS) Console.Write("FOURCC_DDS, ");
                if (ddsh.ddspf.fourCC == FOURCC_DXT1) Console.Write("FOURCC_DXT1, ");
                if (ddsh.ddspf.fourCC == FOURCC_DXT2) Console.Write("FOURCC_DXT2, ");
                if (ddsh.ddspf.fourCC == FOURCC_DXT3) Console.Write("FOURCC_DXT3, ");
                if (ddsh.ddspf.fourCC == FOURCC_DXT4) Console.Write("FOURCC_DXT4, ");
                if (ddsh.ddspf.fourCC == FOURCC_DXT5) Console.Write("FOURCC_DXT5, ");
                if (ddsh.ddspf.fourCC == FOURCC_RXGB) Console.Write("FOURCC_RXGB, ");
                if (ddsh.ddspf.fourCC == FOURCC_ATI1) Console.Write("FOURCC_ATI1, ");
                if (ddsh.ddspf.fourCC == FOURCC_ATI2) Console.Write("FOURCC_ATI2, ");

                if (ddsh.ddspf.fourCC == FOURCC_R16F) Console.Write("FOURCC_R16F, ");
                if (ddsh.ddspf.fourCC == FOURCC_G16R16F) Console.Write("FOURCC_G16R16F, ");
                if (ddsh.ddspf.fourCC == FOURCC_A16B16G16R16F) Console.Write("FOURCC_A16B16G16R16F, ");
                if (ddsh.ddspf.fourCC == FOURCC_R32F) Console.Write("FOURCC_R32F, ");
                if (ddsh.ddspf.fourCC == FOURCC_A32B32G32R32F) Console.Write("FOURCC_A32B32G32R32F, ");

                if (ddsh.ddspf.fourCC == FOURCC_YVYU) Console.Write("FOURCC_YVYU, ");
                if (ddsh.ddspf.fourCC == FOURCC_YUY2) Console.Write("FOURCC_YUY2, ");
                if (ddsh.ddspf.fourCC == FOURCC_R8G8_B8G8) Console.Write("FOURCC_R8G8_B8G8, ");
                if (ddsh.ddspf.fourCC == FOURCC_G8R8_G8B8) Console.Write("FOURCC_G8R8_G8B8, ");
            }

            Console.Write(String.Format("\n    BitCount: {0:d}\n", ddsh.ddspf.rgbBitCount));
            Console.Write(String.Format("    Mask: A[0x{0:X8}] R[0x{1:X8}] G[0x{2:X8}] B[0x{3:X8}] \n", ddsh.ddspf.abitMask, ddsh.ddspf.rbitMask, ddsh.ddspf.gbitMask, ddsh.ddspf.bbitMask));

            return true;
        }
    }
}
