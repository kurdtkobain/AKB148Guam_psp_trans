using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace _2dcSticher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dir = args[0];
            string[] folders = Directory.GetDirectories(dir);
            foreach (string folder in folders)
            {
                var folderName = new DirectoryInfo(folder).Name;
                Console.WriteLine("Processing file: " + folderName);
                string[] files = Directory.GetFiles(folder, "*.png");
                if(files.Length == 0)
                {
                    continue;
                }

                if (files.Length > 2)
                {
                    Bitmap tl = new Bitmap(files[1]);
                    Bitmap tr = new Bitmap(files[0]);
                    Bitmap bl = new Bitmap(files[3]);
                    Bitmap br = new Bitmap(files[2]);

                    Bitmap finalimg = new Bitmap(tl.Width + tr.Width, tl.Height + bl.Height);

                    using (Graphics g = Graphics.FromImage(finalimg))
                    {
                        g.DrawImage(tl, 0, 0);
                        g.DrawImage(tr, tl.Width, 0);
                        g.DrawImage(bl, 0, tl.Height);
                        g.DrawImage(br, bl.Width, tl.Height);
                    }
                    string savepath = folder + "\\" + folderName + ".png";
                    finalimg.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    finalimg.Save(savepath, System.Drawing.Imaging.ImageFormat.Png);
                    tl.Dispose();
                    tr.Dispose();
                    bl.Dispose();
                    br.Dispose();
                    finalimg.Dispose();
                }
                else
                {

                    Bitmap t = new Bitmap(files[0]);
                    Bitmap b = new Bitmap(files[1]);
                    Bitmap finalimg2 = new Bitmap(t.Width, t.Height + b.Height);

                    using (Graphics g = Graphics.FromImage(finalimg2))
                    {
                        g.DrawImage(t, 0, 0);
                        g.DrawImage(b, 0, t.Height);
                    }
                    string savepath2 = folder + "\\" + folderName + ".png";
                    finalimg2.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    finalimg2.Save(savepath2, System.Drawing.Imaging.ImageFormat.Png);
                    t.Dispose();
                    b.Dispose();
                    finalimg2.Dispose();
                }
                foreach(string file in files)
                {
                    File.Delete(file);
                }
            }
        }
    }
}
