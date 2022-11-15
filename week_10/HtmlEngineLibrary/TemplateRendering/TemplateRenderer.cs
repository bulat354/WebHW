using System.Text;
using System.Text.RegularExpressions;

namespace HtmlEngineLibrary.TemplateRendering
{
    public class TemplateRenderer
    {
        public string Render(string template, StatementVariables variables)
        {
            var result = new StringBuilder();
            var cursor = 0;

            var nodes = GetNodes(template);
            var elements = GetTemplateElements(nodes, 0);

            foreach (var element in elements)
            {
                result.Append(template, cursor, element.GetStartIndex() - cursor);
                result.Append(element.Render(template, variables));
                cursor = element.GetEndIndex();
            }
            result.Append(template, cursor, template.Length - cursor);

            return result.ToString();
        }

        private IEnumerable<TemplateNode> GetNodes(string template)
        {
            foreach (Match match in Regex.Matches(template, @"{{[^{}]*?}}", RegexOptions.Singleline))
            {
                yield return new TemplateNode(match);
            }
        }

        private IEnumerable<ITemplateElement> GetTemplateElements(IEnumerable<TemplateNode> nodes, int nesting)
        {
            var level = 0;
            var stack = new Stack<TemplateNode>();
            foreach (var node in nodes)
            {
                if (node.IsBegin)
                {
                    level++;
                    if (level == nesting + 1)
                        stack.Push(node);
                }
                else if (node.IsEnd)
                {
                    level--;
                    if (level == nesting)
                    {
                        stack.Push(node);
                        yield return GetElement(GetNodesFromStack(stack).Reverse().ToArray());
                    }
                }
                else if (node.IsContinuation)
                {
                    if (level == nesting + 1)
                        stack.Push(node);
                }
                else if (level == nesting)
                    yield return GetElement(node);

                if (level < 0)
                    throw new ArgumentException("Wrong nesting in template");
            }

            if (stack.Count > 0)
                throw new ArgumentException("Wrong nesting in template");
        }

        private IEnumerable<TemplateNode> GetNodesFromStack(Stack<TemplateNode> stack)
        {
            while (stack.Count > 0)
            {
                var peek = stack.Peek();
                if (peek.IsEnd || peek.IsContinuation)
                    yield return stack.Pop();
                else if (peek.IsBegin)
                {
                    yield return stack.Pop();
                    yield break;
                }
            }
        }

        private ITemplateElement GetElement(params TemplateNode[] nodes)
        {
            if (nodes.Length == 0)
                throw new ArgumentException("Wrong nodes");

            if (nodes.Length == 1)
                return GetSinglelineElement(nodes[0]);

            if (nodes.Length == 2)
                return GetMultilineElement(nodes);

            return GetContinuedElement(nodes);
        }

        private SinglelineElement GetSinglelineElement(TemplateNode node)
        {
            if (node.IsBegin || node.IsEnd || node.IsContinuation)
                throw new ArgumentException("Wrong node");

            switch (node.Name)
            {

                default:
                    return new ValueElement(node);
            }
        }

        private MultilineElement GetMultilineElement(TemplateNode[] nodes)
        {
            if (!nodes[0].IsBegin || !nodes[1].IsEnd || nodes[0].Name != nodes[1].Name)
                throw new ArgumentException("Wrong nodes");

            switch (nodes[0].Name)
            {
                case "if":
                    return new IfElement(nodes[0], nodes[1]);
                case "foreach":
                    return new ForeachElement(nodes[0], nodes[1]);
            }

            throw new ArgumentException("Unknown node " + nodes[0].Name);
        }

        private ContinuedElement GetContinuedElement(TemplateNode[] nodes)
        {
            var begin = nodes[0];
            var end = nodes[nodes.Length - 1];
            var continues = nodes.Skip(1).SkipLast(1);

            if (!begin.IsBegin || !end.IsEnd || !continues.All(x => x.IsContinuation) || begin.Name != end.Name)
                throw new ArgumentException("Wrong nodes");

            switch (begin.Name)
            {
                case "if":
                    return new IfElement(begin, end, continues.ToArray());
            }

            throw new ArgumentException("Unknown node " + begin.Name);
        }
    }
}
