using System.Data.SqlClient;
using System.Text.Json;
using System.Linq.Expressions;

using System.Collections;

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