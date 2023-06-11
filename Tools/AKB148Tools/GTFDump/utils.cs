using System;

namespace GTFDump
{
    internal class utils
    {

        // endian util
        public static bool is_little_endian()
        {
            return BitConverter.IsLittleEndian;
        }

        public static bool is_big_endian()
        {
            return !is_little_endian();
        }

        public static uint invert_endian32(uint v)
        {
            return (v << 24) | ((v << 8) & 0x00FF0000) | ((v >> 8) & 0x0000FF00) | ((v >> 24) & 0x000000FF);
        }

        public static uint invert_endian32_cross(uint v)
        {
            return ((v << 8) & 0xFF00FF00) | ((v >> 8) & 0x00FF00FF);
        }

        public static ushort invert_endian16(ushort v)
        {
            return (ushort)((v << 8) | ((v >> 8) & 0x00FF));
        }

        public static uint to_big_endian32(uint v)
        {
            if (is_big_endian()) return v;

            return invert_endian32(v);
        }

        public static ushort to_big_endian16(ushort v)
        {
            if (is_big_endian()) return v;

            return invert_endian16(v);
        }

        public static uint to_little_endian32(uint v)
        {
            if (is_little_endian()) return v;

            return invert_endian32(v);
        }

        public static ushort to_little_endian16(ushort v)
        {
            if (is_little_endian()) return v;

            return invert_endian16(v);
        }


        public static uint from_big_endian32(uint v)
        {
            if (is_big_endian()) return v;

            return invert_endian32(v);
        }

        public static ushort from_big_endian16(ushort v)
        {
            if (is_big_endian()) return v;

            return invert_endian16(v);
        }

        public static uint from_little_endian32(uint v)
        {
            if (is_little_endian()) return v;

            return invert_endian32(v);
        }

        public static ushort from_little_endian16(ushort v)
        {
            if (is_little_endian()) return v;

            return invert_endian16(v);
        }


        public struct layout_t
        {

            public uint dds_offset;
            public uint dds_size;
            public uint gtf_linear_offset;
            public uint gtf_linear_size;
            public uint gtf_swizzle_offset;
            public uint gtf_swizzle_size;

            public ushort width;
            public ushort height;
            public ushort depth;
            public ushort color_depth;
            public uint pitch;

            public uint dds_pitch;
            public ushort dds_depth;
            public bool dds_expand;
        }

        public static uint utilGetAlign(uint size, uint alignment)
        {
            return (size + alignment - 1) & ~(alignment - 1);
        }

        public static float log2(float value)
        {
            return (float)(Math.Log(value) / Math.Log(2.0f));
        }

        public static int log2d(int value)
        {
            return (value > 0) ? (int)log2(value) : -1;
        }

        public static float power(float left, uint right)
        {
            float retf = 1.0f;
            for (uint i = 0; i < right; ++i)
            {
                retf *= left;
            }
            return retf;
        }

        public static uint my_min(uint a, uint b)
        {
            return (a < b ? a : b);
        }

        public static uint my_max(uint a, uint b)
        {
            return (a > b ? a : b);
        }

        public static bool is_pow2(uint x)
        {
            return ((x & (x - 1)) == 0);
        }

        public static byte utilGetMipmapSize(ushort width, ushort height, ushort depth)
        {
            return (byte)(my_max(my_max((uint)log2d(width), (uint)log2d(height)), (uint)log2d(depth)) + 1);
        }

        public static byte utilCountBit(uint bits)
        {
            byte ret = 0;

            for (int i = 0; i < 32; ++i)
            {
                if (Convert.ToBoolean((bits >> i) & 0x01)) ++ret;
            }
            return ret;
        }

        public static void memmove_with_invert_endian16(ref byte[] dest, uint destoff, ref byte[] src, uint srcoff, uint size)
        {
            uint inptr16 = srcoff;
            uint outptr16 = destoff;

            for (uint i = 0; i < size / 2; ++i)
            {
                dest[outptr16] = src[inptr16+1];
                dest[outptr16+1] = src[inptr16];

                inptr16 += 2;
                outptr16+= 2;
            }
            if (Convert.ToBoolean(size % 2))
            {
                Buffer.BlockCopy(src, (int)inptr16, dest, (int)outptr16, (int)(size % 2));
                //memmove(outptr16, inptr16, size % 2);
            }
        }

        public static void memmove_with_invert_endian32(ref byte[] dest, uint destoff, ref byte[] src, uint srcoff, uint size)
        {
            uint inptr32 = srcoff;
            uint outptr32 = destoff;

            for (uint i = 0; i < size / 4; ++i)
            {
                dest[outptr32] = src[inptr32 + 3];
                dest[outptr32 + 1] = src[inptr32+2];
                dest[outptr32 + 2] = src[inptr32+1];
                dest[outptr32 + 3] = src[inptr32];

                inptr32 += 4;
                outptr32 += 4;
            }
            if (Convert.ToBoolean(size % 4))
            {
                Buffer.BlockCopy(src, (int)inptr32, dest, (int)outptr32, (int)(size % 4));
                //memmove(outptr32, inptr32, size % 4);
            }
        }

        public static uint utilToSwizzle(uint x, uint y, uint z, uint log2_width, uint log2_height, uint log2_depth)
        {
            uint offset = 0;

            uint t = 0;
            while (log2_width != 0 | log2_height != 0 | log2_depth != 0)
            {
                if (log2_width != 0)
                {
                    offset |= (x & 0x01) << (int)t;
                    x >>= 1;
                    ++t;
                    --log2_width;
                }
                if (log2_height != 0)
                {
                    offset |= (y & 0x01) << (int)t;
                    y >>= 1;
                    ++t;
                    --log2_height;
                }
                if (log2_depth != 0)
                {
                    offset |= (z & 0x01) << (int)t;
                    z >>= 1;
                    ++t;
                    --log2_depth;
                }
            }

            return offset;
        }

    }
}
