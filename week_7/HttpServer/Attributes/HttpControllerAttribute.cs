using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Attributes
{
    /// <summary>
    /// New controllers must have it
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class HttpControllerAttribute : Attribute
    {
        public string Name { get; protected set; }

        public HttpControllerAttribute(string name)
        {
            Name = name;
        }

        public HttpControllerAttribute() { }
    }
}
