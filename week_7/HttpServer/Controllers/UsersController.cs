using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MyServer.Attributes;

namespace MyServer.Controllers
{
    [HttpController("accounts")]
    internal class UsersController
    {
        [HttpGet("id")]
        public HttpResponse GetUser(int id)
        {
            if (id < 0 || id >= User.Users.Count)
                return HttpResponse.GetNotFoundResponse();
            return new HttpResponse(HttpStatusCode.OK, User.Users[id]);
        }

        [HttpGet]
        public List<User> GetUsers()
        {
            return User.Users;
        }

        [HttpGet]
        public User[] GetUsers(int count)
        {
            return User.Users.Take(count).ToArray();
        }

        [HttpGet("count")]
        public int GetCount()
        {
            return User.Users.Count;
        }

        [HttpPost("add")]
        public void AddUser(int id, string name)
        {
            User.Users.Add(new User(id, name));
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public User(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static List<User> Users { get; set; } = new List<User>()
        {
            new User(0, "Antonio"),
            new User(1, "Montecarlo")
        };
    }
}
