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
        protected string _message;

        public ErrorResult(HttpStatusCode statusCode, string message)
        {
            this.statusCode = statusCode;
            this._message = message;
        }

        public string GetContentType() => "text/plain";

        public byte[] GetResult() => Encoding.UTF8.GetBytes($"Error {(int)statusCode}: {_message}");

        public HttpStatusCode GetStatusCode() => statusCode;

        public static ErrorResult NotFound(string message = "Not Found")
            => new ErrorResult(HttpStatusCode.NotFound, message);
        public static ErrorResult InternalError(string message = "Internal Server Error")
            => new ErrorResult(HttpStatusCode.InternalServerError, message);
        public static ErrorResult BadRequest(string message = "Bad Request")
            => new ErrorResult(HttpStatusCode.NotFound, message);
        public static ErrorResult Unauthorized(string message = "Unauthorized. Access Denied")
            => new ErrorResult(HttpStatusCode.Unauthorized, message);
    }
}
