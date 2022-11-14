using NReco.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlEngineLibrary.TemplateElements
{
    //{{ <name> [(expression)] begin }}
    //{{ <name> end }}
    public abstract class MultilineElement : ITemplateElement
    {
        protected TemplateNode begin;
        protected TemplateNode end;

        public MultilineElement(TemplateNode begin, TemplateNode end)
        {
            if (!begin.IsBegin || !end.IsEnd)
                throw new ArgumentException("Begin isn't begin or end isn't end");

            this.begin = begin;
            this.end = end;
        }

        public int GetEndIndex()
        {
            return end.Match.Index + end.Match.Length;
        }

        public int GetStartIndex()
        {
            return begin.Match.Index;
        }

        public abstract string Render(string template, StatementVariables variables);

        protected string GetTemplatePart(string template, TemplateNode first, TemplateNode second)
        {
            var start = first.Match.Index + first.Match.Length;
            var count = second.Match.Index - start;
            return template.Substring(start, count);
        }

        protected object GetValue(string expression, StatementVariables variables)
        {
            var parser = new LambdaParser();
            return parser.Eval(expression, variables.GetValue);
        }
    }
}
