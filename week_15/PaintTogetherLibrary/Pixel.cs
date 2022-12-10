using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProtocol;
using MyProtocol.Attributes;

namespace PaintTogetherLibrary
{
    public class Pixel
    {
        public Point Location { get; set; }
        public Color Color { get; set; }

        public Pixel() { }

        public Pixel(Point location, Color color)
        {
            Location = location; Color = color;
        }

        public override string ToString()
        {
            return $"Location: ({Location.X}, {Location.Y}), Color: argb({Color.A}, {Color.R}, {Color.G}, {Color.B})";
        }
    }

    public class PixelPackager : IPackager<Pixel>
    {
        public DPTPPacket ToPacket(Pixel obj)
        {
            var packet = DPTPPacket.Create(0, 0);
            packet.SetValueRaw(0, obj.Location.X.ToBytes().ToArray());
            packet.SetValueRaw(1, obj.Location.Y.ToBytes().ToArray());
            packet.SetValueRaw(2, obj.Color.ToBytes());
            return packet;
        }

        public Pixel FromPacket(DPTPPacket packet)
        {
            if (packet.PacketType != 0 && packet.PacketSubtype != 0)
                throw new ArgumentException();

            return new Pixel()
            {
                Location = new Point(
                    packet.GetValueRaw(0).ToInteger(),
                    packet.GetValueRaw(1).ToInteger()),
                Color = packet.GetValueRaw(2).ToColor()
            };
        }

        public DPTPPacket ToPacket(object obj)
        {
            if (obj is Pixel pixel)
                return ToPacket(pixel);

            throw new ArgumentException("obj is not Pixel");
        }

        object IPackager.FromPacket(DPTPPacket packet)
        {
            return FromPacket(packet);
        }
    }
}
