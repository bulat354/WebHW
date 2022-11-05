using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MyServer.Attributes;
using System.Net;

namespace MyServer.Controllers
{
    public class FileMethodInfo
    {
        public readonly string Name;

        private readonly object controller;
        private readonly Dictionary<string, MethodInfo> methods;

        public FileMethodInfo(Type type)
        {
            if (Attribute.IsDefined(type, typeof(FileControllerAttribute)))
            {
                controller = Activator.CreateInstance(type);

                Name = type.GetCustomAttribute<FileControllerAttribute>().FileName.ToLower();

                methods = new Dictionary<string, MethodInfo>();
                foreach (var method in type.GetMethods()
                                    .Where(m => Attribute.IsDefined(m, typeof(DataSourceAttribute)) && m.IsPublic && m.ReturnType != typeof(void)))
                {
                    var name = string.Join('.', method.GetParameters().Select(p => p.Name));
                    if (methods.ContainsKey(name))
                        Debug.ShowWarning($"Class {type.Name} contains DataSource methods with the same parameter names.");
                    else
                        methods.Add(name, method);
                }
            }
            else
                throw new ArgumentException("Controller that was given doesn't have attribute FileController");
        }

        public object? Invoke(HttpListenerRequest request)
        {
            var strParams = request.QueryString;
            var name = string.Join('.', strParams.AllKeys);

            if (methods.ContainsKey(name))
            {
                var method = methods[name];
                var parameters = method.GetParameters();

                object?[] objParams = new object?[parameters.Length];

                try
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var param = parameters[i];
                        var str = strParams[param.Name];
                        objParams[i] = Convert.ChangeType(strParams[param.Name], param.ParameterType);
                    }
                }
                catch
                {
                    throw new NotFoundException("Couldn't find method with the same parameter types");
                }

                return method.Invoke(controller, objParams);
            }
            else
            {
                throw new NotFoundException("Couldn't find method with the same parameter names");
            }
        }
    }
}
