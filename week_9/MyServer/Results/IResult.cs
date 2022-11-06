using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Results
{
    public interface IResult
    {
        byte[] GetResult();
        string GetContentType();
        HttpStatusCode GetStatusCode();
    }

    public interface IResult<T> : IResult { }
}
