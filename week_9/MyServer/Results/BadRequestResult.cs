using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Results
{
    public class BadRequestResult : ErrorResult
    {
        public BadRequestResult(string message) 
            : base(HttpStatusCode.BadRequest, message) { }

        public BadRequestResult()
            : this("Bad request") { }
    }
}
