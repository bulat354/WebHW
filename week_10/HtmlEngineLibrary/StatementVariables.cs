using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlEngineLibrary
{
    public class StatementVariables
    {
        public object root;
        public Dictionary<string, object> local;

        public StatementVariables(object root)
        {
            this.root = root;
            local = new Dictionary<string, object>();
        }

        public StatementVariables(StatementVariables variables)
        {
            root = variables.root;
            local = variables.local
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public void AddLocalVariable(object obj, string name)
        {
            if (local.ContainsKey(name))
                throw new ArgumentException("Object with the same name already exists");
            local[name] = obj;
        }

        public object GetValue(string path)
        {
            var split = path.Split('.');

            if (split.Length <= 0)
                throw new ArgumentException("Wrong path " + path);

            if (local.ContainsKey(split[0]))
                return GetPropertyValue(local[split[0]], split.Skip(1));
            return GetPropertyValue(root, split);
        }

        private object GetPropertyValue(object model, IEnumerable<string> path)
        {
            PropertyInfo property = null;
            Type type = model.GetType();
            object obj = model;

            foreach (var item in path)
            {
                if (obj == null)
                    throw new ArgumentException("Property is null");

                property = type.GetProperty(item);
                if (property == null)
                    throw new ArgumentException("Wrong property name: " + item);

                type = property.PropertyType;
                obj = property.GetValue(obj);

                if (type == null)
                    throw new ArgumentException($"Wrong property type: {type}");
            }
            return obj;
        }

        private static Regex variableRegex = new Regex(@"([a-zA-Z0-9]+\.)*[a-zA-Z0-9]+");

        public string ReplaceAll(string expression)
        {
            var result = new StringBuilder();
            var cursor = 0;
            foreach (Match match in variableRegex.Matches(expression))
            {
                result.Append(expression, cursor, match.Index - cursor);
                result.Append(CastToString(GetValue(match.Value).ToString()));
                cursor = match.Index + match.Length;
            }
            result.Append(expression, cursor, expression.Length - cursor);
            return result.ToString();
        }

        private static string CastToString(object obj)
        {
            if (obj is string str)
                return $"\"{str}\"";
            return obj.ToString();
        }
    }
}
