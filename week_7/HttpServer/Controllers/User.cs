using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace MyServer.Controllers
{
    public class User : IDisposable
    {
        public string Login { get; set; }
        public string Password { get; set; }

        private static SqlConnection connection;
        private static int usersCount;
        private static List<User> users = new List<User>();

        static User()
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AppDB;Integrated Security=True";
            connection = new SqlConnection(connectionString);
            connection.Open();

            var cmd = $"select count(login) from Users";
            var command = new SqlCommand(cmd, connection);

            usersCount = (int)command.ExecuteScalar();

            connection.Close();
        }

        public User(string login, string password)
        {
            Login = login;
            Password = password;
        }
        
        public static void Add(string login, string password)
        {
            if (login == null || password == null ||
                login.Length < 2 || login.Length > 32 ||
                password.Length > 29 || password.Length < 8)
                return;

            connection.Open();
            Debug.SendQueryToDBMsg();

            var cmd = $"insert into Users(login, password) values ('{login}', '{password}');";
            var command = new SqlCommand(cmd, connection);

            var count = command.ExecuteNonQuery();
            connection.Close();

            if (count > 0)
            {
                Debug.QueryToDBSuccesMsg();
                users.Add(new User(login, password));
                usersCount++;
            }
            else
                Debug.QueryToDBErrorMsg();
        }

        public static User[] GetUsers()
        {
            if (usersCount == users.Count)
                return users.ToArray();

            connection.Open();
            Debug.SendQueryToDBMsg();

            var cmd = $"select login, password from Users";
            var command = new SqlCommand(cmd, connection);
            var reader = command.ExecuteReader();
            users = new List<User>();

            while(reader.Read())
            {
                users.Add(new User(reader.GetString(0), reader.GetString(1)));
            }

            connection.Close();

            Debug.QueryToDBSuccesMsg();
            usersCount = users.Count;
            return users.ToArray();
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
