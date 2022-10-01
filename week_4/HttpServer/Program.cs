namespace MyServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Debug.StartDebugging($"http://localhost:{args[0]}/google/");
            }
            else
            {
                Debug.StartDebugging("http://localhost:8080/google/");
            }
        }
    }
}