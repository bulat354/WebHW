using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyServer.Results
{
    public class ObjectResult<T> : IResult<T>
    {
        protected T content;
        protected string contentType;
        protected HttpStatusCode statusCode;

        protected ObjectResult(T content, string contentType, HttpStatusCode statusCode)
        {
            this.content = content;
            this.contentType = contentType;
            this.statusCode = statusCode;
        }

        public ObjectResult(T content)
            : this(content, "Application/json", HttpStatusCode.OK) { }

        public virtual string GetContentType() => contentType;

        public virtual byte[] GetResult() => JsonSerializer.SerializeToUtf8Bytes(content, content.GetType());

        public virtual HttpStatusCode GetStatusCode() => statusCode;
    }
}
