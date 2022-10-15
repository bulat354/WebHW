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
                .Where(t => Attribute.IsDefined(t, typeof(HttpControllerAttribute)))
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
                ? methodName = ""
                : methodName = segments[2].Replace("/", "").ToLower();
            int paramCount = Math.Max(0, segments.Length - 3);

            string fullName = $"{controllerName}.{methodName}.{httpMethodName}.{paramCount}";

            if (methods.TryGetValue(fullName, out var method))
            {
                string[] strParams = request.Url.Segments
                                        .Skip(3)
                                        .Select(s => s.Replace("/", ""))
                                        .ToArray();
                var result = method.Invoke(strParams);

                if (result == null)
                    return HttpResponse.GetEmptyResponse();
                if (result is HttpResponse response)
                    return response;

                return new HttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(result), "Application/json");
            }
            else
            {
                return HttpResponse.GetNotFoundResponse();
            }
        }
    }
}
