namespace MyServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Debug.StartDebugging(new HttpServer());
        }
    }
}