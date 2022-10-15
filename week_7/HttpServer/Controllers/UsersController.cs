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
            if (id < 0 || id >= User.Users.Length)
                return HttpResponse.GetNotFoundResponse();
            return new HttpResponse(HttpStatusCode.OK, User.Users[id]);
        }

        [HttpGet]
        public User[] GetUsers()
        {
            return User.Users;
        }

        [HttpGet]
        public User[] GetUsers(int count)
        {
            return User.Users.Take(count).ToArray();
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

        public static User[] Users { get; set; } = new[]
        {
            new User(0, "Antonio"),
            new User(1, "Montecarlo")
        };
    }
}
