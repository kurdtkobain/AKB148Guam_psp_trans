using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AKB148GASBLib
{
    public class ASBTools
    {
        static object fLock = new Object();
        static int threads = 4;
        static ParallelOptions pOps = new ParallelOptions();

        // Summary:
        //     Gets a List<dialog> from asb file.
        public static List<dialog> getDialogList(string inFile, bool format = false, bool eventOnly = false)
        {
            pOps.MaxDegreeOfParallelism = threads;
            List<dialog> dlist = new List<dialog>();
            using (BinaryReader reader = new BinaryReader(File.Open(inFile, FileMode.Open, FileAccess.Read)))
            {
                reader.BaseStream.Position = 52;
                long tmpl;
                if (BitConverter.IsLittleEndian)
                {
                    tmpl = reader.ReadInt16();
                }
                else
                {
                    byte[] tmpb = new byte[4];
                    tmpb = reader.ReadBytes(2);
                    Array.Reverse(tmpb);
                    tmpl = BitConverter.ToInt16(tmpb, 0);
                }
                reader.BaseStream.Position = tmpl;
                List<byte> mahByteArray = new List<byte>();
                dialog d = new dialog();
                d.offset = reader.BaseStream.Position;
                while (reader.PeekChar() != -1)
                {
                    mahByteArray.Add(reader.ReadByte());
                    if (mahByteArray[mahByteArray.Count - 1] == 0x00)
                    {
                        d.text = System.Text.Encoding.UTF8.GetString(mahByteArray.ToArray());
                        d.size = mahByteArray.Count;
                        dlist.Add(d);
                        mahByteArray.Clear();
                        d = new dialog();
                        d.offset = reader.BaseStream.Position;
                    }
                }
            }
            if (format && !eventOnly)
            {
                List<dialog> final = new List<dialog>();
                Parallel.ForEach(dlist,pOps, dtpl =>
                {
                    if (dtpl.text.StartsWith(System.Text.Encoding.UTF8.GetString(new byte[] { 0x40 })) || dtpl.text.StartsWith(System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 })) || dtpl.text.StartsWith("//") || dtpl.text.StartsWith("pow( x,") || dtpl.text.StartsWith("env") || dtpl.text.StartsWith("__main") || dtpl.text.StartsWith("main") || dtpl.text.StartsWith("se") || Regex.IsMatch(dtpl.text, "([0-9]{2}_[0-9]{2}_[0-9]{4})"))
                    {

                    }
                    else
                    {
                        dialog tmp = new dialog();
                        tmp.offset = dtpl.offset;
                        tmp.size = dtpl.size;
                        string tmps = dtpl.text;
                        tmps = tmps.Replace(System.Text.Encoding.UTF8.GetString(new byte[] { 0x0A }), "<LINEEND>");
                        tmps = tmps.Replace(System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 }), "<END>");
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
                Parallel.ForEach(dlist,pOps, dl =>
                {

                    if (write)
                    {
                        if (dl.text.StartsWith(System.Text.Encoding.UTF8.GetString(new byte[] { 0x40 })) || dl.text.StartsWith(System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 })) || dl.text.StartsWith("//") || dl.text.StartsWith("env") || dl.text.StartsWith("pow( x,") || dl.text.StartsWith("__main") || dl.text.StartsWith("main") || dl.text.StartsWith("se") || Regex.IsMatch(dl.text, "([0-9]{2}_[0-9]{2}_[0-9]{4})"))
                        {

                        }
                        else
                        {
                            dialog tmp = new dialog();
                            tmp.offset = dl.offset;
                            tmp.size = dl.size;
                            string tmps = dl.text;
                            tmps = tmps.Replace(System.Text.Encoding.UTF8.GetString(new byte[] { 0x0A }), "<LINEEND>");
                            tmps = tmps.Replace(System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 }), "<END>");
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
            using (BinaryReader reader = new BinaryReader(File.Open(inFile, FileMode.Open, FileAccess.Read)))
            {
                reader.BaseStream.Position = 52;
                long tmpl;
                long tmpeof;
                if (BitConverter.IsLittleEndian)
                {
                    tmpl = reader.ReadInt16();
                    reader.BaseStream.Position = 60;
                    tmpeof = reader.ReadInt16();
                }
                else
                {
                    byte[] tmpb = new byte[4];
                    tmpb = reader.ReadBytes(2);
                    Array.Reverse(tmpb);
                    reader.BaseStream.Position = 60;
                    byte[] tmpb2 = new byte[4];
                    tmpb2 = reader.ReadBytes(2);
                    Array.Reverse(tmpb2);
                    tmpl = BitConverter.ToInt16(tmpb, 0);
                    tmpeof = BitConverter.ToInt16(tmpb2, 0);
                }
                reader.BaseStream.Position = tmpl;
                List<byte> mahByteArray = new List<byte>();
                dialog d = new dialog();
                d.offset = reader.BaseStream.Position;
                while (reader.BaseStream.Position != tmpeof)
                {
                    mahByteArray.Add(reader.ReadByte());
                    if (mahByteArray[mahByteArray.Count - 1] == 0x00)
                    {
                        d.text = System.Text.Encoding.UTF8.GetString(mahByteArray.ToArray());
                        d.size = mahByteArray.Count;
                        dlist.Add(d);
                        mahByteArray.Clear();
                        d = new dialog();
                        d.offset = reader.BaseStream.Position;
                    }
                }
            }
            if (format)
            {
                List<dialog> final = new List<dialog>();
                Parallel.ForEach(dlist,pOps, dl =>
                {
                    dialog tmp = new dialog();
                    tmp.offset = dl.offset;
                    tmp.size = dl.size;
                    string tmps = dl.text;
                    tmps = tmps.Replace(System.Text.Encoding.UTF8.GetString(new byte[] { 0x0A }), "<LINEEND>");
                    tmps = tmps.Replace(System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 }), "<END>");
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
                    Parallel.ForEach(dlst,pOps, d =>
                    {
                        var mahByteArray = new List<byte>();
                        if (System.Text.Encoding.UTF8.GetBytes(d.text).Length < d.size)
                        {
                            string s = d.text.Replace("\0", string.Empty);
                            int pad = d.size - System.Text.Encoding.UTF8.GetBytes(d.text).Length;
                            mahByteArray.AddRange(System.Text.Encoding.UTF8.GetBytes(s));
                            for (int i = 0; i < pad; i++)
                            {
                                mahByteArray.AddRange(System.Text.Encoding.ASCII.GetBytes(" "));
                            }
                            mahByteArray.Add(0x00);
                        }
                        else if (System.Text.Encoding.UTF8.GetBytes(d.text).Length > d.size)
                        {

                            mahByteArray.AddRange(System.Text.Encoding.UTF8.GetBytes(d.text));
                            mahByteArray.Insert(d.size, 0x00);

                        }
                        else
                        {
                            mahByteArray.AddRange(System.Text.Encoding.UTF8.GetBytes(d.text));
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
