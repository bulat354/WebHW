using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer
{
    public static class ServerPath
    {
        /// <param name="rawPath">HttpListenerRequest.RawUrl, for example '/google/index.html'. Starts with web root</param>
        /// <returns>path like './source/index.html'. Starts with local root</returns>
        public static string GetPath(string rawUrl, Configs configs)
        {
            if (rawUrl == null)
                return configs.LocalRoot + configs.DefaultFile;

            var path = rawUrl.Substring(configs.WebRoot.Length); //delets '/google' part from rawUrl
            if (path.Length < 2) //for urls like "http://localport:8080/google/"
                return configs.LocalRoot + configs.DefaultFile;

            return configs.LocalRoot + path;
        }
    }
}
