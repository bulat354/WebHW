using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Data.SqlClient;
using System.Text.Json;
using System.Text;
using MyServer.Attributes;
using MyServer.Controllers;
using MyServer.Templates;
using MyServer.Results;

namespace MyServer
{
    public class HttpServer : IDisposable
    {
        public static string configFileName { get; private set; } = "serverconfig.json";
        public static Configs configs { get; private set; }
        static HttpServer()
        {
            configs = Configs.Load(configFileName);
        }

        private HttpListener _listener;
        private Thread _listenerThread;
        private CancellationTokenSource _src = new CancellationTokenSource();

        public bool IsWorking;

        public HttpServer()
        {
            _listener = new HttpListener();
            configs = Configs.DefaultConfigs;
            Debug.ListenerCreatedMsg();

            Start();
        }

        public void Start()
        {
            if (IsWorking)
            {
                Debug.AlreadyStartedMsg();
                return;
            }

            StartQuietly();
        }

        private void StartQuietly()
        {
            IsWorking = true;
            _listenerThread = new Thread(() => Task.Run(Listen, _src.Token));
            _listenerThread.Start();

            ControllerManager.Init();
        }

        private async void Listen()
        {
            lock (_listener)
            {
                lock (configs)
                {
                    configs = Configs.Load(Path.GetFullPath(configFileName));
                }
                _listener.Prefixes.Add($"http://localhost:{configs.Port}/");
                _listener.Start();
            }
            Debug.ListenerStartedMsg();

            while (_listener.IsListening)
            {
                Debug.ListenerIsListeningMsg();
                HttpListenerContext context;
                try
                {
                    context = await _listener.GetContextAsync();
                }
                catch
                {
                    return;
                }
                
                var request = context.Request;
                var response = context.Response;
                IResult result;

                try
                {
                    Debug.RequestReceivedMsg(request.Url.ToString());

                    result = FileManager.MethodHandler(request, configs);
                    if ((int)result.GetStatusCode() >= 400)
                        result = ControllerManager.MethodHandler(request, configs);
                }
                catch (Exception e)
                {
                    result = new InternalErrorResult();
                }

                var buffer = result.GetResult();
                var statusCode = (int)result.GetStatusCode();
                var contentType = result.GetContentType();

                response.ContentEncoding = Encoding.UTF8;
                response.StatusCode = statusCode;
                response.ContentType = contentType;
                response.OutputStream.Write(buffer);
                response.OutputStream.Close();
                response.ContentLength64 = buffer.Length;

                Debug.ResponseSendedMsg(statusCode);

                response.Close();
            }
        }

        public void Stop()
        {
            if (!IsWorking)
            {
                Debug.AlreadyStoppedMsg();
                return;
            }

            StopQuietly();
            Debug.ListenerStoppedMsg();
        }

        private void StopQuietly()
        {
            IsWorking = false;
            _listener.Stop();
            _listenerThread.Join();
        }

        public void Restart()
        {
            Debug.RestartMsg();
            if (IsWorking)
            {
                StopQuietly();
                StartQuietly();
            }
            else
            {
                StartQuietly();
            }
        }

        public void Close()
        {
            Stop();
            _listener.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
