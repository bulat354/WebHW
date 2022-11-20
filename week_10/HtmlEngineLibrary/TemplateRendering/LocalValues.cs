

namespace HtmlEngineLibrary.TemplateRendering
{
    internal class LocalValues
    {
        private LocalValues parent;
        private object root;
        private Dictionary<string, object> values;

        public LocalValues(object root)
        {
            this.root = root;
            values = new Dictionary<string, object>();
        }

        public LocalValues(LocalValues parent, params (string, object)[] values)
        {
            this.parent = parent;
            this.values = new Dictionary<string, object>();
            foreach (var value in values)
            {
                if (!this.values.TryAdd(value.Item1, value.Item2))
                    throw new ArgumentException($"Value with name {value.Item1} already exists");
            }
        }

        public object? GetValue(string name)
        {
            object? result = null;

            if (values.TryGetValue(name, out result))
                return result;

            if (root != null && TryGetValueFromRoot(name, out result))
                return result;

            return parent.GetValue(name);
        }

        private bool TryGetValueFromRoot(string name, out object? value)
        {
            var field = root.GetType().GetField(name);
            value = null;
            if (field == null)
            {
                var property = root.GetType().GetProperty(name);
                if (property != null)
                    value = property.GetValue(root);
            }
            else
                value = field.GetValue(root);
            return value != null;
        }
    }
}
