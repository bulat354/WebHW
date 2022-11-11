using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Controllers
{
    public abstract class ControllerBase
    {
        internal HttpListenerResponse _response;
        internal HttpListenerRequest _request;
    }
}
