using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Attributes
{
    public class HttpGetAttribute : HttpMethodAttribute
    {
        public HttpGetAttribute(string name = "") : base(name, "get") { }
    }
}