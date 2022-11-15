

namespace HtmlEngineLibrary.TemplateRendering
{
    //{{ <name> [(expression)] begin }}
    //[{{ <name> [(expression)] continue }}]
    //...
    //[{{ <name> [(expression)] continue }}]
    //{{ <name> end }}
    internal abstract class ContinuedElement : MultilineElement
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
