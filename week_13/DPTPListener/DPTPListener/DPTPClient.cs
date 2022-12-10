using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using MyProtocol;

namespace DPTPLibrary
{
    public class DPTPClient : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public DPTPClient(string hostname, int port)
        {
            _client = new TcpClient(hostname, port);
            _stream = _client.GetStream();
        }

        internal DPTPClient(TcpClient client)
        {
            _client = client;
            _stream = _client.GetStream();
        }

        public async Task SendPacket(DPTPPacket packet)
        {
            var buffer = packet.ToPacket();

            await _stream.WriteAsync(buffer, 0, buffer.Length);
            await _stream.FlushAsync();
        }

        public async Task<DPTPPacket?> ReceivePacket()
        {
            var packet = await DPTPPacket.ParseAsync(_stream);
            await _stream.FlushAsync();

            return packet;
        }

        public void Close()
        {
            _client.Close();
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
