namespace GTFDump
{
    internal class ddsinfo
    {
        public struct CellUtilDDSPixelFormat
        {

            public uint size;
            public uint flags;
            public uint fourCC;
            public uint rgbBitCount;
            public uint rbitMask;
            public uint gbitMask;
            public uint bbitMask;
            public uint abitMask;
        }

        public struct CellUtilDDSHeader
        {

            public uint magic;
            public uint size;
            public uint flags;
            public uint height;
            public uint width;
            public uint pitchOrLinearSize;
            public uint depth;
            public uint mipMapCount;
            public uint[] reserved1 = new uint[11];
            public CellUtilDDSPixelFormat ddspf;
            public uint caps1;
            public uint caps2;
            public uint[] reserved2 = new uint[3];

            public CellUtilDDSHeader()
            {
            }
        }

        private static uint MAKEFOURCC(char ch0, char ch1, char ch2, char ch3) {
            return ((uint)((char)(ch0)) | ((uint)((char)(ch1)) << 8) | ((uint)((char)(ch2)) << 16) | ((uint)((char)(ch3)) << 24));
                }

        public const uint FOURCC_DDS = ((uint)((char)('D')) | ((uint)((char)('D')) << 8) | ((uint)((char)('S')) << 16) | ((uint)((char)(' ')) << 24));
        public const uint FOURCC_DXT1 = ((uint)((char)('D')) | ((uint)((char)('X')) << 8) | ((uint)((char)('T')) << 16) | ((uint)((char)('1')) << 24));
        public const uint FOURCC_DXT2 = ((uint)((char)('D')) | ((uint)((char)('X')) << 8) | ((uint)((char)('T')) << 16) | ((uint)((char)('2')) << 24));
        public const uint FOURCC_DXT3 = ((uint)((char)('D')) | ((uint)((char)('X')) << 8) | ((uint)((char)('T')) << 16) | ((uint)((char)('3')) << 24));
        public const uint FOURCC_DXT4 = ((uint)((char)('D')) | ((uint)((char)('X')) << 8) | ((uint)((char)('T')) << 16) | ((uint)((char)('4')) << 24));
        public const uint FOURCC_DXT5 = ((uint)((char)('D')) | ((uint)((char)('X')) << 8) | ((uint)((char)('T')) << 16) | ((uint)((char)('5')) << 24));
        public const uint FOURCC_RXGB = ((uint)((char)('R')) | ((uint)((char)('X')) << 8) | ((uint)((char)('G')) << 16) | ((uint)((char)('B')) << 24));
        public const uint FOURCC_ATI1 = ((uint)((char)('A')) | ((uint)((char)('T')) << 8) | ((uint)((char)('I')) << 16) | ((uint)((char)('1')) << 24));
        public const uint FOURCC_ATI2 = ((uint)((char)('A')) | ((uint)((char)('T')) << 8) | ((uint)((char)('I')) << 16) | ((uint)((char)('2')) << 24));


        // surface description flags
        public const uint DDSF_MAX_MIPMAPS = 16;
        public const uint DDSF_MAX_VOLUME = 512;
        public const uint DDSF_MAX_TEXTURES = 16;

        public const uint DDSF_CAPS = 0x00000001;
        public const uint DDSF_HEIGHT = 0x00000002;
        public const uint DDSF_WIDTH = 0x00000004;
        public const uint DDSF_PITCH = 0x00000008;
        public const uint DDSF_PIXELFORMAT = 0x00001000;
        public const uint DDSF_MIPMAPCOUNT = 0x00020000;
        public const uint DDSF_LINEARSIZE = 0x00080000;
        public const uint DDSF_DEPTH = 0x00800000;

        // pixel format flags                
        public const uint DDSF_ALPHAPIXELS = 0x00000001;
        public const uint DDSF_FOURCC = 0x00000004;
        public const uint DDSF_RGB = 0x00000040;
        public const uint DDSF_RGBA = 0x00000041;
        public const uint DDSF_LUMINANCE = 0x00020000;
        public const uint DDSF_BUMPDUDV = 0x00080000;

        // dwCaps1 flags                     
        public const uint DDSF_COMPLEX = 0x00000008;
        public const uint DDSF_TEXTURE = 0x00001000;
        public const uint DDSF_MIPMAP = 0x00400000;

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

        public const uint FOURCC_R16F = 0x6F;
        public const uint FOURCC_G16R16F = 0x70;
        public const uint FOURCC_A16B16G16R16F = 0x71;
        public const uint FOURCC_R32F = 0x72;
        public const uint FOURCC_A32B32G32R32F = 0x74;

        public const uint FOURCC_YVYU = 0x59565955;
        public const uint FOURCC_YUY2 = 0x32595559;
        public const uint FOURCC_R8G8_B8G8 = 0x47424752;
        public const uint FOURCC_G8R8_G8B8 = 0x42475247;


        /* RGB formats. */
        public const uint D3DFMT_R8G8B8 = 20;
        public const uint D3DFMT_A8R8G8B8 = 21;
        public const uint D3DFMT_X8R8G8B8 = 22;
        public const uint D3DFMT_R5G6B5 = 23;
        public const uint D3DFMT_X1R5G5B5 = 24;
        public const uint D3DFMT_A1R5G5B5 = 25;
        public const uint D3DFMT_A4R4G4B4 = 26;
        public const uint D3DFMT_R3G3B2 = 27;
        public const uint D3DFMT_A8 = 28;
        public const uint D3DFMT_A8R3G3B2 = 29;
        public const uint D3DFMT_X4R4G4B4 = 30;
        public const uint D3DFMT_A2B10G10R10 = 31;
        public const uint D3DFMT_A8B8G8R8 = 32;
        public const uint D3DFMT_X8B8G8R8 = 33;
        public const uint D3DFMT_G16R16 = 34;
        public const uint D3DFMT_A2R10G10B10 = 35;
        public const uint D3DFMT_A16B16G16R16 = 36;

        /* Palette formats. */
        public const uint D3DFMT_A8P8 = 40;
        public const uint D3DFMT_P8 = 41;

        /* Luminance formats. */
        public const uint D3DFMT_L8 = 50;
        public const uint D3DFMT_A8L8 = 51;
        public const uint D3DFMT_A4L4 = 52;

        /* Floating point formats */
        public const uint D3DFMT_R16F = 111;
        public const uint D3DFMT_G16R16F = 112;
        public const uint D3DFMT_A16B16G16R16F = 113;
        public const uint D3DFMT_R32F = 114;
        public const uint D3DFMT_G32R32F = 115;
        public const uint D3DFMT_A32B32G32R32F = 116;

        public const uint DDSD_CAPS = 0x00000001U;
        public const uint DDSD_PIXELFORMAT = 0x00001000U;
        public const uint DDSD_WIDTH = 0x00000004U;
        public const uint DDSD_HEIGHT = 0x00000002U;
        public const uint DDSD_PITCH = 0x00000008U;
        public const uint DDSD_MIPMAPCOUNT = 0x00020000U;
        public const uint DDSD_LINEARSIZE = 0x00080000U;
        public const uint DDSD_DEPTH = 0x00800000U;

        public const uint DDSCAPS_ALPHA = 0x00000002U;
        public const uint DDSCAPS_COMPLEX = 0x00000008U;
        public const uint DDSCAPS_TEXTURE = 0x00001000U;
        public const uint DDSCAPS_MIPMAP = 0x00400000U;
        public const uint DDSCAPS2_VOLUME = 0x00200000U;
        public const uint DDSCAPS2_CUBEMAP = 0x00000200U;

        public const uint DDSCAPS2_CUBEMAP_POSITIVEX = 0x00000400U;
        public const uint DDSCAPS2_CUBEMAP_NEGATIVEX = 0x00000800U;
        public const uint DDSCAPS2_CUBEMAP_POSITIVEY = 0x00001000U;
        public const uint DDSCAPS2_CUBEMAP_NEGATIVEY = 0x00002000U;
        public const uint DDSCAPS2_CUBEMAP_POSITIVEZ = 0x00004000U;
        public const uint DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x00008000U;
        public const uint DDSCAPS2_CUBEMAP_ALL_FACES = 0x0000FC00U;

        public const uint DDPF_ALPHAPIXELS = 0x00000001U;
        public const uint DDPF_ALPHA = 0x00000002U;
        public const uint DDPF_FOURCC = 0x00000004U;
        public const uint DDPF_RGB = 0x00000040U;
        public const uint DDPF_PALETTEINDEXED1 = 0x00000800U;
        public const uint DDPF_PALETTEINDEXED2 = 0x00001000U;
        public const uint DDPF_PALETTEINDEXED4 = 0x00000008U;
        public const uint DDPF_PALETTEINDEXED8 = 0x00000020U;
        public const uint DDPF_LUMINANCE = 0x00020000U;
        public const uint DDPF_ALPHAPREMULT = 0x00008000U;
        public const uint DDPF_NORMAL = 0x80000000U;   // @@ Custom nv flag.
    }
}
