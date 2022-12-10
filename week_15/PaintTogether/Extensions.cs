using PaintTogetherLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintTogether
{
    public static class Extensions
    {
        public static bool IsInBounds(this PictureBox picture, Point point)
        {
            return point.X >= 0 && point.Y >= 0 && point.X < picture.Width && point.Y < picture.Height;
        }

        public static PointF GetScale(this PictureBox picture)
        {
            var point = new PointF();
            var image = picture.Image;
            point.X = picture.Width / (float)image.Width;
            point.Y = picture.Height / (float)image.Height;
            return point;
        }

        public static Point Round(this PointF point)
        {
            var x = (int)Math.Round(point.X);
            var y = (int)Math.Round(point.Y);
            return new Point(x, y);
        }

        public static Pixel ToMessage(this Point point, Color color)
        {
            return new Pixel()
            {
                Color = color,
                Location = point
            };
        }

        public static Color GetPixel(this Bitmap bitmap, Point location)
        {
            return bitmap.GetPixel(location.X, location.Y);
        }

        public static void SetPixel(this Bitmap bitmap, Point location, Color color)
        {
            bitmap.SetPixel(location.X, location.Y, color);
        }

        public static void AddLastRange<T>(this LinkedList<T> list, IEnumerable<T> values)
        {
            foreach (var val in values)
                list.AddLast(val);
        }
    }
}
