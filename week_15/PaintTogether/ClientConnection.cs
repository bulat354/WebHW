using MyProtocol;
using PaintTogetherLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DPTPLibrary;

namespace PaintTogether
{
    internal class ClientConnection
    {
        private string userName;
        private DPTPClient client;

        public Form1 Form { get; set; }

        public async void ListenAsync(string userName)
        {
            var host = "127.0.0.1";
            var port = 8888;

            this.userName = userName;

            try
            {
                client = new DPTPClient(host, port);

                Task.Run(ReceiveBytesAsync);
                await SendBytesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            client.Close();
        }

        private async Task SendBytesAsync()
        {
            var packet = new PlayerPackager().ToPacket(new Player(userName, true));
            await client.SendPacket(packet);

            while (true)
            {
                packet = GetPacket();
                if (packet != null)
                    await client.SendPacket(packet);
            }
        }

        private DPTPPacket? GetPacket()
        {
            Form.MessageReady.WaitOne();
            var list = Form.PointsToSend;
            var packager = new PixelPackager();
            lock (list)
            {
                if (list.Count > 0)
                {
                    var node = list.First;
                    list.RemoveFirst();
                    if (node != null)
                        return packager.ToPacket(node.Value);
                }
                else
                {
                    Form.MessageReady.Reset();
                }
            }
            return null;
        }

        private async Task ReceiveBytesAsync()
        {
            while (true)
            {
                try
                {
                    var packet = await client.ReceivePacket();

                    if (packet == null)
                        continue;
                    ProcessPacket(packet);
                }
                catch
                {
                    break;
                }
            }
        }

        private void ProcessPacket(DPTPPacket packet)
        {
            var packager = PackagerBuilder.GetPackager(packet);
            var result = packager.FromPacket(packet);
            Action? action = null;

            if (result is Player player)
            {
                if (player.IsJoined)
                    action = () => Form.AddPlayer(player.Name);
                else
                    action = () => Form.RemovePlayer(player.Name);
            }
            else if (result is Pixel pixel)
            {
                action = () => Form.PaintPoint(pixel);
            }

            if (action != null)
                Form.Invoke(action);
        }
    }
}
