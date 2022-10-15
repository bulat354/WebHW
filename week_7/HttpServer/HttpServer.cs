using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Data.SqlClient;
using System.Text.Json;
using System.Text;
using MyServer.Attributes;
using MyServer.Controllers;

namespace MyServer
{
    public class HttpServer : IDisposable
    {
        private HttpListener _listener;
        private Thread _listenerThread;
        private Configs configs;
        private CancellationTokenSource _src = new CancellationTokenSource();

        public static string configFileName = "serverconfig.json";

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

                try
                {
                    Debug.RequestReceivedMsg(request.Url.ToString());

                    var result = FileManager.MethodHandler(request, configs);
                    if (!result.IsSuccess)
                        result = ControllerManager.MethodHandler(request, configs);

                    result.ToListenerResponse(response);

                    Debug.ResponseSendedMsg(response.StatusCode);
                }
                catch (Exception e)
                {
                    HttpResponse.GetInternalErrorResponse(e).ToListenerResponse(response);
                }

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
