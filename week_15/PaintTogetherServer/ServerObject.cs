using DPTPLibrary;
using MyProtocol;
using PaintTogetherLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PaintTogetherServer
{
    internal class ServerObject
    {
        DPTPListener listener = new DPTPListener(IPAddress.Any, 8888);
        List<ClientObject> clients = new List<ClientObject>();

        PlayerPackager playerPackager = new PlayerPackager();
        PixelPackager pixelPackager = new PixelPackager();

        Dictionary<(int, int), Pixel> dict = 
            new Dictionary<(int, int), Pixel>();

        protected internal void RemoveConnection(string id)
        {
            ClientObject? client = clients.FirstOrDefault(c => c.Id == id);

            if (client != null)
            {
                clients.Remove(client);
                client?.Close();
            }
        }

        protected internal async Task ListenAsync()
        {
            try
            {
                listener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    DPTPClient client = await listener.AcceptClientAsync();

                    ClientObject clientObject = new ClientObject(client, this);
                    clients.Add(clientObject);
                    Task.Run(clientObject.ProcessAsync);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        protected internal void Disconnect()
        {
            foreach (var client in clients)
            {
                client.Close();
            }
            listener.Stop();
        }

        protected internal async Task BroadcastMessageAsync(DPTPPacket packet, string id)
        {
            foreach (var client in clients)
            {
                if (client.Id != id)
                {
                    await client.client.SendPacket(packet);
                }
            }
        }

        protected internal void AddPoint(Pixel pixel)
        {
            dict[(pixel.Location.X, pixel.Location.Y)] = pixel;
        }

        protected internal async Task SendUsers(string id)
        {
            var client = clients.FirstOrDefault(x => x.Id == id);

            if (client != null)
            {
                foreach (var c in clients)
                {
                    await client.client.SendPacket(playerPackager.ToPacket(c.userName));
                }
            }
        }

        protected internal async Task SendPoints(string id)
        {
            var client = clients.FirstOrDefault(x => x.Id == id);

            if (client != null)
            {
                foreach (var point in dict)
                {
                    await client.client.SendPacket(pixelPackager.ToPacket(point.Value));
                }
            }
        }
    }
}
