using MyProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintTogetherLibrary
{
    public static class PackagerBuilder
    {
        public static IPackager GetPackager(DPTPPacket packet)
        {
            var type = packet.PacketType;
            var subtype = packet.PacketSubtype;

            if (type == 0 && subtype == 0)
                return new PixelPackager();
            else if (type == 1 && subtype < 2)
                return new PlayerPackager();

            throw new ArgumentException("Unknown packet type and subtype");
        }
    }
}
