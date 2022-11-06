using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Results
{
    public class InternalErrorResult : ErrorResult
    {
        public InternalErrorResult(string message) 
            : base(HttpStatusCode.InternalServerError, message) { }

        public InternalErrorResult()
            : base(HttpStatusCode.InternalServerError, "Unknown Server Error") { }
    }
}
