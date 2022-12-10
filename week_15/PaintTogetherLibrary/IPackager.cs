using MyProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintTogetherLibrary
{
    public interface IPackager<T> : IPackager
    {
        DPTPPacket ToPacket(T obj);
        T FromPacket(DPTPPacket packet);
    }

    public interface IPackager
    {
        DPTPPacket ToPacket(object obj);
        object FromPacket(DPTPPacket packet);
    }
}
