using KGySoft.Drawing;
using KGySoft.Drawing.Imaging;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JPEGA2PNG
{
    internal class Program
    {

        public class JPGA
        {
            public char[] tag = new char[4];
            public int numImg;
            public int jpgSize;
            public int ukn2;
        }
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                if (args[0] == "-f")
                {
                    convert2PNG(args[1]);
                }
                if (args[0] == "-d")
                {
                    string[] files = Directory.GetFiles(args[1], "*.jpga", SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        convert2PNG(file);
                    }
                }
            }
        }
        public static void convert2PNG(string file)
        {
            //Console.WriteLine($"Processing: {file}");
            string outFile = Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".png";
            if (File.Exists(outFile))
            {
                Console.WriteLine($"{outFile} exists skipping.....");
                return;
            }
            Bitmap jpg;
            Bitmap alphaIndexed;
            using (BinaryReader br = new BinaryReader(File.OpenRead(file)))
            {
                JPGA head = new JPGA();
                head.tag = br.ReadChars(4);
                if (!head.tag.SequenceEqual(new char[] { 'J', 'P', 'G', 'A' }))
                {
                    return;
                }
                head.numImg = br.ReadInt32();
                head.jpgSize = br.ReadInt32();
                head.ukn2 = br.ReadInt32();
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] jpgData = br.ReadBytes((int)((head.jpgSize)));
                    ms.Write(jpgData, 0, jpgData.Length);
                    jpg = new Bitmap(ms);
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    while (br.Read() == 0) { }
                    br.BaseStream.Position -= 1;
                    int size = (int)(br.BaseStream.Length - br.BaseStream.Position);
                    byte[] alphaData = br.ReadBytes(size);
                    ms.Write(alphaData, 0, alphaData.Length);
                    alphaIndexed = new Bitmap(ms);
                }
            }

            mergeImageAndAlphaMaskKGy(jpg, alphaIndexed);
            Console.WriteLine($"Writing file: {outFile}");
            jpg.Save(outFile, ImageFormat.Png);
            jpg.Dispose();
            alphaIndexed.Dispose();

        }

        public static void mergeImageAndAlphaMaskKGy(Bitmap main, Bitmap alpha)
        {
            using (IReadableBitmapData dataAlpha = alpha.GetReadableBitmapData())
            using (IReadWriteBitmapData dataMain = main.GetReadWriteBitmapData())
            {
                Parallel.For(0, dataMain.Height, y =>
                {
                    IReadWriteBitmapDataRow rowMain = dataMain[y];
                    IReadableBitmapDataRow rowAlpha = dataAlpha[y];

                    for (int x = 0; x < dataAlpha.Width; x++)
                    {
                        rowMain[x] = new Color32((byte)(255 * rowAlpha[x].GetBrightnessF()), rowMain[x].R, rowMain[x].G, rowMain[x].B);
                    }
                });
            }
        }
    }
}
