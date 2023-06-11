namespace GTFDump
{
    internal class gtfinfo
    {
        public struct CellGtfFileHeader
        {

            public uint version;      // Version (Correspond to dds2gtf converter version)
            public uint size;         // Total size of Texture. (Excluding size of header & attribute)
            public uint numTexture;   // Number of textures in this file.

        }

        public struct CellGcmTexture
        {
            public byte format;
            public byte mipmap;
            public byte dimension;
            public byte cubemap;
            public uint remap;
            public ushort width;
            public ushort height;
            public ushort depth;
            public byte location;
            public byte _padding;
            public uint pitch;
            public uint offset;
        }

        public struct CellGtfTextureAttribute
        {
            public uint id;            // Texture ID.  
            public uint offsetToTex;   // Offset to texture from begining of file.
            public uint textureSize;   // Size of texture.
            public CellGcmTexture tex;		// Texture structure defined in GCM library.
        }


        //	Enable
        public const byte CELL_GCM_FALSE = (0);
        public const byte CELL_GCM_TRUE = (1);

        // Location
        public const byte CELL_GCM_LOCATION_LOCAL = (0);
        public const byte CELL_GCM_LOCATION_MAIN = (1);

        // SetTexture
        public const byte CELL_GCM_TEXTURE_B8 = (0x81);
        public const byte CELL_GCM_TEXTURE_A1R5G5B5 = (0x82);
        public const byte CELL_GCM_TEXTURE_A4R4G4B4 = (0x83);
        public const byte CELL_GCM_TEXTURE_R5G6B5 = (0x84);
        public const byte CELL_GCM_TEXTURE_A8R8G8B8 = (0x85);
        public const byte CELL_GCM_TEXTURE_COMPRESSED_DXT1 = (0x86);
        public const byte CELL_GCM_TEXTURE_COMPRESSED_DXT23 = (0x87);
        public const byte CELL_GCM_TEXTURE_COMPRESSED_DXT45 = (0x88);
        public const byte CELL_GCM_TEXTURE_G8B8 = (0x8B);
        public const byte CELL_GCM_TEXTURE_R6G5B5 = (0x8F);
        public const byte CELL_GCM_TEXTURE_DEPTH24_D8 = (0x90);
        public const byte CELL_GCM_TEXTURE_DEPTH24_D8_FLOAT = (0x91);
        public const byte CELL_GCM_TEXTURE_DEPTH16 = (0x92);
        public const byte CELL_GCM_TEXTURE_DEPTH16_FLOAT = (0x93);
        public const byte CELL_GCM_TEXTURE_X16 = (0x94);
        public const byte CELL_GCM_TEXTURE_Y16_X16 = (0x95);
        public const byte CELL_GCM_TEXTURE_R5G5B5A1 = (0x97);
        public const byte CELL_GCM_TEXTURE_COMPRESSED_HILO8 = (0x98);
        public const byte CELL_GCM_TEXTURE_COMPRESSED_HILO_S8 = (0x99);
        public const byte CELL_GCM_TEXTURE_W16_Z16_Y16_X16_FLOAT = (0x9A);
        public const byte CELL_GCM_TEXTURE_W32_Z32_Y32_X32_FLOAT = (0x9B);
        public const byte CELL_GCM_TEXTURE_X32_FLOAT = (0x9C);
        public const byte CELL_GCM_TEXTURE_D1R5G5B5 = (0x9D);
        public const byte CELL_GCM_TEXTURE_D8R8G8B8 = (0x9E);
        public const byte CELL_GCM_TEXTURE_Y16_X16_FLOAT = (0x9F);
        public const byte CELL_GCM_TEXTURE_COMPRESSED_B8R8_G8R8 = (0xAD);
        public const byte CELL_GCM_TEXTURE_COMPRESSED_R8B8_R8G8 = (0xAE);
        public const byte CELL_GCM_TEXTURE_COMPRESSED_B8R8_G8R8_RAW = (0x8D);
        public const byte CELL_GCM_TEXTURE_COMPRESSED_R8B8_R8G8_RAW = (0x8E);

        public const byte CELL_GCM_TEXTURE_SZ = (0x00);
        public const byte CELL_GCM_TEXTURE_LN = (0x20);
        public const byte CELL_GCM_TEXTURE_NR = (0x00);
        public const byte CELL_GCM_TEXTURE_UN = (0x40);

        public const byte CELL_GCM_TEXTURE_DIMENSION_1 = (1);
        public const byte CELL_GCM_TEXTURE_DIMENSION_2 = (2);
        public const byte CELL_GCM_TEXTURE_DIMENSION_3 = (3);

        public const byte CELL_GCM_TEXTURE_REMAP_ORDER_XYXY = (0);
        public const byte CELL_GCM_TEXTURE_REMAP_ORDER_XXXY = (1);
        public const byte CELL_GCM_TEXTURE_REMAP_FROM_A = (0);
        public const byte CELL_GCM_TEXTURE_REMAP_FROM_R = (1);
        public const byte CELL_GCM_TEXTURE_REMAP_FROM_G = (2);
        public const byte CELL_GCM_TEXTURE_REMAP_FROM_B = (3);
        public const byte CELL_GCM_TEXTURE_REMAP_ZERO = (0);
        public const byte CELL_GCM_TEXTURE_REMAP_ONE = (1);
        public const byte CELL_GCM_TEXTURE_REMAP_REMAP = (2);



    }
}
