using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintTogetherLibrary
{
    public class PaintPointMessage : BaseMessage
    {
        public Point Location { get; set; }
        public Color Color { get; set; }

        public override BaseMessage FromString(string message)
        {
            var split = message.Split(' ');
            if (split.Length != 6 || split[0] != "paint")
                throw new ArgumentException();

            Location = new Point(int.Parse(split[1]), int.Parse(split[2]));
            Color = Color.FromArgb(int.Parse(split[3]), int.Parse(split[4]), int.Parse(split[5]));
            return this;
        }

        public override string ToString()
        {
            return $"paint {Location.X} {Location.Y} {Color.R} {Color.G} {Color.B}";
        }
    }
}
