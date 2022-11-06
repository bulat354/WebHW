using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Results
{
    public class NotFoundResult : ErrorResult
    {
        public NotFoundResult() 
            : this("Page not found") { }

        public NotFoundResult(string message)
            : base(HttpStatusCode.NotFound, message) { }
    }
}
