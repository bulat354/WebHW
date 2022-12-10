using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProtocol;
using MyProtocol.Attributes;

namespace PaintTogetherLibrary
{
    public class Player
    {
        public string Name { get; set; }
        public bool IsJoined { get; set; }
        
        public Player() { }

        public Player(string name, bool isJoined)
        {
            Name = name;
            IsJoined = isJoined;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class PlayerPackager : IPackager<Player>
    {
        public Player FromPacket(DPTPPacket packet)
        {
            if (packet == null) 
                throw new ArgumentNullException();

            if (packet.PacketType != 1 && packet.PacketSubtype > 1)
                throw new ArgumentException("Incorrect packet");

            return new Player()
            {
                IsJoined = packet.PacketSubtype == 1,
                Name = Encoding.UTF8.GetString(packet.GetValueRaw(0))
            };
        }

        public DPTPPacket ToPacket(Player obj)
        {
            var packet = DPTPPacket.Create(1, obj.IsJoined ? (byte)1 : (byte)0);
            packet.SetValueRaw(0, Encoding.UTF8.GetBytes(obj.Name));
            return packet;
        }

        public DPTPPacket ToPacket(object obj)
        {
            if (obj is Player player)
                return ToPacket(player);

            throw new ArgumentException("obj is not Player");
        }

        object IPackager.FromPacket(DPTPPacket packet)
        {
            return FromPacket(packet);
        }
    }
}
