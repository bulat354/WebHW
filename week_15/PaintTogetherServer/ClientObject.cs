using DPTPLibrary;
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
        protected internal Player userName { get; protected set; }

        protected internal DPTPClient client;
        ServerObject server;

        PlayerPackager playerPackager = new PlayerPackager();
        PixelPackager pixelPackager = new PixelPackager();

        public ClientObject(DPTPClient tcpClient, ServerObject serverObject)
        {
            client = tcpClient;
            server = serverObject;
        }

        public async Task ProcessAsync()
        {
            try
            {
                var packet = await client.ReceivePacket();
                userName = playerPackager.FromPacket(packet);                
                
                string message = $"{userName} присоединился";

                await server.BroadcastMessageAsync(packet, Id);
                Console.WriteLine(message);

                await server.SendUsers(Id);
                await server.SendPoints(Id);

                while (true)
                {
                    try
                    {
                        packet = await client.ReceivePacket();
                        var pixel = pixelPackager.FromPacket(packet);

                        Console.WriteLine($"{userName}: {pixel}");

                        await server.BroadcastMessageAsync(packet, Id);

                        server.AddPoint(pixel);
                    }
                    catch
                    {
                        message = $"{userName} отсоединился";
                        
                        Console.WriteLine(message);
                        userName.IsJoined = false;
                        await server.BroadcastMessageAsync(playerPackager.ToPacket(userName), Id);

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
            client.Close();
        }
    }
}
