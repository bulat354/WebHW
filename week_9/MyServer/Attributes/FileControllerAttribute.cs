using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FileControllerAttribute : Attribute
    {
        public readonly string FileName;

        public FileControllerAttribute(string fileName)
        {
            FileName = fileName;
        }
    }
}
