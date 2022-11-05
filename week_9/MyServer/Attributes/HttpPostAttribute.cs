using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Attributes
{
    public class HttpPostAttribute : HttpMethodAttribute
    {
        public HttpPostAttribute(string name = "") : base(name) { }

        public override string GetMethodName()
        {
            return "post";
        }

        public override NameValueCollection Parse(HttpListenerRequest request)
        {
            var text = "";
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                text = reader.ReadToEnd();
            }
            var result = new NameValueCollection();
            foreach (var tuple in text.Split('&')
                                      .Select(x => x.Split('=')))
            {
                result.Set(tuple[0], tuple[1]);
            }
            return result;
        }
    }
}
