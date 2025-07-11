using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TGASharpLib;

namespace _2dcDump
{
    internal class Program
    {
        public static string assString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        static string[] blacklist = new string[] //Files have GIM data
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
                if (blacklist.Contains(Path.GetFileNameWithoutExtension(file)) || Path.GetFileNameWithoutExtension(file).StartsWith("tc") || Path.GetFileNameWithoutExtension(file).StartsWith("krm"))
                {
                    continue;
                }
                Console.WriteLine("Processing file: " + Path.GetFileNameWithoutExtension(file));
                int imgnum = 0;
                string membernum = Path.GetFileNameWithoutExtension(file).Split('_')[0];
                string eventnum = Path.GetFileNameWithoutExtension(file).Split('_')[1].Split('-')[0];
                string posenum = Path.GetFileNameWithoutExtension(file).Split('_')[1].Split('-')[1];
                string expnum = Path.GetFileNameWithoutExtension(file).Split('_')[1].Split('-')[2];
                if (File.Exists($"2dc/{membernum}/{eventnum}/{posenum}/{expnum}" + "/" + imgnum + ".png"))
                {
                    continue;
                }
                if (!Directory.Exists(Path.GetFileNameWithoutExtension(file)))
                {
                    Directory.CreateDirectory($"2dc/{membernum}/{eventnum}/{posenum}/{expnum}");
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
                        if (File.Exists($"2dc/{membernum}/{eventnum}/{posenum}/{expnum}" + "/" + Path.GetFileNameWithoutExtension(file) + ".nfo"))
                        {
                            File.Delete($"2dc/{membernum}/{eventnum}/{posenum}/{expnum}" + "/" + Path.GetFileNameWithoutExtension(file) + ".nfo");
                        }
                        using (StreamWriter fs = File.AppendText($"2dc/{membernum}/{eventnum}/{posenum}/{expnum}" + "/" + Path.GetFileNameWithoutExtension(file) + ".nfo"))
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

                    for (int i = 0; i <= hdet.numFiles-1; i++)
                    {
                        CH2D.CH2DTEXI texi = new CH2D.CH2DTEXI(br);
                        if (Encoding.ASCII.GetString(texi.tag) != "CH2DTEXI")
                        {
                            throw new Exception("WRONG TAG: CH2DTEXI");
                        }
                        if (ONLYDUMPINFO)
                        {
                            using (StreamWriter fs = File.AppendText($"2dc/{membernum}/{eventnum}/{posenum}/{expnum}" + "/" + Path.GetFileNameWithoutExtension(file) + ".nfo"))
                            {
                                fs.WriteLine();
                                fs.WriteLine(texi.getInfoString());
                            }

                        }
                        var comp = br.ReadBytes(3);
                        if (comp[0] == 0x4D && comp[1] == 0x49 && comp[2] == 0x47) // dammit GIM
                        {
                            br.BaseStream.Position -= 3;
                            br.BaseStream.Position += texi.size2;
                            var currentpos = br.BaseStream.Position;
                            if ((currentpos + 24) < br.BaseStream.Length)
                            {
                                br.BaseStream.Position += 24;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            br.BaseStream.Position -= 3;
                            var image = (TGASharpLib.TGA.FromBytes(br.ReadBytes(texi.size2))).ToBitmap(true);
                            image.MakeTransparent();
                            image.Save($"2dc/{membernum}/{eventnum}/{posenum}/{expnum}" + "/" + i.ToString() + ".png", ImageFormat.Png);
                            var currentpos = br.BaseStream.Position;
                            if ((currentpos + 30) < br.BaseStream.Length)
                            {
                                br.ReadBytes(30);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
