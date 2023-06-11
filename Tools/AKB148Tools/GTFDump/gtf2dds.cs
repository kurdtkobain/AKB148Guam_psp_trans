using System;
using static GTFDump.ddsinfo;
using static GTFDump.gtfinfo;

namespace GTFDump
{
    internal class gtf2dds
    {
        public static bool gtf2ddsConvHeader(ref CellUtilDDSHeader ddsh, CellGcmTexture texture)
        {

            byte raw_format = gtfutil.gtfGetRawFormat(texture.format);
            bool is_dxt = gtfutil.gtfIsDxtn(raw_format);


            switch (raw_format)
            {
                case CELL_GCM_TEXTURE_B8:
                    ddsh.ddspf.flags = DDSF_LUMINANCE;
                    ddsh.ddspf.rgbBitCount = 8;
                    ddsh.ddspf.rbitMask = 0x000000ff;
                    break;
                case CELL_GCM_TEXTURE_A1R5G5B5:
                    ddsh.ddspf.flags = DDPF_RGB | DDPF_ALPHAPIXELS;
                    ddsh.ddspf.rgbBitCount = 16;
                    ddsh.ddspf.abitMask = 0x00008000;
                    ddsh.ddspf.rbitMask = 0x00007c00;
                    ddsh.ddspf.gbitMask = 0x000003e0;
                    ddsh.ddspf.bbitMask = 0x0000001f;
                    break;
                case CELL_GCM_TEXTURE_A4R4G4B4:
                    ddsh.ddspf.flags = DDPF_RGB | DDPF_ALPHAPIXELS;
                    ddsh.ddspf.rgbBitCount = 16;
                    ddsh.ddspf.abitMask = 0x0000f000;
                    ddsh.ddspf.rbitMask = 0x00000f00;
                    ddsh.ddspf.gbitMask = 0x000000f0;
                    ddsh.ddspf.bbitMask = 0x0000000f;
                    break;
                case CELL_GCM_TEXTURE_R5G6B5:
                    ddsh.ddspf.flags = DDPF_RGB;
                    ddsh.ddspf.rgbBitCount = 16;
                    ddsh.ddspf.abitMask = 0x00000000;
                    ddsh.ddspf.rbitMask = 0x0000f800;
                    ddsh.ddspf.gbitMask = 0x000007e0;
                    ddsh.ddspf.bbitMask = 0x0000001f;
                    break;
                case CELL_GCM_TEXTURE_R6G5B5:
                    ddsh.ddspf.flags = DDPF_RGB;
                    ddsh.ddspf.rgbBitCount = 16;
                    ddsh.ddspf.abitMask = 0x00000000;
                    ddsh.ddspf.rbitMask = 0x0000fc00;
                    ddsh.ddspf.gbitMask = 0x000003e0;
                    ddsh.ddspf.bbitMask = 0x0000001f;
                    break;
                case CELL_GCM_TEXTURE_R5G5B5A1:
                    ddsh.ddspf.flags = DDPF_RGB;
                    ddsh.ddspf.rgbBitCount = 16;
                    ddsh.ddspf.abitMask = 0x00000001;
                    ddsh.ddspf.rbitMask = 0x0000f800;
                    ddsh.ddspf.gbitMask = 0x000007c0;
                    ddsh.ddspf.bbitMask = 0x0000003e;
                    break;
                case CELL_GCM_TEXTURE_D1R5G5B5:
                    ddsh.ddspf.flags = DDPF_RGB;
                    ddsh.ddspf.rgbBitCount = 16;
                    ddsh.ddspf.abitMask = 0x00008000;
                    ddsh.ddspf.rbitMask = 0x00007c00;
                    ddsh.ddspf.gbitMask = 0x000003e0;
                    ddsh.ddspf.bbitMask = 0x0000001f;
                    break;
                case CELL_GCM_TEXTURE_A8R8G8B8:
                    ddsh.ddspf.flags = DDPF_RGB | DDPF_ALPHAPIXELS;
                    ddsh.ddspf.rgbBitCount = 32;
                    ddsh.ddspf.abitMask = 0xff000000;
                    ddsh.ddspf.rbitMask = 0x00ff0000;
                    ddsh.ddspf.gbitMask = 0x0000ff00;
                    ddsh.ddspf.bbitMask = 0x000000ff;

                    break;
                case CELL_GCM_TEXTURE_D8R8G8B8:
                    ddsh.ddspf.flags = DDPF_RGB;
                    ddsh.ddspf.rgbBitCount = 32;
                    ddsh.ddspf.abitMask = 0xff000000;
                    ddsh.ddspf.rbitMask = 0x00ff0000;
                    ddsh.ddspf.gbitMask = 0x0000ff00;
                    ddsh.ddspf.bbitMask = 0x000000ff;
                    break;
                case CELL_GCM_TEXTURE_G8B8:
                    // A8L8
                    ddsh.ddspf.flags = DDSF_LUMINANCE | DDPF_ALPHAPIXELS;
                    ddsh.ddspf.rgbBitCount = 16;
                    ddsh.ddspf.abitMask = 0x0000ff00;
                    ddsh.ddspf.rbitMask = 0x000000ff;
                    break;
                case CELL_GCM_TEXTURE_X16:
                    // L16
                    ddsh.ddspf.flags = DDSF_LUMINANCE;
                    ddsh.ddspf.rgbBitCount = 16;
                    ddsh.ddspf.rbitMask = 0x0000ffff;
                    break;
                case CELL_GCM_TEXTURE_Y16_X16:
                    // G16R16
                    ddsh.ddspf.flags = DDPF_RGB;
                    ddsh.ddspf.rgbBitCount = 32;
                    ddsh.ddspf.abitMask = 0x00000000;
                    ddsh.ddspf.rbitMask = 0x0000ffff;
                    ddsh.ddspf.gbitMask = 0xffff0000;
                    ddsh.ddspf.bbitMask = 0x00000000;
                    break;
                case CELL_GCM_TEXTURE_COMPRESSED_DXT1:
                    ddsh.ddspf.flags = DDPF_FOURCC;
                    ddsh.ddspf.fourCC = FOURCC_DXT1;
                    break;
                case CELL_GCM_TEXTURE_COMPRESSED_DXT23:
                    ddsh.ddspf.flags = DDPF_FOURCC;
                    ddsh.ddspf.fourCC = FOURCC_DXT3;
                    break;
                case CELL_GCM_TEXTURE_COMPRESSED_DXT45:
                    ddsh.ddspf.flags = DDPF_FOURCC;
                    ddsh.ddspf.fourCC = FOURCC_DXT5;
                    break;
                case CELL_GCM_TEXTURE_W16_Z16_Y16_X16_FLOAT:
                    ddsh.ddspf.flags = DDPF_FOURCC;
                    ddsh.ddspf.fourCC = FOURCC_A16B16G16R16F;
                    break;
                case CELL_GCM_TEXTURE_W32_Z32_Y32_X32_FLOAT:
                    ddsh.ddspf.flags = DDPF_FOURCC;
                    ddsh.ddspf.fourCC = FOURCC_A32B32G32R32F;
                    break;
                case CELL_GCM_TEXTURE_X32_FLOAT:
                    ddsh.ddspf.flags = DDPF_FOURCC;
                    ddsh.ddspf.fourCC = FOURCC_R32F;
                    break;
                case CELL_GCM_TEXTURE_Y16_X16_FLOAT:
                    ddsh.ddspf.flags = DDPF_FOURCC;
                    ddsh.ddspf.fourCC = FOURCC_G16R16F;
                    break;
                case CELL_GCM_TEXTURE_COMPRESSED_B8R8_G8R8_RAW:
                    ddsh.ddspf.flags = DDPF_FOURCC;
                    ddsh.ddspf.fourCC = FOURCC_R8G8_B8G8;
                    break;
                case CELL_GCM_TEXTURE_COMPRESSED_R8B8_R8G8_RAW:
                    ddsh.ddspf.flags = DDPF_FOURCC;
                    ddsh.ddspf.fourCC = FOURCC_G8R8_G8B8;
                    break;

                // unsupported
                case CELL_GCM_TEXTURE_COMPRESSED_HILO8:
                case CELL_GCM_TEXTURE_COMPRESSED_HILO_S8:
                default:
                    return false;
            }

            ddsh.magic = FOURCC_DDS;
            ddsh.size = 124;
            ddsh.flags |= DDSD_CAPS;
            ddsh.flags |= DDSD_PIXELFORMAT;
            ddsh.flags |= DDSD_WIDTH;
            ddsh.flags |= DDSD_HEIGHT;
            ddsh.caps1 |= DDSCAPS_TEXTURE;

            if (texture.mipmap != 1)
            {
                ddsh.flags |= DDSD_MIPMAPCOUNT;
                ddsh.caps1 |= DDSCAPS_MIPMAP;
                ddsh.caps1 |= DDSCAPS_COMPLEX;
                ddsh.mipMapCount = texture.mipmap;
            }

            if (texture.dimension == CELL_GCM_TEXTURE_DIMENSION_3)
            {
                ddsh.flags |= DDSD_DEPTH;
                ddsh.caps1 |= DDSCAPS2_VOLUME;
                ddsh.caps1 |= DDSCAPS_COMPLEX;
                ddsh.depth = texture.depth;
            }

            if (texture.cubemap == CELL_GCM_TRUE)
            {
                ddsh.caps1 |= DDSCAPS2_CUBEMAP;
                ddsh.caps1 |= DDSCAPS_COMPLEX;

                ddsh.caps2 |= DDSCAPS2_CUBEMAP_POSITIVEX;
                ddsh.caps2 |= DDSCAPS2_CUBEMAP_NEGATIVEX;
                ddsh.caps2 |= DDSCAPS2_CUBEMAP_POSITIVEY;
                ddsh.caps2 |= DDSCAPS2_CUBEMAP_NEGATIVEY;
                ddsh.caps2 |= DDSCAPS2_CUBEMAP_POSITIVEZ;
                ddsh.caps2 |= DDSCAPS2_CUBEMAP_NEGATIVEZ;
            }

            if (Convert.ToBoolean(ddsh.ddspf.flags & DDPF_ALPHAPIXELS))
            {
                ddsh.caps1 |= DDSCAPS_ALPHA;
            }

            ddsh.ddspf.size = 32;


            ddsh.width = texture.width;
            ddsh.height = texture.height;

            bool bLinearSize = false;
            bool bNoPitchOrLinearSize = false;
            if (raw_format == CELL_GCM_TEXTURE_COMPRESSED_B8R8_G8R8_RAW ||
                raw_format == CELL_GCM_TEXTURE_COMPRESSED_R8B8_R8G8_RAW)
            {
                bNoPitchOrLinearSize = true;
            }

            if (bNoPitchOrLinearSize)
            {
                ddsh.pitchOrLinearSize = 0;
            }
            else if (is_dxt)
            {
                ushort color_depth = gtfutil.gtfGetDepth(raw_format);
                ddsh.pitchOrLinearSize = (uint)(((texture.width + 3) / 4) * ((texture.height + 3) / 4) * color_depth);

                ddsh.flags |= DDSD_LINEARSIZE;
            }
            else if (bLinearSize)
            {
                uint pitch = gtfutil.gtfGetPitch(raw_format, texture.width);
                ddsh.pitchOrLinearSize = texture.height * pitch;

                ddsh.flags |= DDSD_LINEARSIZE;
            }
            else if (texture.pitch != 0)
            {
                ddsh.pitchOrLinearSize = texture.pitch;

                ddsh.flags |= DDSD_PITCH;
            }

            return true;
        }
    }
}
