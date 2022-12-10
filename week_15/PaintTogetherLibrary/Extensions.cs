using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintTogetherLibrary
{
    internal static class Extensions
    {
        public static IEnumerable<byte> ToBytes(this int integer)
        {
            while (integer > 0)
            {
                yield return (byte)(integer % 256);
                integer /= 256;
            }
        }

        public static int ToInteger(this byte[] bytes)
        {
            //if (bytes == null)
            //    throw new ArgumentNullException();

            var integer = 0;
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                integer = integer * 256 + bytes[i];
            }
            return integer;
        }

        public static Color ToColor(this byte[] bytes)
        {
            if (bytes.Length < 4)
                throw new ArgumentException();

            return Color.FromArgb(
                bytes[0],
                bytes[1],
                bytes[2],
                bytes[3]
                );
        }

        public static byte[] ToBytes(this Color color)
        {
            return new byte[]
            {
                color.A,
                color.R,
                color.G,
                color.B
            };
        }
    }
}
