using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Attributes
{
    public class HttpPostAttribute : HttpMethodAttribute
    {
        public HttpPostAttribute(string name = "") : base(name, "post") { }
    }
}
