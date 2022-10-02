using System;
using System.IO;
using System.Net;

namespace MyServer
{
    public class HttpServer
    {
        private HttpListener listener;
        private Thread thread;

        public readonly string Url;
        private string defaultPath;
        private string rootPath;

        public HttpServer(string url)
        {
            Url = url;

            rootPath = "./source/";
            defaultPath = "./source/index.html";

            listener = new HttpListener();
            listener.Prefixes.Add(Url);

            Debug.ListenerCreatedMsg();
        }

        public void Start()
        {
            if (listener.IsListening)
            {
                listener.Stop();
                Debug.RestartMsg();
            }

            listener.Start();
            Debug.ListenerStartedMsg();

            thread = new Thread(Listen);
            thread.Start();
        }

        private void Listen()
        {
            Debug.ListenerIsListeningMsg();
            HttpListenerContext context;

            try
            {
                context = listener.GetContext();
            }
            catch
            {
                return;
            }

            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            Debug.RequestReceivedMsg(request.Url.ToString());

            var buffer = CreateBytes(request);

            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            Debug.ResponseSendedMsg();

            Listen();
        }

        private string CreateFilePath(HttpListenerRequest request)
        {
            var path = request.RawUrl.Substring(8);
            if (path.Length == 0)
            {
                return defaultPath;
            }
            else
            {
                return rootPath + path;
            }
        }

        private byte[] CreateBytes(HttpListenerRequest request)
        {
            var filePath = CreateFilePath(request);
            byte[] buffer;
            if (File.Exists(filePath))
            {
                buffer = File.ReadAllBytes(filePath);
            }
            else
            {
                Debug.HtmlNotFoundMsg(filePath);
                return new byte[0];
            }
            return buffer;
        }

        public void Stop()
        {
            if (!listener.IsListening)
                Debug.AlreadyStoppedMsg();

            listener.Stop();
            Debug.ListenerStoppedMsg();
        }
    }
}
