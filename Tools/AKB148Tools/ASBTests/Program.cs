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
        static List<ASBTools.DialougeData> diagDataList;
        static void Main(string[] args)
        {
            FileAttributes attr = File.GetAttributes(args[0]);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                foreach (string s in Directory.GetFiles(args[0], "*.asb"))
                {
                    RUNTest(s);
                }
            }
            else
            {
                RUNTest(args[0]);
            }
            /*if (args[1] == "true" || args[1] == "True" || args[1] == "1")
                RUNTest(args[0], true);
            else
            {
                RUNTest(args[0]);
            }*/
        }
        private static void RUNTest(string inFile, bool dumpHex = false)
        {
            diagDataList = ASBTools.getDialogueText(inFile);
            Console.WriteLine("Parsing " + inFile);
            
            if (!dumpHex)
            {
                List<ASBTools.ScriptData> ops = AKB148GASBLib.ASBTools.getScript(inFile);
                using (BinaryWriter b = new BinaryWriter(File.Open("Dump/" + Path.GetFileName(inFile) + ".txt", FileMode.Create)))
                {

                    foreach (ASBTools.ScriptData opc in ops)
                    {
                        b.Write(Encoding.UTF8.GetBytes(opcodeMeaning(opc, inFile)));
                        b.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                    }
                }
            }
            else
            {
                byte[] data = ASBTools.getScriptRaw(inFile);
                using (BinaryWriter b = new BinaryWriter(File.Open(Path.GetFileName(inFile) + ".HEX.txt", FileMode.Create)))
                {

                        foreach (byte by in data)
                        {
                            b.Write(Encoding.UTF8.GetBytes(by.ToString("X2") + " "));
                        }
                        b.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
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
                        return "setSpeakerAsMemberWithVoice(" + opc.paramList[0].ToString("X4") + ", " + Members.getMemberName(opc.paramList[1]) + ", " + checkForText(opc.paramList[2]) + ")";
                    }
                    else
                    {
                        return "setSpeakerAsPlayer(" + opc.paramList[0].ToString("X4") + "," + Members.getMemberName(opc.paramList[1]) + ")";
                    }

                case "0427":
                    return "SetLocationName(" + checkForText(opc.paramList[0]) + ")";

                case "0327":
                    return "SetBGImage(" + opc.paramList[0].ToString() + ")";

                case "0029":
                    return "setDialogue(" + checkForText(opc.paramList[0]) + ", " + opc.paramList[1].ToString("X4") + ")";

                case "1A27":
                    return "setMemberWithUKN(" + Members.getMemberName(opc.paramList[0]) + ", " + opc.paramList[1].ToString("X4") + ")";

                case "1C27":
                    return "setSpeakerWithImage(" + Members.getMemberName(opc.paramList[0]) + ", " + opc.paramList[1].ToString() + ")";

                case "0229":
                    if (opc.paramNum == 3)
                    {
                        return "setChoice(" + checkForText(opc.paramList[0]) + ", " + checkForText(opc.paramList[1]) + ", " + checkForText(opc.paramList[2]) + ")";
                    }
                    else
                    {
                        return "setChoice(" + checkForText(opc.paramList[0]) + ", " + checkForText(opc.paramList[1]) + ")";
                    }
                case "3227":
                    return "Zoom??(" + Members.getMemberName(opc.paramList[0]) + ", " + opc.paramList[1] + ", " + opc.paramList[2] + ", " + opc.paramList[3] + ", " + opc.paramList[4] + ")";

                case "1E27":
                    return "setMember??(" + Members.getMemberName(opc.paramList[0]) + ")";

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
                        return "playSE(" + checkForText(opc.paramList[0]) + ") Flag:" + opc.UKNFlag.ToString("X2");
                    }
                    else
                    {
                        return "playSE() Flag:" + opc.UKNFlag.ToString("X2");
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

        private static string formatScript2(ASBTools.ScriptData opc, string infile)
        {
            StringBuilder output = new StringBuilder();
            if (opc.OPCode != 0x00)
            {
                output.Append("OPCode: " + opc.OPCode.ToString("X4"));
                output.Append(" Number of Params: " + opc.paramNum.ToString("X2"));
                output.Append(" UKN1: " + opc.UKN1.ToString("X2"));
                output.Append(" UKN2: " + opc.UKN2.ToString("X2"));
                output.Append(" Flag?: " + opc.UKNFlag.ToString("X2"));
                output.Append(" Params: {");
                if (opc.paramList.Count != 0)
                {
                    for (int i = 0; i < opc.paramList.Count; i++)
                    {
                        output.Append(opc.paramList[i].ToString("X4"));
                        if ((i + 1) != opc.paramNum)
                        {
                            output.Append(" , ");
                        }
                    }
                }
                output.Append(" }");
            }
            else
            {
                output.Append("Special :");
                foreach (byte b in opc.RawBytes)
                    output.Append(" " + b.ToString("X2"));
                return output.ToString();
            }
            return output.ToString();
        }

        private static string formatScript(ASBTools.ScriptData opc, string infile)
        {
            StringBuilder output = new StringBuilder();

            switch (opc.OPCode.ToString("X4"))
            {
                case "0000":
                    output.Append("Special :");
                    foreach (byte b in opc.RawBytes)
                        output.Append(" " + b.ToString("X2"));
                    return output.ToString();

                case "4B29":
                    if (opc.paramNum == 3)
                    {
                        output.Append("setSpeakerAsMemberWithVoice(");
                    }
                    else
                    {
                        output.Append("setSpeakerAsPlayer(");
                    }
                    break;

                case "0427":
                    output.Append("SetLocName(");
                    break;

                case "0327":
                    output.Append("SetBGImage(");
                    break;

                case "0029":
                    output.Append("setDialogue(");
                    break;

                case "1A27":
                    output.Append("setMemberWithUKN(");
                    break;

                case "1C27":
                    output.Append("setSpeakerWithImage(");
                    break;

                case "0229":
                    output.Append("setChoice(");
                    break;

                case "2927":
                    if (opc.paramList.Count != 0)
                    {
                        output.Append("CHOICE_UNK(");
                    }
                    else
                    {
                        output.Append("CHOICE_UNK(");
                    }
                    break;

                case "2A29":
                    if (opc.paramList.Count != 0)
                    {
                        output.Append("playSE(");
                    }
                    else
                    {
                        output.Append("playSE(");
                    }
                    break;

                default:
                    output.Append("UKN" + opc.OPCode.ToString("X4") + "(");
                    break;
            }
            if (opc.paramNum != 0 && opc.paramList.Count != 0)
            {
                for (int i = 0; i < opc.paramNum; i++)
                {
                    output.Append(checkForText(opc.paramList[i]));
                    if ((i + 1) != opc.paramNum)
                    {
                        output.Append(" , ");
                    }
                }
            }
            output.Append(")");
            if (opc.UKNFlag != 0x00)
                output.Append(" Flag?: " + opc.UKNFlag.ToString("X2"));
            return output.ToString();
        }

        static string checkForText(long offset)
        {
            foreach(ASBTools.DialougeData ddata in diagDataList)
            {
                if (offset == ddata.Offset)
                    return "\""+ddata.Text+"\"";
            }

            return offset.ToString("X4");
        }
    }
}
