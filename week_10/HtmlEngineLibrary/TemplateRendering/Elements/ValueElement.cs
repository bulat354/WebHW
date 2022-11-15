

namespace HtmlEngineLibrary.TemplateRendering
{
    internal class ValueElement : SinglelineElement
    {
        public ValueElement(TemplateNode node) : base(node) { }

        public override string Render(string template, StatementVariables variables)
        {
            return GetValue(node.Content, variables).ToString();
        }
    }
}
