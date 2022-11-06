using MyServer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Results
{
    public class FileResult : IResult
    {
        protected string path;

        public FileResult(string path)
        {
            this.path = path;
        }

        public string GetContentType() => FileManager.GetContentType(path);

        public byte[] GetResult() => File.ReadAllBytes(path);

        public HttpStatusCode GetStatusCode() => HttpStatusCode.OK;
    }
}
