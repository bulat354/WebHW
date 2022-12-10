using DPTPLibrary;
using MyProtocol;

namespace TestClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();
            var client = new DPTPClient("127.0.0.1", 8888);

            Console.ReadKey();
            client.SendPacket(DPTPPacket.Create(0, 0));
            Console.ReadKey();
            client.SendPacket(DPTPPacket.Create(0, 1));
            client.SendPacket(DPTPPacket.Create(0, 2));
            Console.ReadKey();
            client.SendPacket(DPTPPacket.Create(0, 3));
            client.SendPacket(DPTPPacket.Create(1, 0));
            client.SendPacket(DPTPPacket.Create(1, 1));
            Console.ReadKey();
            client.SendPacket(DPTPPacket.Create(1, 2));
        }
    }
}