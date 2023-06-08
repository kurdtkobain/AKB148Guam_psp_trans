using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAMY.Extensions
{
    public static class MiscExtensions
    {
        public static bool isNull<T>(this T obj) => obj == null;

        public static string ToHexString(this ushort[] data)
        {
            StringBuilder datastring = new StringBuilder(data.Length * 5);

            for (var i = 0; i < data.Length; i++)
            {
                datastring.AppendFormat("{0,4:x4} ", data[i]);
            }

            // remove the last space
            datastring.Remove(datastring.Length - 1, 1);

            return datastring.ToString();
        }

        public static string ToHexString(this byte[] data)
        {
            StringBuilder datastring = new StringBuilder(data.Length * 3);

            for (var i = 0; i < data.Length; i++)
            {
                datastring.AppendFormat("{0,2:x2} ", data[i]);
            }

            // remove the last space
            datastring.Remove(datastring.Length - 1, 1);

            return datastring.ToString();
        }
    }
}
