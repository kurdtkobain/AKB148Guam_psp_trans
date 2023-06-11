namespace GTFDump
{
    internal class ddsinfo
    {
        public struct CellUtilDDSPixelFormat
        {

            public int size;
            public int flags;
            public int fourCC;
            public int rgbBitCount;
            public uint rbitMask;
            public uint gbitMask;
            public uint bbitMask;
            public uint abitMask;
        }

        public struct CellUtilDDSHeader
        {

            public int magic;
            public int size;
            public int flags;
            public int height;
            public int width;
            public int pitchOrLinearSize;
            public int depth;
            public int mipMapCount;
            public int[] reserved1 = new int[11];
            public CellUtilDDSPixelFormat ddspf;
            public int caps1;
            public int caps2;
            public int[] reserved2 = new int[3];

            public CellUtilDDSHeader()
            {
            }
        }

        public const int FOURCC_DDS  = ('D') | ('D') << 8 | ('S') << 16 | (' ') << 24;
        public const int FOURCC_DXT1 = ('D') | ('X') << 8 | ('T') << 16 | ('1') << 24;
        public const int FOURCC_DXT2 = ('D') | ('X') << 8 | ('T') << 16 | ('2') << 24;
        public const int FOURCC_DXT3 = ('D') | ('X') << 8 | ('T') << 16 | ('3') << 24;
        public const int FOURCC_DXT4 = ('D') | ('X') << 8 | ('T') << 16 | ('4') << 24;
        public const int FOURCC_DXT5 = ('D') | ('X') << 8 | ('T') << 16 | ('5') << 24;
        public const int FOURCC_RXGB = ('R') | ('X') << 8 | ('G') << 16 | ('B') << 24;
        public const int FOURCC_ATI1 = ('A') | ('T') << 8 | ('I') << 16 | ('1') << 24;
        public const int FOURCC_ATI2 = ('A') | ('T') << 8 | ('I') << 16 | ('2') << 24;


        // surface description flags
        public const int DDSF_MAX_MIPMAPS = 16;
        public const int DDSF_MAX_VOLUME = 512;
        public const int DDSF_MAX_TEXTURES = 16;

        public const int DDSF_CAPS = 0x00000001;
        public const int DDSF_HEIGHT = 0x00000002;
        public const int DDSF_WIDTH = 0x00000004;
        public const int DDSF_PITCH = 0x00000008;
        public const int DDSF_PIXELFORMAT = 0x00001000;
        public const int DDSF_MIPMAPCOUNT = 0x00020000;
        public const int DDSF_LINEARSIZE = 0x00080000;
        public const int DDSF_DEPTH = 0x00800000;

        // pixel format flags                
        public const int DDSF_ALPHAPIXELS = 0x00000001;
        public const int DDSF_FOURCC = 0x00000004;
        public const int DDSF_RGB = 0x00000040;
        public const int DDSF_RGBA = 0x00000041;
        public const int DDSF_LUMINANCE = 0x00020000;
        public const int DDSF_BUMPDUDV = 0x00080000;

        // dwCaps1 flags                     
        public const int DDSF_COMPLEX = 0x00000008;
        public const int DDSF_TEXTURE = 0x00001000;
        public const int DDSF_MIPMAP = 0x00400000;

        // dwCaps2 flags
        public const long DDSF_CUBEMAP = 0x00000200L;
        public const long DDSF_CUBEMAP_POSITIVEX = 0x00000400L;
        public const long DDSF_CUBEMAP_NEGATIVEX = 0x00000800L;
        public const long DDSF_CUBEMAP_POSITIVEY = 0x00001000L;
        public const long DDSF_CUBEMAP_NEGATIVEY = 0x00002000L;
        public const long DDSF_CUBEMAP_POSITIVEZ = 0x00004000L;
        public const long DDSF_CUBEMAP_NEGATIVEZ = 0x00008000L;
        public const long DDSF_CUBEMAP_ALL_FACES = 0x0000FC00L;
        public const long DDSF_VOLUME = 0x00200000L;

        public const int FOURCC_R16F = 0x6F;
        public const int FOURCC_G16R16F = 0x70;
        public const int FOURCC_A16B16G16R16F = 0x71;
        public const int FOURCC_R32F = 0x72;
        public const int FOURCC_A32B32G32R32F = 0x74;

        public const int FOURCC_YVYU = 0x59565955;
        public const int FOURCC_YUY2 = 0x32595559;
        public const int FOURCC_R8G8_B8G8 = 0x47424752;
        public const int FOURCC_G8R8_G8B8 = 0x42475247;


        /* RGB formats. */
        public const int D3DFMT_R8G8B8 = 20;
        public const int D3DFMT_A8R8G8B8 = 21;
        public const int D3DFMT_X8R8G8B8 = 22;
        public const int D3DFMT_R5G6B5 = 23;
        public const int D3DFMT_X1R5G5B5 = 24;
        public const int D3DFMT_A1R5G5B5 = 25;
        public const int D3DFMT_A4R4G4B4 = 26;
        public const int D3DFMT_R3G3B2 = 27;
        public const int D3DFMT_A8 = 28;
        public const int D3DFMT_A8R3G3B2 = 29;
        public const int D3DFMT_X4R4G4B4 = 30;
        public const int D3DFMT_A2B10G10R10 = 31;
        public const int D3DFMT_A8B8G8R8 = 32;
        public const int D3DFMT_X8B8G8R8 = 33;
        public const int D3DFMT_G16R16 = 34;
        public const int D3DFMT_A2R10G10B10 = 35;
        public const int D3DFMT_A16B16G16R16 = 36;

        /* Palette formats. */
        public const int D3DFMT_A8P8 = 40;
        public const int D3DFMT_P8 = 41;

        /* Luminance formats. */
        public const int D3DFMT_L8 = 50;
        public const int D3DFMT_A8L8 = 51;
        public const int D3DFMT_A4L4 = 52;

        /* Floating point formats */
        public const int D3DFMT_R16F = 111;
        public const int D3DFMT_G16R16F = 112;
        public const int D3DFMT_A16B16G16R16F = 113;
        public const int D3DFMT_R32F = 114;
        public const int D3DFMT_G32R32F = 115;
        public const int D3DFMT_A32B32G32R32F = 116;

        public const int DDSD_CAPS = 0x00000001;
        public const int DDSD_PIXELFORMAT = 0x00001000;
        public const int DDSD_WIDTH = 0x00000004;
        public const int DDSD_HEIGHT = 0x00000002;
        public const int DDSD_PITCH = 0x00000008;
        public const int DDSD_MIPMAPCOUNT = 0x00020000;
        public const int DDSD_LINEARSIZE = 0x00080000;
        public const int DDSD_DEPTH = 0x00800000;

        public const int DDSCAPS_ALPHA = 0x00000002;
        public const int DDSCAPS_COMPLEX = 0x00000008;
        public const int DDSCAPS_TEXTURE = 0x00001000;
        public const int DDSCAPS_MIPMAP = 0x00400000;
        public const int DDSCAPS2_VOLUME = 0x00200000;
        public const int DDSCAPS2_CUBEMAP = 0x00000200;

        public const int DDSCAPS2_CUBEMAP_POSITIVEX = 0x00000400;
        public const int DDSCAPS2_CUBEMAP_NEGATIVEX = 0x00000800;
        public const int DDSCAPS2_CUBEMAP_POSITIVEY = 0x00001000;
        public const int DDSCAPS2_CUBEMAP_NEGATIVEY = 0x00002000;
        public const int DDSCAPS2_CUBEMAP_POSITIVEZ = 0x00004000;
        public const int DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x00008000;
        public const int DDSCAPS2_CUBEMAP_ALL_FACES = 0x0000FC00;

        public const int DDPF_ALPHAPIXELS = 0x00000001;
        public const int DDPF_ALPHA = 0x00000002;
        public const int DDPF_FOURCC = 0x00000004;
        public const int DDPF_RGB = 0x00000040;
        public const int DDPF_PALETTEINDEXED1 = 0x00000800;
        public const int DDPF_PALETTEINDEXED2 = 0x00001000;
        public const int DDPF_PALETTEINDEXED4 = 0x00000008;
        public const int DDPF_PALETTEINDEXED8 = 0x00000020;
        public const int DDPF_LUMINANCE = 0x00020000;
        public const int DDPF_ALPHAPREMULT = 0x00008000;
        public const uint DDPF_NORMAL = 0x80000000U;   // @@ Custom nv flag.
    }
}
