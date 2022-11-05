using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyServer.Attributes;

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
    }
}
