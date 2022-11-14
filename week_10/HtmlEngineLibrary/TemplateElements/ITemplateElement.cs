using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEngineLibrary.TemplateElements
{
    //{{ <name> (<expression>) [begin|continue|end] }}
    public interface ITemplateElement
    {
        string Render(string template, StatementVariables variables);
        int GetEndIndex();
        int GetStartIndex();
    }
}
