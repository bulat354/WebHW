using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Results
{
    public class EmptyResult : IResult
    {
        public string GetContentType() => "Application/json";

        public byte[] GetResult() => new byte[0];

        public HttpStatusCode GetStatusCode() => HttpStatusCode.NoContent;
    }
}
