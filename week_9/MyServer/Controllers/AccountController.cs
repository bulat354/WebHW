using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MyServer.Attributes;
using MyServer.Results;

namespace MyServer.Controllers
{
    [ApiController("accounts")]
    public class AccountController : ControllerBase
    {
        [HttpPost("login")]
        public IResult Login(string email, string password, string remember)
        {
            Console.WriteLine($"{email} + {password}");

            var result = Account.CheckAccount(new Account() { Email = email, Password = password });
            if (result != null)
            {
                var cookie = new Cookie("SessionId", $"{{IsAuthorize: true, Id: {result.Id}}}", "/");
                if (remember == "on")
                    cookie.Expires = DateTime.Now + TimeSpan.FromDays(1);
                _response.SetCookie(cookie);
                return new ObjectResult<bool>(true);
            }

            return new EmptyResult();
        }
    }
}