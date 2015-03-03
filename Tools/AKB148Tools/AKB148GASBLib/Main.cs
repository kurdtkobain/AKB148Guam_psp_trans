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
        private static object fLock = new Object();
        private static int threads = 4;
        private static ParallelOptions pOps = new ParallelOptions();

        // Summary:
        //     Gets a List<dialog> from asb file.
        public static List<dialog> getDialogList(string inFile, bool format = false, bool eventOnly = false)
        {
            pOps.MaxDegreeOfParallelism = threads;
            List<dialog> dlist = new List<dialog>();
            EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Little, File.Open(inFile, FileMode.Open, FileAccess.Read));
            reader.BaseStream.Position = 52;
            int script_offset = reader.ReadInt32();
            int script_size = reader.ReadInt32();
            //int script_end = reader.ReadInt32();
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
            EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Little, File.Open(inFile, FileMode.Open, FileAccess.Read));
            reader.BaseStream.Position = 52;
            int script_offset = reader.ReadInt32();
            int script_size = reader.ReadInt32();
            //int script_end = reader.ReadInt32();
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