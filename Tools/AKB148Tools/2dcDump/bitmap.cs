using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2dcDump
{
    internal class bitmap
    {
        public class test
        {
            public static Bitmap getBMPFromData(int width, int height, byte[] data)
            {

                // Create the 8bpp indexed bitmap
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);


                // To access the raw image data we need to use pointers
                // Note that you need to compile with /unsafe
                // or set "Allow unsafe code blocks" in your project's
                // configuration properties


                unsafe
                {
                    // Lock the bitmap data to a fixed position
                    // This is basically the same as your code, but since
                    // You only write data, you might as well use WriteOnly 
                    BitmapData bmData = bmp.LockBits(
                            new Rectangle(0, 0, bmp.Width, bmp.Height),
                            ImageLockMode.WriteOnly,
                            PixelFormat.Format8bppIndexed);

                    // Get a pointer to the beginning of the pixel data 
                    byte* p = (byte*)bmData.Scan0;

                    // Since a Bitmap scan line is ALWAYS multiples of
                    // 4 bytes we need to 'step over' the unused bytes
                    // at the end.
                    // Stride tells us the size of the scan line 
                    int diff = bmData.Stride - bmp.Width;

                    // Loop through your array and add each byte     
                    // For each loop increment the pointer by 1
                    // When we reach the end of a line step over
                    // the unused bytes 
                    for (int i = 0; i < data.Length; i++)
                    {
                        *p++ = data[i];
                        if (i % bmp.Width == 0 && i > 0)
                            p += diff;
                    }

                    // Release the memory
                    bmp.UnlockBits(bmData);
                    return bmp;
                }
            }
        }
    }
}
