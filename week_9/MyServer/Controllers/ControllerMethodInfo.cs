using MyServer.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Controllers
{
    internal class ControllerMethodInfo
    {
        public readonly string Name;
        public readonly bool IsVoid;

        private readonly object? controller;
        private readonly MethodInfo method;
        private readonly HttpMethodAttribute httpAttribute;
        private readonly ParameterInfo[] parameters;

        public ControllerMethodInfo(object? controller, Type type, MethodInfo method)
        {
            if (Attribute.IsDefined(type, typeof(ApiControllerAttribute)))
            {
                this.controller = controller;

                var controllerName = type.GetCustomAttribute<ApiControllerAttribute>().Name.ToLower();
                if (controllerName == null)
                    controllerName = type.Name.ToLower();

                if (Attribute.IsDefined(method, typeof(HttpMethodAttribute)))
                {
                    this.method = method;
                    httpAttribute = method.GetCustomAttribute<HttpMethodAttribute>();
                    parameters = method.GetParameters();

                    var methodName = httpAttribute.Name.ToLower();
                    if (methodName.StartsWith('.'))
                        methodName = method.Name.ToLower() + methodName;
                    else if (methodName == null)
                        methodName = method.Name.ToLower() + ".get";

                    Name = $"{controllerName}.{methodName}";
                    IsVoid = method.ReturnType == typeof(void);
                }
                else
                    throw new ArgumentException("Method that was given doesn't have attribute HttpMethod");
            }
            else
                throw new ArgumentException("Controller that was given doesn't have attribute HttpController");
        }

        public static IEnumerable<ControllerMethodInfo> GetMethods(Type controller)
        {
            var obj = Activator.CreateInstance(controller);
            return controller.GetMethods()
                .Where(m => Attribute.IsDefined(m, typeof(HttpMethodAttribute)))
                .Select(m => new ControllerMethodInfo(obj, controller, m));
        }

        public object? Invoke(HttpListenerRequest request)
        {
            NameValueCollection strParams;
            try
            {
                strParams = httpAttribute.Parse(request);
            }
            catch
            {
                throw new InvalidRequestException("Cannot parse giving input.");
            }
            object?[] objParams = new object?[parameters.Length];

            try
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    var param = parameters[i];
                    var str = strParams[param.Name];
                    if (str == null)
                        return null;
                    objParams[i] = Convert.ChangeType(strParams[param.Name], param.ParameterType);
                }
            }
            catch
            {
                throw new NotFoundException("Cannot find method with the same parameter types");
            }

            return method.Invoke(controller, objParams);
        }
    }
}
