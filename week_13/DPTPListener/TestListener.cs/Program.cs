using DPTPLibrary;
using MyProtocol;

namespace TestListener.cs
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var listener = new DPTPListener(System.Net.IPAddress.Any, 8888);
            listener.Start();

            var i = 0;
            while (true)
            {
                var task = listener.AcceptClientAsync();
                task.Wait();
                var client = task.Result;
                Console.WriteLine($"New client: {i}");

                var j = i;
                Task.Run(() => DoTask(client, j));
                i++;
            }
        }

        static async void DoTask(DPTPClient client, int number)
        {
            while (true)
            {
                var pack = await client.ReceivePacket();
                if (pack != null)
                    Console.WriteLine($"{number}: {pack.PacketType} {pack.PacketSubtype}");
            }
        }
    }
}