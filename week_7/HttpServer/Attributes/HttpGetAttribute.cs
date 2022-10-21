using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Attributes
{
    public class HttpGetAttribute : HttpMethodAttribute
    {
        public HttpGetAttribute(string name = "") : base(name) { }

        public override string GetMethodName()
        {
            return "get";
        }

        public override NameValueCollection Parse(HttpListenerRequest request)
        {
            return request.QueryString;
        }
    }
}