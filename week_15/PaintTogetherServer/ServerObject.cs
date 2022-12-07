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
        TcpListener tcpListener = new TcpListener(IPAddress.Any, 8888);
        List<ClientObject> clients = new List<ClientObject>();

        Dictionary<(int, int), PaintPointMessage> dict = 
            new Dictionary<(int, int), PaintPointMessage>();

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
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
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
            tcpListener.Stop();
        }

        protected internal async Task BroadcastMessageAsync(BaseMessage message, string id)
        {
            foreach (var client in clients)
            {
                if (client.Id != id)
                {
                    await client.Writer.WriteLineAsync(message.ToString());
                    await client.Writer.FlushAsync();
                }
            }
        }

        protected internal void AddPoint(PaintPointMessage message)
        {
            dict[(message.Location.X, message.Location.Y)] = message;
        }

        protected internal async Task SendUsers(string id)
        {
            var client = clients.FirstOrDefault(x => x.Id == id);

            if (client != null)
            {
                foreach (var c in clients)
                {
                    var message = new PlayerJoinMessage()
                    {
                        Name = c.userName
                    };
                    await client.Writer.WriteLineAsync(message.ToString());
                    await client.Writer.FlushAsync();
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
                    await client.Writer.WriteLineAsync(point.Value.ToString());
                    await client.Writer.FlushAsync();
                }
            }
        }
    }
}
