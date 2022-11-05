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
    public class ApiControllerAttribute : Attribute
    {
        public string Name { get; protected set; }

        public ApiControllerAttribute(string name)
        {
            Name = name;
        }

        public ApiControllerAttribute() { }
    }
}
