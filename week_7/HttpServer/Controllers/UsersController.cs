using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MyServer.Attributes;

namespace MyServer.Controllers
{
    [ApiController("accounts")]
    internal class UsersController
    {
        [HttpGet]
        public User[] GetUsers()
        {
            return User.GetUsers();
        }

        [HttpPost("add")]
        public void AddUser(string login, string pass) //void send code 204
        {
            User.Add(login, pass);
        }
    }
}
