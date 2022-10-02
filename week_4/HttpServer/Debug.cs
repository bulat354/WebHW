using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer
{
    public static class Debug
    {
        public static void ProjectDirIsInvalidMsg()
        {
            ShowMessage("ERROR: Проект не находится в папке '.../MyServer/'.");
        }

        public static void HtmlNotFoundMsg(string path)
        {
            ShowMessage($"ERROR: Файл '{path}' не найден.");
        }

        public static void ListenerIsListeningMsg()
        {
            ShowMessage("Ожидание запроса...");
        }

        public static void ListenerCreatedMsg()
        {
            ShowMessage("Сервер создан.");
        }

        public static void ListenerStartedMsg()
        {
            ShowMessage("Сервер запущен.");
        }

        public static void ListenerStoppedMsg()
        {
            ShowMessage("Сервер остановлен.");
        }

        public static void RequestReceivedMsg(string request)
        {
            ShowMessage($"Запрос '{request}' принят.");
        }

        public static void ResponseSendedMsg()
        {
            ShowMessage("Ответ отправлен.");
        }

        public static void RestartMsg()
        {
            ShowMessage("Сервер перезапускается...");
        }

        public static void AlreadyStoppedMsg()
        {
            ShowMessage("Сервер еще не запущен.");
        }

        public static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        public static void StartDebugging(string url)
        {
            var server = new HttpServer(url);

            var hint = @"
Переместите папку source в папку с exe файлом.

Имеются следующие команды:
- start - запуск/перезапуск сервера;
- stop - остановка сервера;
- exit - выход;
";
            Console.WriteLine(hint);

            while (true)
            {
                var cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "start":
                        server.Start();
                        break;
                    case "stop":
                        server.Stop();
                        break;
                    case "exit":
                        Console.WriteLine("Нажмите на любую кнопку, чтобы выйти");
                        Console.ReadKey();
                        return;

                    default:
                        Console.WriteLine(hint);
                        break;
                }
            }
        }
    }
}
