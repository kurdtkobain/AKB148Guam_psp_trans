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

            public int dds_offset;
            public int dds_size;
            public int gtf_linear_offset;
            public int gtf_linear_size;
            public int gtf_swizzle_offset;
            public int gtf_swizzle_size;

            public ushort width;
            public ushort height;
            public ushort depth;
            public ushort color_depth;
            public int pitch;

            public int dds_pitch;
            public ushort dds_depth;
            public bool dds_expand;
        }

        public static int utilGetAlign(int size, int alignment)
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

        public static int my_min(int a, int b)
        {
            return (a < b ? a : b);
        }

        public static int my_max(int a, int b)
        {
            return (a > b ? a : b);
        }

        public static bool is_pow2(uint x)
        {
            return ((x & (x - 1)) == 0);
        }

        public static byte utilGetMipmapSize(ushort width, ushort height, ushort depth)
        {
            return (byte)(my_max(my_max(log2d(width), log2d(height)), log2d(depth)) + 1);
        }

        public static byte utilCountBit(int bits)
        {
            byte ret = 0;

            for (int i = 0; i < 32; ++i)
            {
                if (Convert.ToBoolean((bits >> i) & 0x01)) ++ret;
            }
            return ret;
        }

        public static void memmove_with_invert_endian16(ref byte[] dest, int destoff, ref byte[] src, int srcoff, int size)
        {
            int inptr16 = srcoff;
            int outptr16 = destoff;

            for (uint i = 0; i < size / 2; ++i)
            {
                dest[outptr16] = src[inptr16+1];
                dest[outptr16+1] = src[inptr16];

                inptr16 += 2;
                outptr16+= 2;
            }
            if (Convert.ToBoolean(size % 2))
            {
                Buffer.BlockCopy(src, inptr16, dest, outptr16, (size % 2));
                //memmove(outptr16, inptr16, size % 2);
            }
        }

        public static void memmove_with_invert_endian32(ref byte[] dest, int destoff, ref byte[] src, int srcoff, int size)
        {
            int inptr32 = srcoff;
            int outptr32 = destoff;

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
                Buffer.BlockCopy(src, inptr32, dest, outptr32, (size % 4));
                //memmove(outptr32, inptr32, size % 4);
            }
        }

        public static int utilToSwizzle(int x, int y, int z, int log2_width, int log2_height, int log2_depth)
        {
            int offset = 0;

            int t = 0;
            while (log2_width != 0 | log2_height != 0 | log2_depth != 0)
            {
                if (log2_width != 0)
                {
                    offset |= (x & 0x01) << t;
                    x >>= 1;
                    ++t;
                    --log2_width;
                }
                if (log2_height != 0)
                {
                    offset |= (y & 0x01) << t;
                    y >>= 1;
                    ++t;
                    --log2_height;
                }
                if (log2_depth != 0)
                {
                    offset |= (z & 0x01) << t;
                    z >>= 1;
                    ++t;
                    --log2_depth;
                }
            }

            return offset;
        }

    }
}
