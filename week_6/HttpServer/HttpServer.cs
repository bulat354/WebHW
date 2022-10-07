using System;
using System.IO;
using System.Net;

namespace MyServer
{
    public class HttpServer : IDisposable
    {
        private HttpListener _listener;
        private Thread _listenerThread;
        private Configs _config;
        private CancellationTokenSource _src = new CancellationTokenSource();

        public static string configFileName = "serverconfig.json";

        public bool IsWorking;

        public HttpServer()
        {
            _listener = new HttpListener();
            _config = Configs.DefaultConfigs;
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
                lock (_config)
                {
                    _config = Configs.Load(Path.GetFullPath(configFileName));
                }
                _listener.Prefixes.Add($"http://localhost:{_config.Port}{_config.WebRoot}/");
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
                var rawUrl = request.RawUrl.ToString();
                var url = request.Url.ToString();
                Debug.RequestReceivedMsg(url);

                if (rawUrl.StartsWith(_config.WebRoot + "/"))
                {
                    CreateResponse(request, response);
                }
                else
                {
                    response.RedirectLocation = url + '/';
                    response.StatusCode = (int)StatusCode.Redirect;
                    response.Close();
                }
                Debug.ResponseSendedMsg(response.StatusCode);
            }
        }

        private void CreateResponse(HttpListenerRequest request, HttpListenerResponse response)
        {
            int statusCode;
            try
            {
                var path = ServerPath.GetPath(request.RawUrl, _config);
                statusCode = (int)FileManager.TryReadFile(path, out var buffer);

                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
                response.ContentType = FileManager.GetType(Path.GetExtension(path));
            }
            catch (Exception e)
            {
                statusCode = (int)StatusCode.ServerError;
                Debug.UnknownErrorMsg(e);
            }
            response.StatusCode = statusCode;
            response.Close();
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

    public enum StatusCode
    {
        Succesfully = 200,
        Redirect = 303,
        InvalidUrl = 400,
        FileNotFound = 404,
        ServerError = 500
    }
}
