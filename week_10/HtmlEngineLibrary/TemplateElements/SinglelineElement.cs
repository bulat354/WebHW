using NReco.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlEngineLibrary.TemplateElements
{
    //{{ <name> [(<expression>)] }}
    public abstract class SinglelineElement : ITemplateElement
    {
        protected TemplateNode node;

        public SinglelineElement(TemplateNode node)
        {
            this.node = node;
        }

        public int GetEndIndex()
        {
            return node.Match.Index + node.Match.Length;
        }

        public int GetStartIndex()
        {
            return node.Match.Index;
        }

        public abstract string Render(string template, StatementVariables variables);

        protected object GetValue(string expression, StatementVariables variables)
        {
            var parser = new LambdaParser();
            return parser.Eval(expression, variables.GetValue);
        }
    }
}
