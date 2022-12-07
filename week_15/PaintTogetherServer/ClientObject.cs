using PaintTogetherLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PaintTogetherServer
{
    internal class ClientObject
    {
        protected internal string Id { get; } = Guid.NewGuid().ToString();
        protected internal string userName { get; protected set; }

        protected internal StreamWriter Writer { get; }
        protected internal StreamReader Reader { get; }

        TcpClient client;
        ServerObject server;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            client = tcpClient;
            server = serverObject;

            var stream = client.GetStream();

            Reader = new StreamReader(stream);
            Writer = new StreamWriter(stream);
        }

        public async Task ProcessAsync()
        {
            try
            {
                userName = await Reader.ReadLineAsync();
                string message = $"{userName} присоединился";

                var join = new PlayerJoinMessage()
                {
                    Name = userName
                };
                await server.BroadcastMessageAsync(join, Id);
                Console.WriteLine(message);

                await server.SendUsers(Id);
                await server.SendPoints(Id);

                while (true)
                {
                    try
                    {
                        message = await Reader.ReadLineAsync();
                        if (message == null)
                            continue;

                        Console.WriteLine($"{userName}: {message}");

                        var parsed = BaseMessage.ParseMessage(message);
                        await server.BroadcastMessageAsync(parsed, Id);

                        if (parsed is PaintPointMessage point)
                            server.AddPoint(point);
                    }
                    catch
                    {
                        message = $"{userName} отсоединился";
                        
                        Console.WriteLine(message);
                        var disconnect = new PlayerDisconnectMessage()
                        {
                            Name = userName
                        };
                        await server.BroadcastMessageAsync(disconnect, Id);

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                server.RemoveConnection(Id);
            }
        }

        protected internal void Close()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }
    }
}
