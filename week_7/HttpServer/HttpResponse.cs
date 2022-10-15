using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyServer
{
    public class HttpResponse
    {
        public readonly HttpStatusCode StatusCode;
        public readonly string ContentType;
        public readonly byte[] Content;

        public bool IsSuccess { get { return StatusCode == HttpStatusCode.OK; } }

        public static HttpResponse GetNotFoundResponse()
        {
            Debug.FileNotFoundMsg();
            return new HttpResponse(HttpStatusCode.NotFound, "404: Not Found");
        }

        public static HttpResponse GetBadRequestResponse()
        {
            Debug.InvalidPathMsg();
            return new HttpResponse(HttpStatusCode.BadRequest, "400: Bad Request");
        }

        public static HttpResponse GetInternalErrorResponse(Exception e)
        {
            Debug.UnknownErrorMsg(e);
            return new HttpResponse(HttpStatusCode.InternalServerError, "500: Server Error");
        }

        public static HttpResponse GetEmptyResponse()
        {
            return new HttpResponse(HttpStatusCode.NoContent, (byte[])null);
        }

        public HttpResponse(HttpStatusCode statusCode, byte[] content, string contentType = "text/plane")
        {
            StatusCode = statusCode;
            ContentType = contentType;
            Content = content;
        }

        public HttpResponse(HttpStatusCode statusCode, string content, string contentType = "text/plane") 
            : this(statusCode, Encoding.ASCII.GetBytes(content), contentType) { }

        public HttpResponse(HttpStatusCode statusCode, object content, string contentType = "Application/json")
            : this(statusCode, JsonSerializer.Serialize(content), contentType) { }

        public void ToListenerResponse(HttpListenerResponse response)
        {
            response.StatusCode = (int)StatusCode;
            if (Content != null)
            {
                response.ContentType = ContentType;
                response.OutputStream.Write(Content, 0, Content.Length);
                response.OutputStream.Close();
            }
        }
    }
}
