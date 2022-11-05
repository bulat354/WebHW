using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer
{
    public class NotFoundException : Exception
    {
        public NotFoundException() { }
        public NotFoundException(string message) : base(message) { }
    }

    public class InternalException : Exception
    {
        public InternalException() { }
        public InternalException(string message) : base(message) { }
    }

    public class InvalidRequestException : Exception
    {
        public InvalidRequestException() { }
        public InvalidRequestException(string message) : base(message) { }
    }
}
