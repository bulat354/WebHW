using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer
{
    public static class Debug
    {
        public static void ListenerCreatedMsg()
        {
            ShowMessage("Сервер создан");
        }

        public static void AlreadyStartedMsg()
        {
            ShowMessage("Сервер уже запущен");
        }

        public static void ListenerStartedMsg()
        {
            ShowMessage("Сервер запущен");
        }

        public static void ListenerIsListeningMsg()
        {
            ShowMessage("Ожидание запроса...");
        }

        public static void DirectoryNotFoundMsg()
        {
            ShowMessage("ERROR:\tДиректория не найдена");
        }

        public static void FileNotFoundMsg()
        {
            ShowMessage("ERROR:\tФайл не найден");
        }

        public static void InvalidPathMsg()
        {
            ShowMessage("ERROR:\tКлиент указал неправильный путь");
        }

        public static void UnknownErrorMsg(Exception e)
        {
            ShowMessage($"ERROR:\tПроизошла ошибка сервера. Описание ошибки: \"{e.Message}\"");
        }

        public static void RequestReceivedMsg(string request)
        {
            ShowMessage($"Запрос '{request}' принят");
        }

        public static void ResponseSendedMsg(int responseCode)
        {
            ShowMessage($"Ответ отправлен. Код ответа: {responseCode}");
        }

        public static void ListenerStoppedMsg()
        {
            ShowMessage("Сервер остановлен");
        }

        public static void RestartMsg()
        {
            ShowMessage("Сервер перезапущен");
        }

        public static void AlreadyStoppedMsg()
        {
            ShowMessage("Сервер еще не запущен");
        }

        public static void ConfigFileNotFoundMsg(string path)
        {
            ShowMessage($"WARNING:\tФайл конфигурации не найден. Создан новый с настройками по умолчанию по пути '{path}'");
        }

        public static void ConfigsLoadedMsg()
        {
            ShowMessage("Настройки успешно загружены");
        }

        public static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        public static void StartDebugging(HttpServer server)
        {
            var hint = @"
Переместите папку source в папку с exe файлом.

Имеются следующие команды:
- start - запуск сервера;
- restart - перезапуск сервера;
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
                    case "restart":
                        server.Restart();
                        break;
                    case "exit":
                        server.Close();
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
