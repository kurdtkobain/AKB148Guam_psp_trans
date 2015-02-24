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

        public static List<dialog> getDialogList(string inFile, bool format = false, bool eventOnly = false)
        {
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
                Console.WriteLine("Start position is at offset {0}", tmpl);
                reader.BaseStream.Position = tmpl;
                List<byte> mahByteArray = new List<byte>();
                dialog d = new dialog();
                d.offset = reader.BaseStream.Position;
                while (reader.PeekChar() != -1)
                {
                    if (reader.PeekChar() == 0x00)
                    {
                        mahByteArray.Add(reader.ReadByte());
                        d.text = System.Text.Encoding.UTF8.GetString(mahByteArray.ToArray());
                        d.size = mahByteArray.Count;
                        dlist.Add(d);
                        mahByteArray.Clear();
                        d = new dialog();
                        d.offset = reader.BaseStream.Position;
                    }
                    else
                    {
                        mahByteArray.Add(reader.ReadByte());
                    }
                }
            }
            if (format)
            {
                List<dialog> final = new List<dialog>();
                foreach (dialog dl in dlist)
                {
                    if (dl.text.StartsWith(System.Text.Encoding.UTF8.GetString(new byte[] { 0x40 })) || dl.text.StartsWith(System.Text.Encoding.UTF8.GetString(new byte[] { 0x00 })) || dl.text.StartsWith("//") || dl.text.StartsWith("pow( x,") || dl.text.StartsWith("env") || dl.text.StartsWith("__main") || dl.text.StartsWith("main") || dl.text.StartsWith("se") || Regex.IsMatch(dl.text, "([0-9]{2}_[0-9]{2}_[0-9]{4})"))
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
                        final.Add(tmp);
                    }
                }
                dlist.Clear();
                return final;
            }
            else if (eventOnly)
            {
                List<dialog> final = new List<dialog>();
                bool write = false;
                string filename = "@" + Path.GetFileNameWithoutExtension(inFile);
                foreach (dialog dl in dlist)
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
                            final.Add(tmp);
                        }
                    }
                    else
                    {
                        if (dl.text.Contains(filename))
                        {
                            write = true;
                        }
                    }
                }
                dlist.Clear();
                return final;
            }
            else
            {
                return dlist;
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
