using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MiscUtil.Conversion;
using MiscUtil.IO;
using AKB148GASBLib;

namespace ASBTests
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 1)
                RUNTest(args[0], true);
            else
                RUNTest(args[0]);
        }

        private static void RUNTest(string inFile, bool dumpHexToo = false)
        {
            
            List<ASBTools.ScriptData> ops = AKB148GASBLib.ASBTools.getScript(inFile);
           
                using (BinaryWriter b = new BinaryWriter(File.Open(Path.GetFileName(inFile) + ".txt", FileMode.Create)))
                {

                    foreach (ASBTools.ScriptData opc in ops)
                    {
                        b.Write(Encoding.UTF8.GetBytes(opcodeMeaning(opc, inFile)));
                        b.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                    }
                }
            if (dumpHexToo)
            {
                using (BinaryWriter b = new BinaryWriter(File.Open(Path.GetFileName(inFile) + ".HEX.txt", FileMode.Create)))
                {

                    foreach (ASBTools.ScriptData opc in ops)
                    {
                        foreach (byte by in opc.RawBytes)
                        {
                            b.Write(Encoding.UTF8.GetBytes(by.ToString("X2") + " "));
                        }
                        b.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                    }
                }

            }
        }

        private static string opcodeMeaning(ASBTools.ScriptData opc, string infile)
        {
            switch (opc.OPCode.ToString("X4"))
            {
                case "0000":
                    StringBuilder str = new StringBuilder();
                    str.Append("Special :");
                    foreach (byte b in opc.RawBytes)
                        str.Append(" " + b.ToString("X2"));
                    return str.ToString();
                case "4B29":
                    if (opc.paramNum == 3)
                    {
                        return "setSpeakerAsMemberWithVoice(" + opc.paramList[0].ToString("X4") + ", " + Members.getMemberName(opc.paramList[1]) + ", " + ASBTools.getDialogFromOffset(infile, opc.paramList[2]) + ")";
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
