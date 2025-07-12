using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APKUnpack
{
    internal class APK
    {
        public class PACKHEDR
        {
            public long headerSize;
            public int dummy;
            public int zero;
            public int dummySize;
            public int dummy2;
            public string dummyFill;

            public PACKHEDR(EndianBinaryReader reader)
            {
                if (Encoding.ASCII.GetString(reader.ReadBytes(8)) != "PACKHEDR")
                {
                    throw new Exception("WRONG PACKHEDR");
                }
                headerSize = reader.ReadInt64();
                dummy = reader.ReadInt32();
                zero = reader.ReadInt32();
                dummySize = reader.ReadInt32();
                dummy2 = reader.ReadInt32();
                dummyFill = Encoding.ASCII.GetString(reader.ReadBytes(16));
            }
        }

        public class PACKTOC
        {
            public long headerSize;
            public long TOCoffset;
            public int entrySize;
            public long count;
            public uint offset;
            public long zero;
            public List<PACKTOCENTRY> entries;
            public PACKTOC(EndianBinaryReader reader)
            {
                if (Encoding.ASCII.GetString(reader.ReadBytes(8)) != "PACKTOC ")
                {
                    throw new Exception("WRONG PACKTOC");
                }
                headerSize = reader.ReadInt64();
                TOCoffset = reader.BaseStream.Position;
                entrySize = reader.ReadInt32();
                count = reader.ReadInt32();
                offset = reader.ReadUInt32();
                zero = reader.ReadInt32();
                entries = new List<PACKTOCENTRY>();
                for (int l = 0; l < count; l++)
                {
                    var entry = new PACKTOCENTRY(reader);
                    entries.Add(entry);
                }
            }
        }

        public class PACKTOCENTRY
        {
            public int flags;
            public int stringIndex;
            public int headerIndex;
            public int zero0;
            public uint offset;
            public int count;
            public long decompSize;
            public long compSize;

            public PACKTOCENTRY(EndianBinaryReader reader)
            {
                flags = reader.ReadInt32(); //0x200 == Deflate  |  0x300 == LZMA 
                stringIndex = reader.ReadInt32();
                headerIndex = reader.ReadInt32(); //always 0
                zero0 = reader.ReadInt32(); //always 0
                offset = reader.ReadUInt32();
                count = reader.ReadInt32();
                decompSize = reader.ReadInt64();
                compSize = reader.ReadInt64();
            }
        }

        public class PACKFSLS
        {
            public long headerSize;
            public long archives;
            public long dummy;
            public long dummy2;
            public long zero;
            public GENESTRT strList;

            public PACKFSLS(EndianBinaryReader reader)
            {
                if (Encoding.ASCII.GetString(reader.ReadBytes(8)) != "PACKFSLS")
                {
                    throw new Exception("WRONG PACKFSLS");
                }
                headerSize = reader.ReadInt64();
                dummy = reader.ReadInt32();
                archives = reader.ReadInt32();
                dummy2 = reader.ReadInt32();
                zero = reader.ReadInt32();
                long tmppos = reader.BaseStream.Position;
                strList = new GENESTRT(reader);
            }
        }

        public class GENESTRT
        {
            public long offset;
            public long stringcnt;
            public long tableOffset;
            public long dataOffset;
            public long sectionSize;
            public long[] nameofflist;
            public long names_off;
            public string[] stringlist;

            public GENESTRT(EndianBinaryReader reader)
            {
                if (Encoding.ASCII.GetString(reader.ReadBytes(8)) != "GENESTRT")
                {
                    throw new Exception("WRONG GENESTRT");
                }
                
                reader.ReadInt64();
                long tmppos = reader.BaseStream.Position;
                stringcnt = reader.ReadInt32();
                tableOffset = reader.ReadInt32();
                dataOffset = reader.ReadInt32();
                sectionSize = reader.ReadInt32();
                nameofflist = new long[stringcnt];
                long dataloc = tmppos + dataOffset;
                for (int i = 0; i < stringcnt; i++)
                {
                    nameofflist[i] = reader.ReadInt32();
                }
                stringlist = new string[stringcnt];
                for (int j = 0; j < stringcnt; j++)
                {
                    reader.BaseStream.Position = dataloc + nameofflist[j];
                    stringlist[j] = ReadStringZ(reader);
                }
            }

            private static string ReadStringZ(EndianBinaryReader reader)
            {
                string result = "";
                char c;
                for (int i = 0; i < 255; i++)
                {
                    if ((c = (char)reader.ReadByte()) == 0)
                    {
                        break;
                    }
                    result += c.ToString();
                }
                return result;
            }
        }
    }
}
