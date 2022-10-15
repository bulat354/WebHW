using System;
using System.Collections.Generic;
using System.Linq;
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
        public string Name { get; protected set; }

        public HttpMethodAttribute(string name, string httpMethod)
        {
            Name = $"{name}.{httpMethod}";
        }
    }
}
