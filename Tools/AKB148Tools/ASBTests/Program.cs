using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MiscUtil.Conversion;
using MiscUtil.IO;
using AKB148GASBLib;

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
        public int paramNum;
        public int UKN1;
        public int UKN2;
        public int UKNFlag;
        public int OPCode;
        public byte[] Special;

        public List<int> paramList = new List<int>();
    }

    class Program
    {
        static void Main(string[] args)
        {
            FileStream ostrm;
            StreamWriter writer;
            TextWriter oldOut = Console.Out;
            try
            {
                ostrm = new FileStream("./Redirect.txt", FileMode.Create, FileAccess.Write);
                writer = new StreamWriter(ostrm);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open Redirect.txt for writing");
                Console.WriteLine(e.Message);
                return;
            }
            Console.SetOut(writer);
            if (args.Length > 1)
                RUNTest(args[0], true);
            else
                RUNTest(args[0]);
            Console.SetOut(oldOut);
            writer.Close();
            ostrm.Close();
            Console.WriteLine("Done");
        }

        private static void RUNTest(string inFile, bool dumpHexOnly = false)
        {
            EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Little, File.Open(inFile, FileMode.Open, FileAccess.Read));
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
            reader.Close();
            Array.Reverse(pointD[pointD.Count - 2].Data);
            if (dumpHexOnly)
            {

                using (BinaryWriter b = new BinaryWriter(File.Open(fileName + ".HEX.txt", FileMode.Create)))
                {
                    foreach (byte opc in pointD[pointD.Count - 2].Data)
                    {
                        b.Write(Encoding.UTF8.GetBytes(opc.ToString("X2")));
                        b.Write(Encoding.UTF8.GetBytes(" "));
                    }
                }
                return;
            }
            MemoryStream pDataStream = new MemoryStream(pointD[pointD.Count - 2].Data);
            EndianBinaryReader R = new EndianBinaryReader(EndianBitConverter.Big, pDataStream);
            List<OPCodes> ops = new List<OPCodes>();
            checkOpcode(R, ops,pointD[pointD.Count -2].Size);
            R.Close();
            ops.Reverse();
            using (BinaryWriter b = new BinaryWriter(File.Open(fileName + ".txt", FileMode.Create)))
            {
                foreach (OPCodes opc in ops)
                {
                    b.Write(Encoding.UTF8.GetBytes(opcodeMeaning(opc, inFile)));
                    b.Write(Encoding.UTF8.GetBytes(Environment.NewLine));

                    /*string tmp = String.Format("{0,-21} {1,-12} {2,-17} Param List: ",
                        "OPCode: " + opc.OPCode.ToString("X4"),
                        "UNKNOWN: " + opc.unk.ToString("X4"),
                        "Params: " + opc.paramNum.ToString());
                    b.Write(Encoding.UTF8.GetBytes(tmp));
                    foreach (int i in opc.paramList)
                    {
                        b.Write(Encoding.UTF8.GetBytes(i.ToString("X4")));
                        b.Write(Encoding.UTF8.GetBytes("  "));
                    }
                    b.Write(Encoding.UTF8.GetBytes(Environment.NewLine));*/
                }
            }
        }

        private static void checkOpcode(EndianBinaryReader s, List<OPCodes> opC, int size)
        {

            OPCodes op = new OPCodes();
            Console.WriteLine(s.ReadByte().ToString("X2") + " ");
            while (s.BaseStream.Position < size)
            {
                op.paramNum = s.ReadByte();
                Console.Write(op.paramNum.ToString("X2") + " ");
                op.UKN1 = s.ReadByte();
                Console.Write(op.UKN1.ToString("X2") + " ");
                op.UKN2 = s.ReadByte();
                Console.Write(op.UKN2.ToString("X2") + " ");
                if (op.UKN2 != 0x00)
                {
                    op.UKNFlag = s.ReadByte();
                    Console.Write(op.UKNFlag.ToString("X2") + " ");
                    byte tmpByte = s.ReadByte();

                    if (tmpByte != 0x21)
                    {
                        if (tmpByte == 0x00) {
                            op.Special = s.ReadBytes(14);
                            foreach (byte b in op.Special)
                                Console.Write(b.ToString("X2") + " ");
                        }
                        else
                        {
                            op.Special = new byte[] { tmpByte, s.ReadByte() };
                            foreach (byte b in op.Special)
                                Console.Write(b.ToString("X2") + " ");
                        }
                    }
                    else
                    {
                        Console.Write(tmpByte.ToString("X2") + " ");
                    }
                }
                else
                {
                    op.UKNFlag = s.ReadByte();
                    Console.Write(op.UKNFlag.ToString("X2") + " ");
                    op.OPCode = s.ReadInt16();
                    Console.Write(op.OPCode.ToString("X4") + " ");
                    if ((op.OPCode == 0x0000 && op.UKNFlag == 0x00) || op.UKNFlag == 0x02 || (op.UKNFlag == 0x01 && op.OPCode == 0x0000))
                    {
                        
                        op.Special = s.ReadBytes(3);
                        foreach (byte b in op.Special)
                            Console.Write(b.ToString("X2") + " ");
                    }
                    else{
                        if (op.paramNum != 0)
                        {
                            for (int g = 0; g < op.paramNum; g++)
                            {
                                int tmpprams = s.ReadInt32();
                                op.paramList.Add(tmpprams);
                                foreach(int p in op.paramList)
                                    Console.Write(p.ToString("X4") + " ");
                                Console.Write(s.ReadByte().ToString("X2") + " ");

                            }
                        }
                    }
                }
                op.paramList.Reverse();
                opC.Add(op);
                op = new OPCodes();
                Console.Write(Environment.NewLine);

            }

            /*OPCodes op = new OPCodes();
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
                else if (tmp == 0x01)
                {
                    op.paramList.Add(s.ReadInt32());
                    
                }
            }*/
        }

        private static string opcodeMeaning(OPCodes opc, string infile)
        {
            switch (opc.OPCode.ToString("X4"))
            {
                case "4B29":
                    if (opc.paramNum == 3)
                    {
                        return "setSpeakerAsMemberWithEvent(" + opc.paramList[0].ToString("X4") + ", " + Members.getMemberName(opc.paramList[1]) + ", " + ASBTools.getDialogFromOffset(infile, opc.paramList[2]) + ")";
                    }
                    else
                    {
                        return "setSpeakerAsPlayer(" + opc.paramList[0].ToString("X4") + "," + Members.getMemberName(opc.paramList[1]) + ")";
                    }

                case "0427":
                    return "SetLocName(" + ASBTools.getDialogFromOffset(infile, opc.paramList[0]) + ")";

                case "0327":
                    return "SetBGImage(" + opc.paramList[0].ToString() + ")";

                case "0029":
                    return "setDialogue(" + ASBTools.getDialogFromOffset(infile, opc.paramList[0]) + ", " + opc.paramList[1].ToString("X4") + ")";

                case "1A27":
                    return "setMemberWithUKN(" + Members.getMemberName(opc.paramList[0]) + ", " + opc.paramList[1].ToString("X4") + ")";

                case "1C27":
                    return "setSpeakerWithImage(" + Members.getMemberName(opc.paramList[0]) + ", " + opc.paramList[1].ToString() + ")";

                case "0229":
                    return "setChoice(" + ASBTools.getDialogFromOffset(infile, opc.paramList[0]) + ", " + ASBTools.getDialogFromOffset(infile, opc.paramList[1]) + ", " + ASBTools.getDialogFromOffset(infile, opc.paramList[2]) + ")";

                case "2927":
                    if (opc.paramList.Count != 0)
                    {
                        return "CHOICE_UNK(" + opc.paramList[0].ToString("X4") + ")";
                    }
                    else
                    {
                        return "CHOICE_UNK()";
                    }

                case "2A29":
                    if (opc.paramList.Count != 0)
                    {
                        return "playSE(" + ASBTools.getDialogFromOffset(infile, opc.paramList[0]) + ")";
                    }
                    else
                    {
                        return "UKNSEffect()";
                    }

                default:
                    string tmp = "UKN" + opc.OPCode.ToString("X4") + "(";
                    if (opc.paramNum != 0 && opc.paramList.Count != 0)
                    {
                        for (int i = 0; i < opc.paramNum; i++)
                        {
                            tmp += opc.paramList[i].ToString("X4");
                            if ((i + 1) != opc.paramNum)
                            {
                                tmp += " , ";
                            }
                        }
                    }
                    tmp += ")";
                    return tmp;
            }
        }
    }
}
