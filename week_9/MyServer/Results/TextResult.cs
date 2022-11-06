using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Results
{
    public class TextResult : IResult
    {
        protected string content;
        protected HttpStatusCode statusCode;

        public TextResult(string content, HttpStatusCode statusCode)
        {
            this.content = content;
            this.statusCode = statusCode;
        }

        public string GetContentType() => "text/plain";

        public byte[] GetResult() => Encoding.UTF8.GetBytes(content);

        public HttpStatusCode GetStatusCode() => statusCode;
    }
}
