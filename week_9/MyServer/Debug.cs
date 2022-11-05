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

        public static void FileNotFoundMsg()
        {
            ShowError("Файл не найден");
        }

        public static void InvalidPathMsg()
        {
            ShowError("Клиент указал неправильный путь");
        }

        public static void UnknownErrorMsg(Exception e)
        {
            ShowError($"Произошла ошибка сервера. Описание ошибки: \n\t\"{e}\"");
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
            ShowMessage("Сервер перезапускается...");
        }

        public static void AlreadyStoppedMsg()
        {
            ShowMessage("Сервер еще не запущен");
        }

        public static void ConfigFileNotFoundMsg(string path)
        {
            ShowWarning($"Файл конфигурации не найден. Создан новый по пути '{path}'");
        }

        public static void ConfigsLoadedMsg()
        {
            ShowMessage("Настройки успешно загружены");
        }

        public static void SendQueryToDBMsg()
        {
            ShowMessage("Отправка запроса на базу данных");
        }

        public static void QueryToDBSuccesMsg()
        {
            ShowMessage("Запрос успешно обработан базой данных");
        }

        public static void QueryToDBErrorMsg()
        {
            ShowWarning("Запрос базой данных не обработан");
        }

        public static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        public static void ShowError(string message)
        {
            Console.WriteLine("ERROR:\t" + message);
        }

        public static void ShowWarning(string message)
        {
            Console.WriteLine("WARNING:\t" + message);
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
