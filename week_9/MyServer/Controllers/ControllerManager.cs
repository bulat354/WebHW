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

        public static HttpResponse? MethodHandler(HttpListenerRequest request, Configs configs)
        {
            if (request.Url == null)
            {
                Debug.ShowWarning("Request without url. Skipped.");
                return HttpResponse.GetNotFoundResponse();
            }

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
                try
                {
                    var result = method.Invoke(request);

                    if (method.IsVoid)
                        return HttpResponse.GetEmptyResponse();

                    if (result == null)
                        return HttpResponse.GetNotFoundResponse();

                    if (result is HttpResponse resp)
                        return resp;

                    return new HttpResponse(HttpStatusCode.OK, result);
                }
                catch (NotFoundException e)
                {
                    Debug.ShowError(e.Message);
                    return HttpResponse.GetNotFoundResponse();
                }
                catch (InvalidRequestException e)
                {
                    Debug.ShowError(e.Message);
                    return HttpResponse.GetBadRequestResponse();
                }
            }
            else
            {
                return HttpResponse.GetNotFoundResponse();
            }
        }

        public static void Init()
        {
            methods = new Dictionary<string, ControllerMethodInfo>();
            foreach (var m in Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(ApiControllerAttribute)))
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
