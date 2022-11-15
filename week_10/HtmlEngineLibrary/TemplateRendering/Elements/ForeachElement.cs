using System.Collections;
using System.Text;

namespace HtmlEngineLibrary.TemplateRendering
{
    internal class ForeachElement : MultilineElement
    {
        public ForeachElement(TemplateNode begin, TemplateNode end) : base(begin, end)
        {
        }

        public override string Render(string template, StatementVariables variables)
        {
            var split = begin.Expression.Substring(1, begin.Expression.Length - 2).Split(" in ").Select(x => x.Trim()).ToArray();
            if (split.Length != 2)
                throw new ArgumentException("Wrong expression near foreach: " + begin.Expression);

            var enumerable = GetValue(split[1], variables);
            if (enumerable is IEnumerable items)
            {
                var renderer = new TemplateRenderer();
                var result = new StringBuilder();
                var part = GetTemplatePart(template, begin, end);
                foreach (object item in items)
                {
                    var local = new StatementVariables(variables);
                    local.AddLocalVariable(item, split[0]);
                    result.AppendLine(renderer.Render(part, local));
                }
                return result.ToString();
            }

            throw new ArgumentException(split[1] + " is not IEnumerable");
        }
    }
}
