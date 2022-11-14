using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlEngineLibrary.TemplateElements
{
    //{{ <name> [(expression)] begin }}
    //[{{ <name> [(expression)] continue }}]
    //...
    //[{{ <name> [(expression)] continue }}]
    //{{ <name> end }}
    public abstract class ContinuedElement : MultilineElement
    {
        protected TemplateNode[] continues;

        protected ContinuedElement(TemplateNode begin, TemplateNode end, params TemplateNode[] continues) : base(begin, end)
        {
            if (!continues.All(x => x.IsContinuation))
                throw new ArgumentException("At least one of continues isn't continuation");

            this.continues = continues;
        }
    }
}
