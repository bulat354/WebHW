using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NReco.Linq;

namespace HtmlEngineLibrary.TemplateElements
{
    public class ValueElement : SinglelineElement
    {
        public ValueElement(TemplateNode node) : base(node) { }

        public override string Render(string template, StatementVariables variables)
        {
            return GetValue(node.Content, variables).ToString();
        }
    }
}
