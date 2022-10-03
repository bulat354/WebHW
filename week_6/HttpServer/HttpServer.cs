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
                Debug.ListenerStartedMsg();
            }

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
                Debug.RequestReceivedMsg(request.Url.ToString());

                var statusCode = 200;
                try
                {
                    var path = ServerPath.GetPath(request.RawUrl, _config);
                    var buffer = FileManager.ReadFile(path);

                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.OutputStream.Close();
                    response.ContentType = FileManager.GetType(Path.GetExtension(path));
                }
                catch (DirectoryNotFoundException e)
                {
                    statusCode = 404;
                    Debug.DirectoryNotFoundMsg();
                }
                catch (FileNotFoundException e)
                {
                    statusCode = 404;
                    Debug.FileNotFoundMsg();
                }
                catch (ArgumentException e)
                {
                    statusCode = 400;
                    Debug.InvalidPathMsg();
                }
                catch (Exception e)
                {
                    statusCode = 500;
                    Debug.UnknownErrorMsg(e);
                }
                response.StatusCode = statusCode;
                response.Close();
                Debug.ResponseSendedMsg(statusCode);
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
        }

        private void StopQuietly()
        {
            IsWorking = false;
            _listener.Stop();
            _listenerThread.Join();
            Debug.ListenerStoppedMsg();
        }

        public void Restart()
        {
            if (IsWorking)
            {
                StopQuietly();
                StartQuietly();
            }
            else
            {
                StartQuietly();
            }
            Debug.RestartMsg();
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
