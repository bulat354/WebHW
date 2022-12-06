using System;

namespace PaintTogetherServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var server = new ServerObject();
            var task = Task.Run(server.ListenAsync);
            
            while (true)
            {
                var command = Console.ReadLine();
                if (command == "exit")
                {
                    server.Disconnect();
                    task.Wait();
                    break;
                }
            }

            Console.WriteLine("Нажмите на любую кнопку, чтобы выйти");
            Console.ReadKey();
        }
    }
}