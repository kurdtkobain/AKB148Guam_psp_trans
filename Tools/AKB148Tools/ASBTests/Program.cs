using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiscUtil.Conversion;
using MiscUtil.IO;
using System.Text.RegularExpressions;

namespace ASBTests
{

    class PointerTOC
    {
        public int sOffset;
        public int UKN1;
        public int UKN2;
        public int pOffset;
        public int pSize;
    }

    class PointerData
    {
        public int Offset;
        public int Size;
        public byte[] Data;
    }
    class OPCodes
    {
        public int OPCode;
        public int unk;
        public int paramNum;
        public List<int> paramList = new List<int>();
    }

    class Program
    {
        static void Main(string[] args)
        {
            RUNTest(args[0]);
            //Console.ReadKey();
        }

        private static void RUNTest(string inFile)
        {
            using (EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Little, File.Open(inFile, FileMode.Open, FileAccess.Read)))
            {
                reader.ReadInt32(); //always 0x00000000
                string fileName = Encoding.UTF8.GetString(reader.ReadBytes(15));
                reader.ReadByte(); //0x00
                reader.ReadBytes(16); //always 0's
                int UKN1 = reader.ReadInt32(); //always 0x44000000
                int UKN2 = reader.ReadInt32(); //always 0x99000000
                int nOffset = reader.ReadInt32(); //always 0x380C0000
                int nSize = reader.ReadInt32();
                int sOffset = reader.ReadInt32();
                int sSize = reader.ReadInt32();
                int sEOF = reader.ReadInt32();
                int UKN3 = reader.ReadInt32(); //always 0x09000000
                List<PointerTOC> tst = new List<PointerTOC>();
                while (reader.BaseStream.Position < nOffset)
                {
                    PointerTOC list = new PointerTOC();
                    list.sOffset = reader.ReadInt32();
                    list.UKN1 = reader.ReadInt32();
                    list.UKN2 = reader.ReadInt32();
                    list.pOffset = reader.ReadInt32();
                    list.pSize = reader.ReadInt32();
                    tst.Add(list);

                }
                long tmpoff = reader.BaseStream.Position;
                List<PointerData> pointD = new List<PointerData>();
                foreach (PointerTOC p in tst)
                {
                    PointerData pData = new PointerData();
                    pData.Offset = p.pOffset + nOffset;
                    reader.BaseStream.Position = pData.Offset;
                    pData.Size = p.pSize;
                    //pData.Data = new byte[pData.Size];
                    pData.Data = reader.ReadBytes(pData.Size);
                    pointD.Add(pData);
                }
                //MemoryStream pDataStream = new MemoryStream(pointD[pointD.Count - 2].Data);
                List<OPCodes> ops = new List<OPCodes>();
                reader.BaseStream.Position = pointD[pointD.Count - 2].Offset;
                checkOpcode(reader, ops, pointD[pointD.Count - 2].Size + pointD[pointD.Count - 2].Offset);
                Console.WriteLine("DEBUG");

                 using (BinaryWriter b = new BinaryWriter(File.Open(fileName+".txt", FileMode.Create)))
                  {
                      foreach (OPCodes opc in ops)
                      {
                          string tmp = String.Format("{0,-21} {1,-12} {2,-17} Param List: ",
                              "OPCode: " + opc.OPCode.ToString("X4"),
                              "UNKNOWN: " + opc.unk.ToString("X4"),
                              "Params: " + opc.paramNum.ToString());
                          b.Write(Encoding.UTF8.GetBytes(tmp));
                          foreach (int i in opc.paramList)
                          {
                              b.Write(Encoding.UTF8.GetBytes(i.ToString("X4")));
                              b.Write(Encoding.UTF8.GetBytes("  "));
                          }
                          b.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                      }
                  }

                /* using (BinaryWriter b = new BinaryWriter(File.Open(fileName + ".bin.txt", FileMode.Create)))
                 {
                     string temp = "";
                     foreach (byte byteD in pointD[pointD.Count - 2].Data)
                     {
                         temp += (byteD.ToString("X2")+" ");   
                     }


                     temp = temp.Replace("27 01 00 00 00 00", "<START>;");
                     temp = temp.Replace("27 03 00 00 00 01", "<BGChange>;");
                     temp = temp.Replace("27 04 00 00 00 01", "<UNK2704>;");
                     temp = temp.Replace("27 05 00 00 00 00", "<UNK2705>;");
                     temp = temp.Replace("27 0C 00 00 00 00", "<UNK270C>;");
                     temp = temp.Replace("27 12 00 00 00 00", "<UNK2712>;");
                     temp = temp.Replace("27 13 00 00 00 00", "<UNK2713>;");
                     temp = temp.Replace("27 16 00 00 00 00", "<Dialogue2716>;");
                     temp = temp.Replace("27 18 00 00 00 01", "<UNK2718>;");
                     temp = temp.Replace("27 19 00 00 00 00 2B", "<END>;");
                     temp = temp.Replace("27 1A 00 00 00 02", "<DialogueVoice>;");
                     temp = temp.Replace("27 1C 00 00 00 02", "<Dialogue271C>;");
                     temp = temp.Replace("27 21 00 00 00 01", "<SetupVoice???>;");
                     temp = temp.Replace("27 22 00 00 00 02", "<Dialogue2722>;");
                     temp = temp.Replace("27 27 00 00 00 01", "<UNK2727>;");
                     temp = temp.Replace("27 29 00 00 00 01", "<Dialogue2729>;");
                     temp = temp.Replace("27 2B 00 00 00 00 05", "<UNK272B>;");
                     temp = temp.Replace("27 30 00 00 00 01", "<Dialogue2730>;");
                     temp = temp.Replace("27 32 00 00 00 05", "<UNK2732>;");
                     temp = temp.Replace("27 49 00 00 00 00", "<UNK2749>;");
                     temp = temp.Replace("27 4A 00 00 00 00", "<UNK274A>;");
                     temp = temp.Replace("29 00 00 00 00 02", "<UNK2900>;");
                     temp = temp.Replace("29 02 00 00 00 03", "<UNK2902>;");
                     temp = temp.Replace("29 2A 00 00 00 01", "<AudioSoundFXPlay>;");
                     temp = temp.Replace("29 2A 03 00 00 00", "<AudioSoundFX???>;");
                     temp = temp.Replace("29 4B 01 00 00 02", "<DialoguePC>;");
                     temp = temp.Replace("29 4B 01 00 00 03", "<AudioVoice>;");
                     temp = temp.Replace("29 E2 03 00 00 01", "<UNK29E2>;");


                     temp = temp.Replace(";", Environment.NewLine);
                     b.Write(Encoding.UTF8.GetBytes(temp));
                 }*/

            }
        }

        private static void checkOpcode(EndianBinaryReader s, List<OPCodes> opC, int size)
        {
            OPCodes op = new OPCodes();
            while (s.BaseStream.Position < size)
            {
                byte tmp = s.ReadByte();
                if (tmp == 0x27 || tmp == 0x29)
                {
                    s.BaseStream.Position -= 1;
                    op.OPCode = s.ReadInt16();
                    op.unk = s.ReadInt16();
                    s.ReadByte();
                    op.paramNum = s.ReadByte();
                    opC.Add(op);
                    op = new OPCodes();
                }
                else if(tmp == 0x01)
                {
                    op.paramList.Add(s.ReadInt32());
                }
            }
        }
    }
}
