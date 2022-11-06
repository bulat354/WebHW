using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyServer.Attributes;
using MyServer.Results;

namespace MyServer.Controllers
{
    [ApiController("accounts")]
    public class AccountController
    {
        [HttpGet("all")]
        public List<Account> GetAll()
        {
            return Account.GetAll();
        }

        [HttpPost("login")]
        public bool Login(string email, string password)
        {
            Console.WriteLine($"{email} + {password}");
            return Account.CheckAccount(new Account() { Email = email, Password = password }) != null;
        }
    }
}