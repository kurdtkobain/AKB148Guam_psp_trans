using System;
using static GTFDump.ddsinfo;
using static GTFDump.gtfinfo;
using static GTFDump.utils;

namespace GTFDump
{
    internal class convert
    {

        static void convertMoveMemory(ref byte[] gtf, int gtfoff, ref byte[] dds, int ddsoff, int size, bool bDds2Gtf)
        {

            if (bDds2Gtf)
            {
                Buffer.BlockCopy(gtf, gtfoff, dds, ddsoff, size);
            }
            else
            {
                Buffer.BlockCopy(dds, ddsoff, gtf, gtfoff, size);
            }
        }

        static void convertMoveMemoryWithInvertEndian16(ref byte[] gtf, int gtfoff, ref byte[] dds, int ddsoff, int size, bool bDds2Gtf)
        {
            if (bDds2Gtf)
            {
                memmove_with_invert_endian16(ref gtf, gtfoff, ref dds, ddsoff, size);
            }
            else
            {
                memmove_with_invert_endian16(ref dds, ddsoff, ref gtf, gtfoff, size);
            }
        }

        static void convertMoveMemoryWithInvertEndian32(ref byte[] gtf, int gtfoff, ref byte[] dds, int ddsoff, int size, bool bDds2Gtf)
        {
            if (bDds2Gtf)
            {
                memmove_with_invert_endian32(ref gtf,  gtfoff, ref dds, ddsoff, size);
            }
            else
            {
                memmove_with_invert_endian32(ref dds, ddsoff, ref gtf,  gtfoff, size);
            }
        }

        const int CELL_GTFCONV_ENDIAN_NOINVERT = 0;
        const int CELL_GTFCONV_ENDIAN_INVERT32 = 1;
        const int CELL_GTFCONV_ENDIAN_INVERT16 = 2;
        const int CELL_GTFCONV_ENDIAN_INVERT32_EVEN = 3;

        static int getInvertFlag(CellGcmTexture tex)
        {
            int invert_flag = CELL_GTFCONV_ENDIAN_INVERT32;

            byte raw_format = gtfutil.gtfGetRawFormat(tex.format);
            bool is_dxt = gtfutil.gtfIsDxtn(raw_format);
            ushort depth = gtfutil.gtfGetDepth(raw_format);

            if (is_dxt)
            {
                invert_flag = CELL_GTFCONV_ENDIAN_NOINVERT;
            }

            if (depth == 2)
            {
                invert_flag = CELL_GTFCONV_ENDIAN_INVERT16;
            }

            if (depth == 4)
            {
                invert_flag = CELL_GTFCONV_ENDIAN_INVERT32;
            }

            if (raw_format == CELL_GCM_TEXTURE_X16 ||
                raw_format == CELL_GCM_TEXTURE_Y16_X16 ||
                raw_format == CELL_GCM_TEXTURE_Y16_X16_FLOAT ||
                raw_format == CELL_GCM_TEXTURE_W16_Z16_Y16_X16_FLOAT)
            {
                invert_flag = CELL_GTFCONV_ENDIAN_INVERT16;
            }

            if (raw_format == CELL_GCM_TEXTURE_W32_Z32_Y32_X32_FLOAT ||
                raw_format == CELL_GCM_TEXTURE_X32_FLOAT)
            {
                invert_flag = CELL_GTFCONV_ENDIAN_INVERT32;
            }

            if (raw_format == CELL_GCM_TEXTURE_COMPRESSED_B8R8_G8R8_RAW ||
                raw_format == CELL_GCM_TEXTURE_COMPRESSED_R8B8_R8G8_RAW)
            {
                invert_flag = CELL_GTFCONV_ENDIAN_INVERT32_EVEN;
            }

            return invert_flag;
        }

        public static bool convertLayOutBuffer(ref layout_t[] playout, CellGcmTexture tex, CellUtilDDSHeader ddsh)
        {
            Console.Write("  [Layout]\n");

            byte raw_format = gtfutil.gtfGetRawFormat(tex.format);
            bool is_dxt = gtfutil.gtfIsDxtn(raw_format);
            bool is_comp = (raw_format == CELL_GCM_TEXTURE_COMPRESSED_B8R8_G8R8_RAW ||
                        raw_format == CELL_GCM_TEXTURE_COMPRESSED_R8B8_R8G8_RAW);

            ushort width = tex.width;
            ushort height = tex.height;
            ushort depth = tex.depth;
            int gtf_pitch = tex.pitch;
            byte cube = 1;
            if (tex.cubemap == CELL_GCM_TRUE)
            {
                cube = 6;
            }

            bool is_cube = (cube == 6);
            bool is_volume = (depth > 1);
            bool is_mip = (tex.mipmap > 1);

            if (gtf_pitch == 0)
            {
                gtf_pitch = gtfutil.gtfGetPitch(raw_format, width);
            }
            ushort color_depth = gtfutil.gtfGetDepth(raw_format);

            // convert to different color_depth
            ushort dds_color_depth = 0;
            if (ddsh.ddspf.rgbBitCount == 24)
            {
                dds_color_depth = 3;
            }
            if (ddsh.ddspf.fourCC == FOURCC_R16F)
            {
                dds_color_depth = 2;
            }

            int layoff = 0;

            layout_t latest = new layout_t();
            // cubemap
            for (int n = 0; n < cube; ++n)
            {
                bool new_face = true;
                ushort w = width;
                ushort h = height;
                ushort v = depth;
                ushort m = 0;

                // mipmap
                while (m < tex.mipmap)
                {
                    layout_t layout = new layout_t();

                    layout.width = w;
                    layout.height = h;
                    layout.depth = v;
                    layout.pitch = gtf_pitch;
                    layout.color_depth = color_depth;
                    layout.dds_depth = dds_color_depth;
                    layout.dds_expand = (dds_color_depth != 0);

                    if (is_dxt)
                    {
                        // dxtn
                        layout.dds_size = (w + 3) / 4 * ((h + 3) / 4) * color_depth;
                    }
                    else if (is_comp)
                    {
                        // B8R8_G8R8, R8B8_R8G8
                        layout.dds_size = (w + 1) / 2 * h * 4;
                    }
                    else
                    {
                        layout.dds_size = w * h * color_depth;
                    }

                    // swizzle gtf size
                    layout.gtf_swizzle_size = layout.dds_size;

                    if (dds_color_depth != 0)
                    {
                        layout.dds_pitch = w * dds_color_depth;
                        layout.dds_size = layout.dds_pitch * h;
                    }

                    // linear gtf size
                    if (is_dxt)
                    {
                        // not power of 2 dxtn
                        layout.gtf_linear_size = ((h + 3) / 4) * gtf_pitch;
                    }
                    else
                    {
                        layout.gtf_linear_size = h * gtf_pitch;
                    }

                    // volume
                    if (is_volume)
                    {
                        layout.dds_size *= v;
                        layout.gtf_swizzle_size *= v;
                        layout.gtf_linear_size *= v;
                    }

                    // offset
                    layout.dds_offset = latest.dds_offset + latest.dds_size;
                    layout.gtf_swizzle_offset = latest.gtf_swizzle_offset + latest.gtf_swizzle_size;
                    if (is_cube && new_face)
                    {
                        // when swizzle cubemap, each face must be aligned on a 128-byte boundary
                        layout.gtf_swizzle_offset = utilGetAlign(layout.gtf_swizzle_offset, 128);
                        new_face = false;
                    }
                    layout.gtf_linear_offset = latest.gtf_linear_offset + latest.gtf_linear_size;

                    playout[layoff] = layout;
                    ++layoff;
                    latest = layout;


                    Console.Write("    ");
                    if (is_cube)
                    {
                        Console.Write($"face[{n}] ");
                    }
                    if (is_volume)
                    {
                        Console.Write($"{w}x{h}x{v}: {layout.dds_size / v} * {v} = {layout.dds_size}\n");
                    }
                    else
                    {
                        Console.Write($"{w}x{h}: {layout.dds_size}\n");
                    }
                    

                    // next miplevel
                    if (!is_mip) break;

                    w >>= 1;
                    h >>= 1;
                    v >>= 1;
                    if (w == 0 && h == 0 && v == 0) break;
                    if (w == 0) w = 1;
                    if (h == 0) h = 1;
                    if (v == 0) v = 1;
                    ++m;
                }
            }

            return true;
        }

        
         public static bool convertBufferByLayout(ref byte[] gtf_image, ref byte[] dds_image, ref layout_t[] playout, int layout_num, CellGcmTexture tex, bool bDds2Gtf)
        {
            bool is_swizzle = gtfutil.gtfIsSwizzle(tex.format);

            byte raw_format = gtfutil.gtfGetRawFormat(tex.format);
            bool is_dxt = gtfutil.gtfIsDxtn(raw_format);

            Console.Write("  [Convert Info]\n");
            for (int i = 0; i < layout_num; ++i)
            {
                layout_t layout = playout[i];

                Console.Write($"    {layout.width}x{layout.height}x{layout.depth}\n");

                if (is_dxt)
                {
                    // dxtn format

                    // get offsets
                    int ddsoff = layout.dds_offset;
                    int gtfoff = layout.gtf_linear_offset;
                    if (is_swizzle)
                    {
                        gtfoff = layout.gtf_swizzle_offset;
                    }

                    int block_size = 16;
                    if (raw_format == CELL_GCM_TEXTURE_COMPRESSED_DXT1)
                    {
                        block_size = 8;
                    }

                    int block_width = (layout.width + 3) / 4;
                    int block_height = (layout.height + 3) / 4;
                    int block_depth = (layout.depth + 3) / 4;
                    int block_pitch = block_width * block_size;

                    int image_size = block_width * block_height * block_size;

                    if (is_swizzle)
                    {
                        // powor of 2 dxtn
                        if (tex.dimension == CELL_GCM_TEXTURE_DIMENSION_3)
                        {
                            // VTC
                            for (int z = 0; z < block_depth; ++z)
                            {
                                for (int y = 0; y < block_height; ++y)
                                {
                                    for (int x = 0; x < block_width; ++x)
                                    {
                                        int depth_block_num = (layout.depth % 4);
                                        if (depth_block_num == 0) depth_block_num = 4;

                                        for (int d_scan = 0; d_scan < depth_block_num; ++d_scan)
                                        {
                                            int ddsptr2 = (ddsoff + image_size * (z * 4 + d_scan));
                                            ddsptr2 += (block_size * (x + y * block_width));

                                            convertMoveMemory(ref gtf_image, gtfoff, ref dds_image, ddsptr2, block_size, bDds2Gtf);
                                            gtfoff += block_size;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // not VTC & powor of 2
                            convertMoveMemory(ref gtf_image, gtfoff, ref dds_image, ddsoff, image_size, bDds2Gtf);
                        }
                    }
                    else
                    {
                        // not powor of 2 dxtn
                        int dxt_pitch = gtfutil.gtfGetPitch(raw_format, tex.width);
                        for (int d = 0; d < layout.depth; ++d)
                        {
                            for (int block_line = 0; block_line < block_height; ++block_line)
                            {
                                convertMoveMemory(ref gtf_image, gtfoff + dxt_pitch * block_height * d + dxt_pitch * block_line, ref dds_image, ddsoff + image_size * d + block_pitch * block_line, block_pitch, bDds2Gtf);
                            }
                        }
                    }
                }
                else
                {
                    // not dxtn format

                    int invert_flag = getInvertFlag(tex);

                    if (is_swizzle)
                    {
                        // 64 bit and 128 bit fat texel swizzled memory layout can be described as "not quite swizzled."
                        if (raw_format == CELL_GCM_TEXTURE_W32_Z32_Y32_X32_FLOAT)
                        {
                            // A swizzled 128 bit texture is treated as a 32-bit texture that happens to be four times as wide.
                            layout.width = (ushort)(layout.width * 4);
                            layout.color_depth = 4;
                        }
                        else if (raw_format == CELL_GCM_TEXTURE_W16_Z16_Y16_X16_FLOAT)
                        {
                            // A swizzled 64 bit texture is treated as a 32-bit texture that happens to be twice as wide.
                            layout.width = (ushort)(layout.width * 2);
                            layout.color_depth = 4;
                        }
                    }

                    if (raw_format == CELL_GCM_TEXTURE_COMPRESSED_B8R8_G8R8_RAW ||
                        raw_format == CELL_GCM_TEXTURE_COMPRESSED_R8B8_R8G8_RAW)
                    {
                        layout.width = (ushort)((layout.width + 1) / 2 * 2);
                    }

                    if (!layout.dds_expand)
                    {
                        layout.dds_depth = layout.color_depth;
                        layout.dds_pitch = layout.width * layout.dds_depth;
                    }

                    for (int d = 0; d < layout.depth; ++d)
                    {
                        for (int line = 0; line < layout.height; ++line)
                        {

                            for (int x = 0; x < layout.width; ++x)
                            {

                                // get ptr
                                var ddsptr = layout.dds_offset + d * layout.dds_pitch * layout.height + line * layout.dds_pitch + x * layout.dds_depth;
                                var gtfptr = layout.gtf_linear_offset + d * layout.height * layout.pitch + line * layout.pitch + x * layout.color_depth;
                                if (is_swizzle)
                                {
                                    gtfptr = layout.gtf_swizzle_offset + utilToSwizzle(x, line, d, log2d(layout.width), log2d(layout.height), log2d(layout.depth)) * layout.color_depth;
                                }

                                if (bDds2Gtf && layout.dds_expand)
                                {
                                    // only dds2gtf
                                    if (layout.dds_depth == 2)
                                    {
                                        // FOURCC_R16F -> Y16_X16_FLOAT

                                        // copy to Y16 and invert endian16
                                        gtf_image[gtfptr] = dds_image[ddsptr+1];
                                        gtf_image[gtfptr+1] = dds_image[ddsptr];

                                        // copy to X16 and invert endian16
                                        gtf_image[gtfptr+2] = dds_image[ddsptr+1];
                                        gtf_image[gtfptr+3] = dds_image[ddsptr];
                                    }
                                    else if (layout.dds_depth == 3)
                                    {
                                        // 24bit RGB -> 32bit ARGB

                                        // copy and invert endian & padding A value
                                        gtf_image[gtfptr] = 0xFF;      // A
                                        gtf_image[gtfptr+1] = dds_image[ddsptr+2]; // R
                                        gtf_image[gtfptr+2] = dds_image[ddsptr+1]; // G
                                        gtf_image[gtfptr+3] = dds_image[ddsptr]; // B
                                    }
                                }
                                else if (invert_flag == CELL_GTFCONV_ENDIAN_INVERT16)
                                {
                                    // 16bit
                                    convertMoveMemoryWithInvertEndian16(ref gtf_image, gtfptr, ref dds_image, ddsptr, layout.color_depth, bDds2Gtf);
                                }
                                else if (invert_flag == CELL_GTFCONV_ENDIAN_INVERT32_EVEN)
                                {
                                    // B8R8_G8R8, R8B8_R8G8
                                    if ((x & 0x01) == 0)
                                    {
                                        // even pixel
                                        convertMoveMemoryWithInvertEndian32(ref gtf_image, gtfptr, ref dds_image, ddsptr, 4, bDds2Gtf);
                                    }
                                }
                                else if (invert_flag == CELL_GTFCONV_ENDIAN_INVERT32)
                                {
                                    // 32bit
                                    convertMoveMemoryWithInvertEndian32(ref gtf_image, gtfptr, ref dds_image, ddsptr, layout.color_depth, bDds2Gtf);
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }
        
    }
}
