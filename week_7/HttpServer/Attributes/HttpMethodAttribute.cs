using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Attributes
{
    /// <summary>
    /// Abstract class for HTTP method attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class HttpMethodAttribute : Attribute
    {
        public string Name { get; }

        public HttpMethodAttribute(string name)
        {
            Name = $"{name}.{GetMethodName()}";
        }

        public abstract string GetMethodName();

        public abstract NameValueCollection Parse(HttpListenerRequest request);
    }
}
