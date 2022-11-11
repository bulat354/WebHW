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
        [HttpPost("login")]
        public IResult Login(string email, string password, string remember)
        {
            var result = Account.CheckAccount(new Account() { Email = email, Password = password });
            if (result != null)
            {
                var session = SessionManager.Instance.CreateSession(result.Id, result.Email);
                _response.AddHeader("Set-Cookie", $"SessionId={session.Id}; Path=/; Max-Age=120");
                return new ObjectResult<bool>(true);
            }

            return new EmptyResult();
        }

        [HttpGet("all")]
        public IResult GetAccounts()
        {
            if (TryGetCurrentAccountId(out var result))
            {
                return (ObjectResult<IEnumerable<Account>>)Account.GetAll();
            }

            return ErrorResult.Unauthorized();
        }

        [HttpGet("current")]
        public IResult GetAccountById()
        {
            if (TryGetCurrentAccountId(out var result))
            {
                return (ObjectResult<Account>)Account.GetAccountById(result);
            }

            return ErrorResult.Unauthorized();
        }

        private bool TryGetCurrentAccountId(out int result)
        {
            var manager = SessionManager.Instance;
            var cookie = _request.Cookies["SessionId"];
            if (cookie != null)
            {
                var guid = cookie.Value;
                if (manager.CheckSession(guid))
                {
                    var session = manager.GetSession(guid);
                    result = session.AccountId;
                    return true;
                }
            }

            result = -1;
            return false;
        }
    }
}