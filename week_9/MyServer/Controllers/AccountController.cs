using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MyServer.Attributes;
using MyServer.Results;

namespace MyServer.Controllers
{
    [ApiController("accounts")]
    public class AccountController : ControllerBase
    {
        public static Dictionary<string, int> Sessions = new Dictionary<string, int>();

        [HttpPost("login")]
        public IResult Login(string email, string password, string remember)
        {
            Console.WriteLine($"{email} + {password}");

            var result = Account.CheckAccount(new Account() { Email = email, Password = password });
            if (result != null)
            {
                var guid = Guid.NewGuid().ToString();
                Sessions[guid] = result.Id;

                var toRemember = remember == "on";
                SetCookie("IsAuthorized", "true", "/", toRemember ? 24 * 60 * 60 : null);
                SetCookie("SessionId", guid, "/", toRemember ? 24 * 60 * 60 : null);
                return new ObjectResult<bool>(true);
            }

            return new EmptyResult();
        }

        [HttpGet("all")]
        public IResult GetAccounts()
        {
            if (CheckSession())
            {
                return (ObjectResult<IEnumerable<Account>>)Account.GetAll();
            }

            return ErrorResult.Unauthorized();
        }

        private bool CheckSession()
        {
            var cookie = _request.Cookies["IsAuthorized"];
            if (cookie != null && cookie.Value == "true")
            {
                cookie = _request.Cookies["SessionId"];
                if (cookie != null && Sessions.ContainsKey(cookie.Value))
                    return true;
                else
                {
                    SetCookie("SessionId", "", "/", 0);
                    SetCookie("IsAuthorized", "", "/", 0);
                }
            }
            return false;
        }

        private void SetCookie(string name, string value, string path = "/", int? seconds = null)
        {
            if (seconds == null)
                _response.AddHeader("Set-Cookie", $"{name}={value}; Path={path}");
            else
                _response.AddHeader("Set-Cookie", $"{name}={value}; Path={path}; Max-Age={seconds}");
        }
    }
}