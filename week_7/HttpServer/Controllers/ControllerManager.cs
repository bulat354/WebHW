using MyServer.Attributes;
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

        static ControllerManager()
        {
            methods = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(ApiControllerAttribute)))
                .SelectMany(t => ControllerMethodInfo.GetMethods(t))
                .ToDictionary(c => c.Name, c => c);
        }

        public static HttpResponse MethodHandler(HttpListenerRequest request, Configs configs)
        {
            var segments = request.Url.Segments;
            if (segments.Length < 2) return HttpResponse.GetNotFoundResponse();

            string controllerName = segments[1].Replace("/", "").ToLower();
            string httpMethodName = request.HttpMethod.ToLower();
            string methodName = (segments.Length < 3)
                ? ""
                : segments[2].Replace("/", "").ToLower();

            string fullName = $"{controllerName}.{methodName}.{httpMethodName}";

            if (methods.TryGetValue(fullName, out var method))
            {
                var result = method.Invoke(request);

                if (method.IsVoid)
                    return HttpResponse.GetEmptyResponse();

                if (result == null)
                    return HttpResponse.GetNotFoundResponse();

                return new HttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(result), "Application/json");
            }
            else
            {
                return HttpResponse.GetNotFoundResponse();
            }
        }
    }
}
