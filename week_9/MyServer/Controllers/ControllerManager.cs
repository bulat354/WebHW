using MyServer.Attributes;
using MyServer.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyServer.Controllers
{
    public class ControllerManager
    {
        private static Dictionary<string, ControllerMethodInfo> methods;

        public static IResult MethodHandler(HttpListenerRequest request, HttpListenerResponse response, Configs configs)
        {
            if (request.Url == null)
            {
                Debug.ShowWarning("Request without url. Skipped.");
                return new BadRequestResult();
            }

            var segments = request.Url.Segments;
            if (segments.Length < 2)
                return new NotFoundResult();

            string controllerName = segments[1].Replace("/", "").ToLower();
            string httpMethodName = request.HttpMethod.ToLower();
            string methodName = (segments.Length < 3)
                ? ""
                : segments[2].Replace("/", "").ToLower();

            string fullName = $"{controllerName}.{methodName}.{httpMethodName}";

            if (methods.TryGetValue(fullName, out var method))
            {
                var result = method.Invoke(request, response);

                if (method.IsVoid || result == null)
                    return new EmptyResult();

                if (result is IResult resp)
                    return resp;

                return new ObjectResult<object>(result);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        public static void Init()
        {
            methods = new Dictionary<string, ControllerMethodInfo>();
            foreach (var m in Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(ApiControllerAttribute)) 
                    && t.IsSubclassOf(typeof(ControllerBase))
                    && !t.IsAbstract && t.IsClass)
                .SelectMany(t => ControllerMethodInfo.GetMethods(t)))
            {
                if (methods.ContainsKey(m.Name))
                {
                    Debug.ShowError($"Method with name {m.Name} already exists.");
                    Debug.ShowWarning("This method will be skipped.");
                    continue;
                }
                methods.Add(m.Name, m);
            }
        }
    }
}
