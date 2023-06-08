using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace _2dcDump
{
    internal class Program
    {
        public static ColorPalette GetPalette(EndianBinaryReader br, System.Drawing.Image image)
        {
            ColorPalette colors = image.Palette;
            for (int i = 0; i < 256; i++)
            {
                byte b = br.ReadByte();
                byte g = br.ReadByte();
                byte r = br.ReadByte();
                byte a = br.ReadByte();

                colors.Entries[i] = Color.FromArgb(a, r, g, b);
            }
            return colors;
        }

        public static int GetImageWidth(EndianBinaryReader br)
        {
            int ret;

            br.BaseStream.Position = 0x14c;
            ret = br.ReadUInt16();
            return ret;
        }

        public static string assString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        static string[] blacklist = new string[]
        {
            "15_00-01-01",
            "15_00-01-03",
            "15_00-02-00",
            "15_00-02-01",
            "15_00-02-02",
            "15_00-02-03",
            "15_00-02-05",
            "15_00-02-06",
            "15_00-03-00",
            "15_00-03-01",
            "15_00-03-02",
            "15_00-03-03",
            "15_00-03-04",
            "15_00-04-00",
            "15_00-04-01",
            "15_00-04-02",
            "15_00-04-03",
            "15_00-04-04",
            "15_00-05-00",
            "15_00-05-01",
            "15_00-05-02",
            "15_01-01-00",
            "15_01-01-01",
            "15_01-01-02",
            "15_01-01-03",
            "15_01-01-04",
            "15_01-02-00",
            "15_01-02-01",
            "15_01-02-02",
            "15_01-02-03",
            "15_01-02-04",
            "15_01-03-00",
            "15_01-03-01",
            "15_01-03-02",
            "15_01-03-03",
            "15_01-04-00",
            "15_01-04-01",
            "15_01-04-02",
            "15_01-04-03",
            "15_01-04-04",
            "15_01-05-01",
            "15_01-05-02",
            "15_01-06-00",
            "15_01-06-01",
            "15_01-06-03",
            "15_01-06-05",
            "15_01-06-06",
            "15_01-06-07",
            "15_01-06-08",
            "15_01-06-10",
            "15_01-06-11",
            "15_02-01-02",
            "15_02-01-03",
            "15_02-01-04",
            "15_02-02-01",
            "15_02-02-02",
            "15_02-02-04",
            "15_02-03-00",
            "15_02-03-02",
            "15_02-03-03",
            "15_02-03-04",
            "15_02-04-00",
            "15_02-04-02",
            "15_02-04-03",
            "15_02-04-04",
            "15_02-05-03",
            "15_02-06-00",
            "15_02-06-01",
            "15_02-06-04",
            "15_02-06-06",
            "15_02-06-08",
            "15_02-06-12",
            "15_02-06-13",
            "15_02-06-14",
            "15_03-01-00",
            "15_03-01-02",
            "15_03-02-00",
            "15_03-02-01",
            "15_03-02-02",
            "15_03-02-03",
            "15_03-02-05",
            "15_03-02-06",
            "15_03-03-01",
            "15_03-03-02",
            "15_03-03-04",
            "15_03-04-00",
            "15_03-04-01",
            "15_03-04-03",
            "15_03-05-00",
            "15_03-05-01",
            "15_03-06-00",
            "15_03-06-02",
            "15_03-06-03",
            "15_03-06-04",
            "15_03-06-05",
            "15_03-06-06",
            "15_03-06-07",
            "15_03-06-08",
            "15_03-06-09"


        };

        public static bool ONLYDUMPINFO = false;

        static void Main(string[] args)
        {
            string dir = args[0];
            string[] files = Directory.GetFiles(dir, "*.2dc");
            if (!Directory.Exists("2dc"))
            {
                Directory.CreateDirectory("2dc");
            }
            CH2D.CH2DPLAN plan;
            foreach (string file in files)
            {
                Console.WriteLine("Processing file: " + Path.GetFileNameWithoutExtension(file));
                int imgnum = 0;
                if (File.Exists("2dc/" + Path.GetFileNameWithoutExtension(file) + "/" + imgnum + ".png") || blacklist.Contains(Path.GetFileNameWithoutExtension(file)) || Path.GetFileNameWithoutExtension(file).StartsWith("tc") || Path.GetFileNameWithoutExtension(file).StartsWith("krm"))
                {
                    continue;
                }
                if (!Directory.Exists(Path.GetFileNameWithoutExtension(file)))
                {
                    Directory.CreateDirectory("2dc/" + Path.GetFileNameWithoutExtension(file));
                }
                long pos;
                using (EndianBinaryReader br = new EndianBinaryReader(EndianBitConverter.Big, new FileStream(file, FileMode.Open)))
                {
                    CH2D.ENDIBIGE = br.ReadBytes(0x10);
                    CH2D.CH2DHEAD head = new CH2D.CH2DHEAD(br);
                    if (Encoding.ASCII.GetString(head.tag) != "CH2DHEAD")
                    {
                        throw new Exception("WRONG TAG: CH2DHEAD");
                    }
                    CH2D.CH2DHDET hdet = new CH2D.CH2DHDET(br);
                    if (Encoding.ASCII.GetString(hdet.tag) != "CH2DHDET")
                    {
                        throw new Exception("WRONG TAG: CH2DHDET");
                    }
                    CH2D.CH2DCHAR chara = new CH2D.CH2DCHAR(br);
                    if (Encoding.ASCII.GetString(chara.tag) != "CH2DCHAR")
                    {
                        throw new Exception("WRONG TAG: CH2DCHAR");
                    }
                    CH2D.CH2DPOSE pose = new CH2D.CH2DPOSE(br);
                    if (Encoding.ASCII.GetString(pose.tag) != "CH2DPOSE")
                    {
                        throw new Exception("WRONG TAG:CH2DPOSE");
                    }
                    plan = new CH2D.CH2DPLAN(br);
                    if (Encoding.ASCII.GetString(plan.tag) != "CH2DPLAN")
                    {
                        throw new Exception("WRONG TAG: CH2DPLAN");

                    }
                    long b4texi = br.BaseStream.Position;
                    if (ONLYDUMPINFO)
                    {
                        if (File.Exists("2dc/" + Path.GetFileNameWithoutExtension(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".nfo"))
                        {
                            File.Delete("2dc/" + Path.GetFileNameWithoutExtension(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".nfo");
                        }
                        using (StreamWriter fs = File.AppendText("2dc/" + Path.GetFileNameWithoutExtension(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".nfo"))
                        {
                            fs.WriteLine(assString(CH2D.ENDIBIGE).Trim((char)0x00));
                            fs.WriteLine();
                            fs.WriteLine(head.getInfoString());
                            fs.WriteLine();
                            fs.WriteLine(hdet.getInfoString());
                            fs.WriteLine();
                            fs.WriteLine(chara.getInfoString());
                            fs.WriteLine();
                            fs.WriteLine(pose.getInfoString());
                            fs.WriteLine();
                            fs.WriteLine(plan.getInfoString());
                            fs.WriteLine();
                            fs.WriteLine("0x" + b4texi.ToString("X4") + " " + b4texi.ToString());
                        }

                    }
                    CH2D.CH2DTEXI texi = new CH2D.CH2DTEXI(br);
                    if (Encoding.ASCII.GetString(texi.tag) != "CH2DTEXI")
                    {
                        throw new Exception("WRONG TAG: CH2DTEXI");
                    }
                    if (ONLYDUMPINFO)
                    {
                        using (StreamWriter fs = File.AppendText("2dc/" + Path.GetFileNameWithoutExtension(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".nfo"))
                        {
                            fs.WriteLine();
                            fs.WriteLine(texi.getInfoString());
                        }

                    }

                    pos = br.BaseStream.Position;
                }
                CH2D.IMGDATA imgdata = new CH2D.IMGDATA(file, in pos, out pos);

                if (ONLYDUMPINFO)
                {
                    using (StreamWriter fs = File.AppendText("2dc/" + Path.GetFileNameWithoutExtension(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".nfo"))
                    {
                        fs.WriteLine();
                        fs.WriteLine(imgdata.getInfoString());
                    }

                }

                using (EndianBinaryReader br = new EndianBinaryReader(EndianBitConverter.Big, new FileStream(file, FileMode.Open)))
                {
                    if (imgdata.width == 0 || imgdata.height == 0)
                    {
                        using (StreamWriter fs = File.CreateText("error.log"))
                        {
                            fs.WriteLine(("\"" + file + "\",").Replace("\\", "\\\\"));
                        }
                        continue;
                    }
                    br.BaseStream.Position = pos;
                    Bitmap img = new Bitmap(imgdata.width, imgdata.height, PixelFormat.Format8bppIndexed);
                    ColorPalette pal = GetPalette(br, img);
                    pal.Entries[0] = Color.Transparent;
                    img.Palette = pal;
                    BitmapData data = img.LockBits(new Rectangle(0, 0, imgdata.width, imgdata.height), ImageLockMode.ReadWrite, img.PixelFormat);
                    IntPtr scan0 = data.Scan0;
                    for (int i = 0; i < img.Height; i++)
                    {
                        byte[] rowPixels = new byte[img.Width];
                        for (int j = 0; j < img.Width; j++)
                        {
                            rowPixels[j] = br.ReadByte();
                        }
                        Marshal.Copy(rowPixels, 0, scan0, rowPixels.Length);
                        scan0 = new IntPtr(scan0.ToInt64() + data.Stride);
                    }
                    img.UnlockBits(data);
                    img.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                    img.Save("2dc/" + Path.GetFileNameWithoutExtension(file) + "/" + imgnum.ToString() + ".png", ImageFormat.Png);
                    imgnum++;
                    pos = br.BaseStream.Position;
                }
                while (true)
                {
                    using (EndianBinaryReader br = new EndianBinaryReader( EndianBitConverter.Big, new FileStream(file, FileMode.Open)))
                    {
                        if (ONLYDUMPINFO)
                        {
                            using (StreamWriter fs = File.AppendText("2dc/" + Path.GetFileNameWithoutExtension(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".nfo"))
                            {
                                fs.WriteLine();
                                fs.WriteLine("0x" + pos.ToString("X4") + " " + pos.ToString());
                                fs.WriteLine();
                                fs.WriteLine("0x" + (pos + 30).ToString("X4") + " " + (pos + 30).ToString());
                            }

                        }
                        br.BaseStream.Position = pos;
                        byte[] tmp = br.ReadBytes(30); // ??????all 0x00????
                        if (tmp.Length < 30)
                            break;

                        if (tmp[29] == 0x47 || tmp[28] == 0x47)
                        {
                            break;
                        }

                        if ( Encoding.ASCII.GetString(tmp).Contains("GENEEOF"))
                        {
                            break;
                        }
                        if(br.ReadByte() == 0x47)
                        {
                            break;
                        }
                        else
                        {
                            br.BaseStream.Position--;
                        }
                        if (br.BaseStream.Length - br.BaseStream.Position < 32)
                        {
                            break;
                        }
                        CH2D.CH2DTEXI texi2 = new CH2D.CH2DTEXI(br);
                        if (Encoding.ASCII.GetString(texi2.tag) != "CH2DTEXI")
                        {
                            break;
                        }
                        pos = br.BaseStream.Position;
                        if (ONLYDUMPINFO)
                        {
                            using (StreamWriter fs = File.AppendText("2dc/" + Path.GetFileNameWithoutExtension(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".nfo"))
                            {
                                fs.WriteLine();
                                fs.WriteLine(texi2.getInfoString());
                            }

                        }

                    }
                    CH2D.IMGDATA imgdata2 = new CH2D.IMGDATA(file, in pos, out pos);

                    using (EndianBinaryReader br = new EndianBinaryReader(EndianBitConverter.Big, new FileStream(file, FileMode.Open)))
                    {
                        if (ONLYDUMPINFO)
                        {
                            using (StreamWriter fs = File.AppendText("2dc/" + Path.GetFileNameWithoutExtension(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".nfo"))
                            {
                                fs.WriteLine();
                                fs.WriteLine(imgdata2.getInfoString());
                            }

                        }
                        if (imgdata2.width == 0 || imgdata2.height == 0)
                        {
                            break;
                        }
                        br.BaseStream.Position = pos;
                        Bitmap img2 = new Bitmap(imgdata2.width, imgdata2.height, PixelFormat.Format8bppIndexed);
                        ColorPalette pal2 = GetPalette(br, img2);
                        pal2.Entries[0] = Color.Transparent;
                        img2.Palette = pal2;
                        BitmapData data2 = img2.LockBits(new Rectangle(0, 0, imgdata2.width, imgdata2.height), ImageLockMode.ReadWrite, img2.PixelFormat);
                        IntPtr scan02 = data2.Scan0;
                        for (int i = 0; i < img2.Height; i++)
                        {
                            byte[] rowPixels2 = new byte[img2.Width];
                            for (int j = 0; j < img2.Width; j++)
                            {
                                rowPixels2[j] = br.ReadByte();
                            }
                            Marshal.Copy(rowPixels2, 0, scan02, rowPixels2.Length);
                            scan02 = new IntPtr(scan02.ToInt64() + data2.Stride);
                        }
                        img2.UnlockBits(data2);
                        img2.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        img2.Save("2dc/" + Path.GetFileNameWithoutExtension(file) + "/" + imgnum.ToString() + ".png", ImageFormat.Png);
                        imgnum++;

                        if (ONLYDUMPINFO)
                        {
                            using (StreamWriter fs = File.AppendText("2dc/" + Path.GetFileNameWithoutExtension(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".nfo"))
                            {
                                fs.WriteLine();
                                fs.WriteLine(br.BaseStream.Position.ToString("X4") + " " + br.BaseStream.Position.ToString());

                            }

                        }
                        pos = br.BaseStream.Position;
                    }
                }
            }
        }
    }
}
