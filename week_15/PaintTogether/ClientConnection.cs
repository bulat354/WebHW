using PaintTogetherLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PaintTogether
{
    internal class ClientConnection
    {
        private string userName;

        private StreamReader? reader;
        private StreamWriter? writer;

        public Form1 Form { get; set; }

        public async void ListenAsync(string userName)
        {
            var host = "127.0.0.1";
            var port = 8888;
            using TcpClient client = new TcpClient();

            this.userName = userName;

            try
            {
                client.Connect(host, port);

                reader = new StreamReader(client.GetStream());
                writer = new StreamWriter(client.GetStream());

                if (writer is null || reader is null)
                    return;

                Task.Run(ReceiveMessageAsync);
                await SendMessageAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            writer?.Close();
            reader?.Close();
        }

        private async Task SendMessageAsync()
        {
            await writer.WriteLineAsync(userName);
            await writer.FlushAsync();

            while (true)
            {
                string? message = GetMessage();
                if (message == null)
                    continue;
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
            }
        }

        private string? GetMessage()
        {
            Form.MessageReady.WaitOne();
            var list = Form.PointsToSend;
            lock (list)
            {
                if (list.Count > 0)
                {
                    var node = list.First;
                    list.RemoveFirst();
                    if (node != null)
                        return node.Value.ToString();
                }
                else
                {
                    Form.MessageReady.Reset();
                }
            }
            return null;
        }

        private async Task ReceiveMessageAsync()
        {
            while (true)
            {
                try
                {
                    string? message = await reader.ReadLineAsync();

                    if (string.IsNullOrEmpty(message))
                        continue;
                    ProcessMessage(message);
                }
                catch
                {
                    break;
                }
            }
        }

        private void ProcessMessage(string message)
        {
            var result = BaseMessage.ParseMessage(message);
            Action action = null;

            if (result is PlayerJoinMessage join)
            {
                action = () => Form.AddPlayer(join.Name);
            }
            else if (result is PlayerDisconnectMessage disconnect)
            {
                action = () => Form.RemovePlayer(disconnect.Name);
            }
            else if (result is PaintPointMessage paint)
            {
                action = () => Form.PaintPoint(paint);
            }

            if (action != null)
            {
                Form.Invoke(action);
            }
        }
    }
}
