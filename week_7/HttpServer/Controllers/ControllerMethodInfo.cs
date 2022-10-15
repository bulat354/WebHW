using MyServer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Controllers
{
    internal class ControllerMethodInfo
    {
        public string Name { get; }
        public object Controller { get; }
        public MethodInfo Method { get; }
        public Type[] ParameterTypes { get; }
        public int ParametersCount { get; }

        public ControllerMethodInfo(object controller, string controllerName, MethodInfo method, string methodName)
        {
            if (controllerName == null)
                controllerName = controller.GetType().Name.ToLower();
            if (methodName == null)
                throw new ArgumentException();

            Controller = controller;
            Method = method;
            ParameterTypes = method.GetParameters()
                .Select(param => param.ParameterType)
                .ToArray();
            ParametersCount = ParameterTypes.Length;

            Name = $"{controllerName}.{methodName}.{ParametersCount}";
        }

        public static IEnumerable<ControllerMethodInfo> GetMethods(Type controller)
        {
            var obj = Activator.CreateInstance(controller);
            return controller.GetMethods()
                .Where(m => Attribute.IsDefined(m, typeof(HttpMethodAttribute)))
                .Select(m => new ControllerMethodInfo
                (
                    obj, controller.GetCustomAttribute<HttpControllerAttribute>().Name.ToLower(),
                    m, m.GetCustomAttribute<HttpMethodAttribute>().Name.ToLower()
                ));
        }

        public object? Invoke(string[] strParams)
        {
            object[] objParams = strParams
                .Take(ParameterTypes.Length)
                .Select((param, i) => Convert.ChangeType(param, ParameterTypes[i]))
                .ToArray();
            return Method.Invoke(Controller, objParams);
        }
    }
}
