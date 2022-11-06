using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Results
{
    public class ErrorResult : IResult
    {
        protected HttpStatusCode statusCode;
        protected string message;

        public ErrorResult(HttpStatusCode statusCode, string message)
        {
            this.statusCode = statusCode;
            this.message = message;
        }

        public string GetContentType() => "text/plain";

        public byte[] GetResult() => Encoding.UTF8.GetBytes($"Error {(int)statusCode}: {message}");

        public HttpStatusCode GetStatusCode() => statusCode;
    }
}
