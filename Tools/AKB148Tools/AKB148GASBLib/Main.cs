using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MiscUtil.IO;
using MiscUtil.Conversion;

namespace AKB148GASBLib
{
    public class ASBTools
    {
        public class Header
        {
            public int ZERO; //always 0x00000000
            public string fileName;
            public byte ZERO2; //0x00
            public byte[] ZEROS; //always 0's
            public int UKN1; //always 0x44000000
            public int UKN2; //always 0x99000000
            public int nOffset; //always 0x380C0000
            public int nSize;
            public int sOffset;
            public int sSize;
            public int sEOF;
            public int UKN3; //always 0x09000000
            public long STARTOFF;

        }
        public class PointerTOC
        {
            public int sOffset;
            public int UKN1;
            public int UKN2;
            public int pOffset;
            public int pSize;
        }

        public class PointerData
        {
            public int Offset;
            public int Size;
            public byte[] Data;
        }
        public class ScriptData
        {
            public long Offset;
            public int paramNum;
            public int UKN1;
            public int UKN2;
            public int UKNFlag;
            public int OPCode;
            public List<int> paramList = new List<int>();
            public List<byte> RawBytes = new List<byte>();
        }

        public class DialougeData
        {
            public long Offset;
            public string Text;
        }

        private static object fLock = new Object();
        private static int threads = 4;
        private static ParallelOptions pOps = new ParallelOptions();


        public static string getDialogFromOffset(string inFile, int offset,bool noParenth = false)
        {
            Header head = getHeader(inFile);
            int script_offset = head.sOffset;
            int script_size = head.sSize;
            EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Little, File.Open(inFile, FileMode.Open, FileAccess.Read));
            reader.BaseStream.Position = script_offset;
            MemoryStream scriptStream = new MemoryStream(reader.ReadBytes(script_size));
            reader.Close();
            scriptStream.Position = offset;
            string output;
            if (!noParenth)
                output = "\"" + ReadStringZ(scriptStream) +"\"";
            else
                output = ReadStringZ(scriptStream);
            output = output.Replace(Encoding.UTF8.GetString(new byte[] { 0x0A }), "<LINEEND>");
            output = output.Replace(Encoding.UTF8.GetString(new byte[] { 0x00 }), "");
            scriptStream.Close();
            return output;
        }

        public static List<DialougeData> getDialogueText(string inFile)
        {
            List<DialougeData> ddlist = new List<DialougeData>();
            List<dialog> dlist = getDialogListRAW(inFile);
            foreach(dialog d in dlist)
            {
                DialougeData dd = new DialougeData();
                dd.Offset = d.offset;
                dd.Text = d.text;
                dd.Text = dd.Text.Replace(Encoding.UTF8.GetString(new byte[] { 0x0A }), "<LINEEND>");
                dd.Text = dd.Text.Replace(Encoding.UTF8.GetString(new byte[] { 0x00 }), "");
                ddlist.Add(dd);
            }
            return ddlist;
        }

        // Summary:
        //     Gets a List<dialog> from asb file.
        public static List<dialog> getDialogList(string inFile, bool format = false, bool eventOnly = false)
        {
            pOps.MaxDegreeOfParallelism = threads;
            List<dialog> dlist = new List<dialog>();
            Header head = getHeader(inFile);
            int script_offset = head.sOffset;
            int script_size = head.sSize;
            EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Little, File.Open(inFile, FileMode.Open, FileAccess.Read));
            reader.BaseStream.Position = script_offset;
            MemoryStream scriptStream = new MemoryStream(reader.ReadBytes(script_size));
            reader.Close();
            while (scriptStream.Position != scriptStream.Length)
            {
                dialog d = new dialog();
                d.offset = scriptStream.Position + script_offset;
                d.text = ReadStringZ(scriptStream);
                d.size = Encoding.UTF8.GetBytes(d.text).Length;
                dlist.Add(d);
            }
            if (format && !eventOnly)
            {
                List<dialog> final = new List<dialog>();
                Parallel.ForEach(dlist, pOps, dtpl =>
                {
                    if (dtpl.text.StartsWith(Encoding.UTF8.GetString(new byte[] { 0x40 })) || dtpl.text.StartsWith(Encoding.UTF8.GetString(new byte[] { 0x00 })) || dtpl.text.StartsWith("//") || dtpl.text.StartsWith("pow( x,") || dtpl.text.StartsWith("env") || dtpl.text.StartsWith("__main") || dtpl.text.StartsWith("main") || dtpl.text.StartsWith("se") || Regex.IsMatch(dtpl.text, "([0-9]{2}_[0-9]{2}_[0-9]{4})"))
                    {
                    }
                    else
                    {
                        dialog tmp = new dialog();
                        tmp.offset = dtpl.offset;
                        tmp.size = dtpl.size;
                        string tmps = dtpl.text;
                        tmps = tmps.Replace(Encoding.UTF8.GetString(new byte[] { 0x0A }), "<LINEEND>");
                        tmps = tmps.Replace(Encoding.UTF8.GetString(new byte[] { 0x00 }), "<END>");
                        tmp.text = tmps;
                        lock (fLock)
                        {
                            final.Add(tmp);
                        }
                    }
                });
                dlist.Clear();
                return final.OrderBy(o => o.offset).ToList();
            }
            else if (eventOnly)
            {
                List<dialog> final = new List<dialog>();
                bool write = false;
                string filename = "@" + Path.GetFileNameWithoutExtension(inFile);
                Parallel.ForEach(dlist, pOps, dl =>
                {
                    if (write)
                    {
                        if (dl.text.StartsWith(Encoding.UTF8.GetString(new byte[] { 0x40 })) || dl.text.StartsWith(Encoding.UTF8.GetString(new byte[] { 0x00 })) || dl.text.StartsWith("//") || dl.text.StartsWith("env") || dl.text.StartsWith("pow( x,") || dl.text.StartsWith("__main") || dl.text.StartsWith("main") || dl.text.StartsWith("se") || Regex.IsMatch(dl.text, "([0-9]{2}_[0-9]{2}_[0-9]{4})"))
                        {
                        }
                        else
                        {
                            dialog tmp = new dialog();
                            tmp.offset = dl.offset;
                            tmp.size = dl.size;
                            string tmps = dl.text;
                            tmps = tmps.Replace(Encoding.UTF8.GetString(new byte[] { 0x0A }), "<LINEEND>");
                            tmps = tmps.Replace(Encoding.UTF8.GetString(new byte[] { 0x00 }), "<END>");
                            tmp.text = tmps;
                            lock (fLock)
                            {
                                final.Add(tmp);
                            }
                        }
                    }
                    else
                    {
                        if (dl.text.Contains(filename))
                        {
                            write = true;
                        }
                    }
                });
                dlist.Clear();
                return final.OrderBy(o => o.offset).ToList();
            }
            else
            {
                return dlist;
            }
        }

        public static List<dialog> getDialogListRAW(string inFile, bool format = false)
        {
            pOps.MaxDegreeOfParallelism = threads;
            List<dialog> dlist = new List<dialog>();
            Header head = getHeader(inFile);
            int script_offset = head.sOffset;
            int script_size = head.sSize;
            EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Little, File.Open(inFile, FileMode.Open, FileAccess.Read));
            reader.BaseStream.Position = script_offset;
            MemoryStream scriptStream = new MemoryStream(reader.ReadBytes(script_size));
            reader.Close();
            while (scriptStream.Position != scriptStream.Length)
            {
                dialog d = new dialog();
                d.offset = scriptStream.Position;// + script_offset;
                d.text = ReadStringZ(scriptStream);
                d.size = Encoding.UTF8.GetBytes(d.text).Length;
                dlist.Add(d);
            }
            if (format)
            {
                List<dialog> final = new List<dialog>();
                Parallel.ForEach(dlist, pOps, dl =>
                {
                    dialog tmp = new dialog();
                    tmp.offset = dl.offset;
                    tmp.size = dl.size;
                    string tmps = dl.text;
                    tmps = tmps.Replace(Encoding.UTF8.GetString(new byte[] { 0x0A }), "<LINEEND>");
                    tmps = tmps.Replace(Encoding.UTF8.GetString(new byte[] { 0x00 }), "<END>");
                    tmp.text = tmps;
                    lock (fLock)
                    {
                        final.Add(tmp);
                    }
                });
                dlist.Clear();
                return final.OrderBy(o => o.offset).ToList();
            }
            else
            {
                return dlist;
            }
        }

        public static bool injectDialogList(string inFile, List<dialog> dlst)
        {
            pOps.MaxDegreeOfParallelism = threads;
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(inFile, FileMode.Open)))
                {
                    Parallel.ForEach(dlst, pOps, d =>
                    {
                        var mahByteArray = new List<byte>();
                        if (Encoding.UTF8.GetBytes(d.text).Length < d.size)
                        {
                            string s = d.text.Replace("\0", string.Empty);
                            int pad = d.size - Encoding.UTF8.GetBytes(d.text).Length;
                            mahByteArray.AddRange(Encoding.UTF8.GetBytes(s));
                            for (int i = 0; i < pad; i++)
                            {
                                mahByteArray.AddRange(Encoding.ASCII.GetBytes(" "));
                            }
                            mahByteArray.Add(0x00);
                        }
                        else if (Encoding.UTF8.GetBytes(d.text).Length > d.size)
                        {
                            mahByteArray.AddRange(Encoding.UTF8.GetBytes(d.text));
                            mahByteArray.Insert(d.size, 0x00);
                        }
                        else
                        {
                            mahByteArray.AddRange(Encoding.UTF8.GetBytes(d.text));
                        }
                        lock (fLock)
                        {
                            writer.BaseStream.Position = d.offset;
                            writer.Write(mahByteArray.ToArray(), 0, d.size);
                        }
                    });
                }
                return true;
            }
            catch
            {
                throw;
                //return false;
            }
        }

        public static string ReadStringZ(Stream reader)
        {
            List<byte> bArray = new List<byte>();
            byte[] k = new byte[1];
            for (int i = 0; i < 255; i++)
            {
                reader.Read(k, 0, 1);
                if (k[0] == 0x00)
                {
                    bArray.Add(k[0]);
                    break;
                }
                bArray.Add(k[0]);
            }
            return Encoding.UTF8.GetString(bArray.ToArray());
        }

        public static List<ScriptData> getScript(string inFile)
        {
            Header head = getHeader(inFile);
            List<PointerData> pointD = getPointerData(inFile);
            Array.Reverse(pointD[pointD.Count -2].Data);
            MemoryStream pDataStream = new MemoryStream(pointD[pointD.Count - 2].Data);
            EndianBinaryReader R = new EndianBinaryReader(EndianBitConverter.Big, pDataStream);
            List<ScriptData> ops = new List<ScriptData>();
            parseScript(R, ops, pointD[pointD.Count - 2].Size);
            R.Close();
            ops.Reverse();
            return ops;

        }

        public static byte[] getScriptRaw(string inFile)
        {
            Header head = getHeader(inFile);
            List<PointerData> pointD = getPointerData(inFile);
            Array.Reverse(pointD[pointD.Count - 2].Data);
            MemoryStream pDataStream = new MemoryStream(pointD[pointD.Count - 2].Data);
            return pDataStream.ToArray();

        }

        private static void parseScript(EndianBinaryReader s, List<ScriptData> opC, int size)
        {

            ScriptData op = new ScriptData();
            s.ReadByte();
            while (s.BaseStream.Position < s.BaseStream.Length)
            {
                op.Offset = s.BaseStream.Position;
                op.paramNum = s.ReadByte();
                op.RawBytes.Add((byte)op.paramNum);
                op.UKN1 = s.ReadByte();
                op.RawBytes.Add((byte)op.UKN1);
                op.UKN2 = s.ReadByte();
                op.RawBytes.Add((byte)op.UKN2);
                if (op.UKN2 != 0x00)
                {
                    op.UKNFlag = s.ReadByte();
                    op.RawBytes.Add((byte)op.UKNFlag);
                    byte tmpByte = s.ReadByte();
                    op.RawBytes.Add(tmpByte);
                    if (tmpByte != 0x21)
                    {
                        if (tmpByte == 0x00 || tmpByte == 0x22 || tmpByte == 0x04)
                        {
                            while (true)
                            {
                                byte tmpbytets = s.ReadByte();
                                op.RawBytes.Add(tmpbytets);
                                if (tmpbytets == (byte)0x05) {
                                    byte tmpbytets2 = s.ReadByte();
                                    if (tmpbytets2 != 0x04)
                                    {
                                        s.BaseStream.Position -=1;
                                        break;
                                    }
                                    op.RawBytes.Add(tmpbytets2);
                                }
                            }
                        }
                        else
                        {
                            byte tmpbyt = s.ReadByte();
                            op.RawBytes.Add(tmpbyt);
                        }
                    }
                }
                else
                {
                    op.UKNFlag = s.ReadByte();
                    op.RawBytes.Add((byte)op.UKNFlag);
                    op.OPCode = s.ReadInt16();
                    byte[] intBytes = BitConverter.GetBytes(op.OPCode);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(intBytes);
                    op.RawBytes.AddRange(intBytes);
                    if (intBytes[3] != 0x27 && intBytes[3] != 0x29 && intBytes[3] != 0x00)
                    {
                        byte[] errby = s.ReadBytes(4);


                        Console.WriteLine("WRONGOPCODE");



                    }
                    if ((op.OPCode == 0x0000 && op.UKNFlag == 0x00) || op.UKNFlag == 0x02 || (op.UKNFlag == 0x01 && op.OPCode == 0x0000))
                    {
                        byte[] tmpByteop = s.ReadBytes(3);
                        op.RawBytes.AddRange(tmpByteop);
                    } else if (op.OPCode == 0x7301 ) {

                        byte[] tmpByteop = s.ReadBytes(5);
                        op.RawBytes.AddRange(tmpByteop);

                    } else
                    {
                        if (op.paramNum != 0)
                        {
                            for (int g = 0; g < op.paramNum; g++)
                            {
                                int tmpprams = s.ReadInt32();
                                byte[] intBytes2 = BitConverter.GetBytes(tmpprams);
                                if (BitConverter.IsLittleEndian)
                                    Array.Reverse(intBytes2);
                                op.RawBytes.AddRange(intBytes2);
                                op.paramList.Add(tmpprams);
                                byte tmpprambyte = s.ReadByte();
                                op.RawBytes.Add(tmpprambyte);
                            }
                        }
                    }
                }
                op.paramList.Reverse();
                opC.Add(op);
                op = new ScriptData();

            }
        }

        public static Header getHeader(string inFile)
        {
            Header head = new Header();
            EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Little, File.Open(inFile, FileMode.Open, FileAccess.Read));
            head.ZERO = reader.ReadInt32(); //always 0x00000000
            head.fileName = Encoding.UTF8.GetString(reader.ReadBytes(15));
            head.ZERO2 =reader.ReadByte(); //0x00
            head.ZEROS = reader.ReadBytes(16); //always 0's
            head.UKN1 = reader.ReadInt32(); //always 0x44000000
            head.UKN2 = reader.ReadInt32(); //always 0x99000000
            head.nOffset = reader.ReadInt32(); //always 0x380C0000
            head.nSize = reader.ReadInt32();
            head.sOffset = reader.ReadInt32();
            head.sSize = reader.ReadInt32();
            head.sEOF = reader.ReadInt32();
            head.UKN3 = reader.ReadInt32(); //always 0x09000000
            head.STARTOFF = reader.BaseStream.Position;
            reader.Close();
            return head;
        }

        public static List<PointerData> getPointerData(string inFile)
        {
            Header head = getHeader(inFile);
            EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Little, File.Open(inFile, FileMode.Open, FileAccess.Read));
            reader.BaseStream.Position = head.STARTOFF;
            List<PointerTOC> tst = new List<PointerTOC>();
            while (reader.BaseStream.Position < head.nOffset)
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
                pData.Offset = p.pOffset + head.nOffset;
                reader.BaseStream.Position = pData.Offset;
                pData.Size = p.pSize;
                pData.Data = reader.ReadBytes(pData.Size);
                pointD.Add(pData);
            }
            reader.Close();
            return pointD;
        }
        }

    public class dialog : INotifyPropertyChanged
    {
        public long offset { get; set; }

        public int size { get; set; }

        public string text { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

  
}